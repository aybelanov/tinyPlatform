using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Data;
using Devices.Dispatcher.Domain;
using Devices.Dispatcher.Services.Settings;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Devices.Proto;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Auto = Devices.Dispatcher.Infrastructure.AutoMapperConfiguration;

namespace Devices.Dispatcher.Infrastructure;

/// <summary>
/// Represents a synchronization method handler
/// </summary>
public class ConfigurationSyncMessageHandler : DelegatingHandler, IDisposable
{
   #region fields

   private readonly IHostApplicationLifetime _applicationLifetime;
   private readonly ILogger<ConfigurationSyncMessageHandler> _logger;
   private readonly ISettingService _settingService;
   private readonly AppDbContext _dbContext;
   private readonly IServiceScopeFactory _serviceScopeFactory;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public ConfigurationSyncMessageHandler(IHostApplicationLifetime applicationLifetime, ILogger<ConfigurationSyncMessageHandler> logger, IServiceScopeFactory serviceScopeFactory,
       ISettingService settingService, AppDbContext dbContext)
   {
      _applicationLifetime = applicationLifetime;
      _logger = logger;
      _settingService = settingService;
      _dbContext = dbContext;
      _serviceScopeFactory = serviceScopeFactory;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Handles a response
   /// </summary>
   /// <param name="request">Request</param>
   /// <param name="cancellationToken">Cancellation token</param>
   /// <returns>response</returns>
   protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   {
      var response = await base.SendAsync(request, cancellationToken);
      if (response.StatusCode == System.Net.HttpStatusCode.UpgradeRequired)
         await UpdateConfiguration(cancellationToken);

      return response;
   }

   #endregion

   #region Utilities

   private async Task UpdateConfiguration(CancellationToken cancellationToken)
   {
      //var grpcClient = _grpcClientFactory.CreateClient<DeviceCalls.DeviceCallsClient>(Defaults.HubGrpcClient);
      using var scope = _serviceScopeFactory.CreateScope();
      var grpcClient = scope.ServiceProvider.GetRequiredService<DeviceCalls.DeviceCallsClient>();
      var deviceSetting = await _settingService.LoadSettingAsync<DeviceSettings>();

      try
      {
         var reply = await grpcClient.ConfigurationCallAsync(new Empty(), cancellationToken: cancellationToken, deadline: DateTime.UtcNow.AddMinutes(1));
         var device = Auto.Mapper.Map(reply, deviceSetting);
         await _settingService.SaveSettingAsync(device);

         var sensors = Auto.Mapper.Map<List<Sensor>>(reply.Sensors);
         await _dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM {nameof(AppDbContext.Sensors)};", cancellationToken);
         //await _dbContext.Database.ExecuteSqlRawAsync($"Delete from {nameof(_dbContext.Sensors)}", cancellationToken);
         _dbContext.Sensors.AddRange(sensors);
         await _dbContext.SaveChangesAsync(cancellationToken);

         _applicationLifetime.StopApplication();
      }
      catch (Exception ex)
      {
         _logger.LogWarning(message: "Cannot update configuration", exception: ex);
      }
      finally
      {

      }
   }

   #endregion
}