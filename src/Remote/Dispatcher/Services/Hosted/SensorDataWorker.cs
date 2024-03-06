using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Data;
using Devices.Dispatcher.Services.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Devices.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Auto = Devices.Dispatcher.Infrastructure.AutoMapperConfiguration;

namespace Devices.Dispatcher.Services.Hosted;

/// <summary>
/// Sensor data sensder
/// </summary>
public class SensorDataWorker : BackgroundService
{
   #region fields

   private readonly ILogger<SensorDataWorker> _logger;
   private readonly IServiceScopeFactory _scopeFactory;
   private readonly HubConnections _hubConnection;

   private DeviceSettings _deviceSettings;
   private int _packetSize;
   private int _dataSendingDelay;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public SensorDataWorker(ILogger<SensorDataWorker> logger, IServiceScopeFactory scopeFactory, HubConnections hubConnection)
   {
      _logger = logger;
      _scopeFactory = scopeFactory;
      _hubConnection = hubConnection;
   }

   #endregion

   #region Methods

   /// <inheritdoc/>
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      if (_hubConnection.Enabled)
      {
         using var scope = _scopeFactory.CreateScope();
         _deviceSettings = await scope.ServiceProvider.GetRequiredService<ISettingService>().LoadSettingAsync<DeviceSettings>();
         _packetSize = _deviceSettings.DataPacketSize == 0 ? 1000 : _deviceSettings.DataPacketSize;
         _dataSendingDelay = _deviceSettings.DataSendingDelay < 1000 ? 1000 : _deviceSettings.DataSendingDelay;

         while (!stoppingToken.IsCancellationRequested)
         {
            try
            {
               await SendSensorRecords(stoppingToken);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Sensor data worker error");
            }

            await Task.Delay(_deviceSettings.DataflowReconnectDelay, stoppingToken);
         }
      }
   }

   private async Task SendSensorRecords(CancellationToken stoppingToken)
   {
      using var scope2 = _scopeFactory.CreateScope();
      var grpcClient = scope2.ServiceProvider.GetRequiredService<DeviceCalls.DeviceCallsClient>();

      try
      {
         while (!stoppingToken.IsCancellationRequested)
         {
            var delayTask = Task.Delay(_dataSendingDelay, stoppingToken);
            using (var scope3 = _scopeFactory.CreateScope())
            {
               using var dbContext = scope3.ServiceProvider.GetRequiredService<AppDbContext>();

               var dataSet =
                   (from d in dbContext.SensorRecords.Where(x => !x.IsSent)
                    join s in dbContext.Sensors on d.SensorId equals s.Id
                    orderby s.PriorityType, d.EventTimestamp descending
                    select d).Take(_packetSize);

               if (dataSet.Any())
               {
                  var recordsProto = Auto.Mapper.Map<List<SensorRecordProto>>(dataSet.ToList());
                  var request = new SensorRecordProtos();
                  request.Records.Add(recordsProto);

                  await grpcClient.SensorDataCallAsync(request, cancellationToken: stoppingToken, deadline: DateTime.UtcNow.AddMinutes(1));

                  await dbContext.SensorRecords.ExecuteUpdateAsync(x => x.SetProperty(p => p.IsSent, true), cancellationToken: stoppingToken);
               }
            }
            await delayTask;
         }
      }
      catch (Exception ex)
      {
         _logger.LogInformation(ex, "Data sending error");
      }
   }

   #endregion
}