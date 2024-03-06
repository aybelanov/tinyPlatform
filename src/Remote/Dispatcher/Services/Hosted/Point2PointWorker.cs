using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Services.Settings;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Devices.Proto;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Dispatcher.Services.Hosted;

/// <summary>
/// Point-to-point worker
/// </summary>
public class Point2PointWorker : BackgroundService
{
   #region fields

   private readonly ILogger<Point2PointWorker> _logger;
   private readonly IServiceScopeFactory _scopeFactory;
   private readonly HubConnections _hubConnections;
   private readonly IPoint2PointService _p2pService;

   private DeviceSettings _deviceSettings;
   private ICommandService _commandService;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public Point2PointWorker(ILogger<Point2PointWorker> logger, IServiceScopeFactory scopeFactory, HubConnections hubConnection, IPoint2PointService p2pService)
   {
      _logger = logger;
      _scopeFactory = scopeFactory;
      _hubConnections = hubConnection;
      _p2pService = p2pService;
   }

   #endregion

   #region Methods

   ///<inheritdoc/>
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      if (_hubConnections.Enabled)
      {
         using var scope = _scopeFactory.CreateScope();
         _commandService = scope.ServiceProvider.GetRequiredService<ICommandService>();
         _deviceSettings = await scope.ServiceProvider.GetRequiredService<ISettingService>().LoadSettingAsync<DeviceSettings>();
         var dataflowReconnectDelay = _deviceSettings.DataflowReconnectDelay < 10000 ? 10000 : _deviceSettings.DataflowReconnectDelay;

         while (!stoppingToken.IsCancellationRequested)
         {
            try
            {
               await PointToPointConnect(stoppingToken);
            }
            catch (Exception ex)
            {
               _logger.LogWarning(ex, "Point-2-point connect is failed.");
            }

            await Task.Delay(dataflowReconnectDelay, stoppingToken);
         }
      }
   }


   private async Task PointToPointConnect(CancellationToken stoppingToken)
   {
      //using var call = _grpcClient.Point2PointStream(cancellationToken: stoppingToken);
      using var scope = _scopeFactory.CreateScope();
      var grpcClient = scope.ServiceProvider.GetRequiredService<DeviceCalls.DeviceCallsClient>();
      using var call = grpcClient.Point2PointStream(cancellationToken: stoppingToken);

      _p2pService.AddServerNotification(new ClientMsg()
      {
         CommonResponse = new() { Notification = $"Point-to-point channel of {_hubConnections.SystemName} is started at {DateTime.UtcNow} UTC." }
      });

      // server stream
      var readTask = Task.Run(async () =>
      {
         try
         {
            await foreach (var message in call.ResponseStream.ReadAllAsync(stoppingToken))
            {
               switch (message.ResultCase)
               {
                  case ServerMsg.ResultOneofCase.Command:
                     await _commandService.ExecuteCommand(message);
                     break;

                  case ServerMsg.ResultOneofCase.Message:
                     _logger.LogInformation(message.Message);
                     break;
               }
            }
         }
         catch (Exception ex)
         {
            _logger.LogInformation(ex.Message, "Poin-2-point server side error");
         }
         finally
         {
            _p2pService.StopNotify();
            call?.Dispose();
         }

      }, stoppingToken);

      try
      {
         // device (client) stream
         while (!readTask.IsCompleted && !stoppingToken.IsCancellationRequested)
         {
            var notify = await _p2pService.GetNotification();
            var notifiction = notify();
            if (notifiction != null)
               await call.RequestStream.WriteAsync(notifiction, stoppingToken);
         }
      }
      catch (Exception ex)
      {
         _logger.LogInformation(ex.Message, "Poin-2-point client side error");
      }
      finally
      {
         await call.RequestStream.CompleteAsync();
         _p2pService.StopNotify();
         call?.Dispose();
      }
   }

   #endregion
}