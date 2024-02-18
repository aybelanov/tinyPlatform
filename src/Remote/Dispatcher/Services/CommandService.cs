using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Data;
using Devices.Dispatcher.Domain;
using Devices.Dispatcher.Services.Settings;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.Devices.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auto = Devices.Dispatcher.Infrastructure.AutoMapperConfiguration;

namespace Devices.Dispatcher.Services;

/// <summary>
/// Represents a command servie implementation
/// </summary>
public class CommandService : ICommandService
{
   #region fields

   private readonly IHostApplicationLifetime _applicationLifetime;
   private readonly ILogger<CommandService> _logger;
   private readonly IPoint2PointService _p2p;
   private readonly AppDbContext _dbContext;
   private readonly ISettingService _settingService;
   private readonly DeviceCalls.DeviceCallsClient _grpcClient;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public CommandService(IHostApplicationLifetime applicationLifetime, ILogger<CommandService> logger, IPoint2PointService p2p,
       AppDbContext appDbContext, ISettingService settingService, DeviceCalls.DeviceCallsClient grpcClient)
   {
      _applicationLifetime = applicationLifetime;
      _logger = logger;
      _grpcClient = grpcClient;
      _p2p = p2p;
      _dbContext = appDbContext;
      _settingService = settingService;
   }

   #endregion

   #region Methods

   /// <inheritdoc/>
   public Task ExecuteCommand(string command, params string[] args)
   {
      throw new NotImplementedException();
   }

   /// <inheritdoc/>
   public Task ExecuteCommand(Action command)
   {
      throw new NotImplementedException();
   }

   /// <inheritdoc/>
   public async Task ExecuteCommand(ServerMsg message)
   {
      ArgumentNullException.ThrowIfNull(message);

      Func<Command, string, Task> commandMethod = (CommandEnum)message.Command.CommandId switch
      {
         CommandEnum.NOTIFY => NotifyExecuting,
         CommandEnum.STATUS => StatusExecuting,
         CommandEnum.RESTART => NotifyNotimplemented, //RestartExecuting,
         CommandEnum.UPDATECONFIG => NotifyNotimplemented, //UpdateConfigExecuting,
         _ => throw new NotImplementedException()
      };

      await commandMethod(message.Command, message.Sender);
   }

   /// <inheritdoc/>
   private async Task UpdateConfigExecuting(Command command, string sender)
   {

      var deviceSetting = await _settingService.LoadSettingAsync<DeviceSettings>();
      try
      {
         var reply = await _grpcClient.ConfigurationCallAsync(new Empty(), deadline: DateTime.UtcNow.AddMinutes(1));
         var device = Auto.Mapper.Map(reply, deviceSetting);
         await _settingService.SaveSettingAsync(device);

         var sensors = Auto.Mapper.Map<List<Sensor>>(reply.Sensors);
         await _dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM {nameof(AppDbContext.Sensors)};");
         //await _dbContext.Database.ExecuteSqlRawAsync($"Delete from {nameof(_dbContext.Sensors)}", cancellationToken);
         _dbContext.Sensors.AddRange(sensors);
         await _dbContext.SaveChangesAsync();
         _p2p.AddServerNotification(new ClientMsg()
         { 
            Receiver = sender,
            CommandResponse = new() { Notification = "Configuration has updated. Unit is restarting..." },
         });
       
         _applicationLifetime.StopApplication();
      }
      catch (Exception ex)
      {
         _logger.LogWarning(message: "Cannot update configuration", exception: ex);
         _p2p.AddServerNotification(new ClientMsg()
         {
            Receiver = sender,
            CommandResponse = new() { Notification = "Exeption.Cannot update configuration." },
         });
      }
      finally
      {

      }
   }

   /// <inheritdoc/>
   private Task RestartExecuting(Command command, string sender)
   {
      _p2p.AddServerNotification(new ClientMsg()
      {
         Receiver = sender,
         CommandResponse = new() { Notification = "Unit is restarting." },
      });
      _applicationLifetime.StopApplication();
      return Task.CompletedTask;
   }

   /// <inheritdoc/>
   private async Task StatusExecuting(Command command, string sender)
   {
      var chunk = "I'm alive!" + Environment.NewLine;

      var deviceSetting = await _settingService.LoadSettingAsync<DeviceSettings>();
      var configTicks = deviceSetting.ModifiedTicks;
      var dateTimeConfig = new DateTime(configTicks);
      chunk += $"Current configuration time: {dateTimeConfig:yyyy:MM:dd HH:mm:ss.FFFFFFF}. ";

      var dataCount = _dbContext.SensorRecords.Count();
      chunk += $"Data rows in the database: {dataCount}. ";

      var sentRecords = _dbContext.SensorRecords.Where(x => x.IsSent).Count();
      var unsentRecords = dataCount - sentRecords;
      chunk += $"Of these, {sentRecords} have been sent and {unsentRecords} are in the queue. ";

      var now = DateTime.UtcNow;
      var nowTicks = now.Ticks;
      var ticksMinuteAgo = now.AddMinutes(-1).Ticks;

      var dataLastMinute = _dbContext.SensorRecords.Where(x => x.EventTimestamp <= nowTicks && x.EventTimestamp >= ticksMinuteAgo).Count();
      chunk += $"Data flow is {dataLastMinute} records per (last) minute.";
      _p2p.AddServerNotification(new ClientMsg()
      {
         Receiver = sender,
         CommandResponse = new() { Notification = chunk },
      });
   }

   private Task NotifyExecuting(Command command, string sender)
   {
      _p2p.AddServerNotification(new ClientMsg()
      {
         Receiver = sender,
         CommandResponse = new() { Notification = $"Ok. I've been notified by {sender}." },
      });
      return Task.CompletedTask;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Executes a notify
   /// </summary>
   /// <param name="command"></param>
   /// <param name="sender">Server message sender</param>
   /// <returns></returns>
   private Task NotifyNotimplemented(Command command, string sender)
   {
      _p2p.AddServerNotification(new ClientMsg()
      {
         Receiver = sender,
         CommandResponse = new() { Notification = $"Command '{(CommandEnum)command.CommandId}' is not implemented into the device software or you work as a demouser." },
      });
      return Task.CompletedTask;
   }

   #endregion
}
