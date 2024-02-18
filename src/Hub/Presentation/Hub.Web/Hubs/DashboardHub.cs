using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Data;
using Hub.Services.Clients;
using Hub.Services.Clients.Devices;
using Hub.Services.Clients.Sensors;
using Hub.Services.Users;
using Hub.Web.Framework.Hubs;
using Microsoft.AspNetCore.Authorization;
using Shared.Clients.Configuration;
using Shared.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Hubs;

public class DashboardHub : BaseHub
{
   #region fileds

   private readonly IWorkContext _workContext;
   private readonly ICommunicator _communicator;
   private readonly IDeviceService _deviceService;
   private readonly ISensorService _sensorService;
   private readonly IUserService _userService;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DashboardHub(IWorkContext workContext,
      ICommunicator communicator,
      IUserService userService,
      ISensorService sensorService,
      IDeviceService deviceService)
   {
      _workContext = workContext;
      _communicator = communicator;
      _deviceService = deviceService;
      _userService = userService;
      _sensorService = sensorService;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Adds a client to the groupname (by the signalr connection id)
   /// </summary>
   /// <param name="groupName">Group name</param>
   /// <returns></returns>
   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public virtual async Task SubscibeToGroups(IEnumerable<string> groupNames)
   {
      await _communicator.AddClientToGroupsAsync(Context.ConnectionId, groupNames);
   }

   /// <summary>
   /// Removes a client to the groupname (by the signalr connection id)
   /// </summary>
   /// <param name="groupName">Group name</param>
   /// <returns></returns>
   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public virtual async Task UnsubscibeFromGroups(IEnumerable<string> groupNames)
   {
      await _communicator.RemoveClientFromGroupsAsync(Context.ConnectionId, groupNames);
   }

   /// <summary>
   /// Subscribes to device online status notification
   /// </summary>
   /// <param name="deviceIds">Device identifier collection</param>
   /// <returns></returns>
   public virtual async Task SubscribeToDeviceStatus(IEnumerable<long> deviceIds)
   {
      var user = await _workContext.GetCurrentUserAsync();

      // secutirty: check allowed user's devices
      var ownDevices = await _deviceService.GetOwnDevicesAsync(new() { UserId = user.Id });
      var sharedDevices = await _deviceService.GetSharedDevicesAsync(new() { UserId = user.Id });
      var allowedDevices = ownDevices.UnionBy(sharedDevices, d => d.Id);
      allowedDevices = allowedDevices.IntersectBy(deviceIds, x => x.Id);

      // add subscriptions
      var groups = allowedDevices.Select(x => $"{nameof(Device)}_{x.Id}");
      await _communicator.AddClientToGroupsAsync(Context.ConnectionId, groups);
   }

   /// <summary>
   /// Unsubscribes from device online status notification
   /// </summary>
   /// <param name="deviceIds">Device identifier collection</param>
   /// <returns></returns>
   public virtual async Task UnsubscribeFromDeviceStatus(IEnumerable<long> deviceIds)
   {
      // remove subscriptions
      var groups = deviceIds.Select(x => $"{nameof(Device)}_{x}");
      await _communicator.RemoveClientFromGroupsAsync(Context.ConnectionId, groups);
   }

   /// <summary>
   /// Sends a command to a device
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <param name="command">Command</param>
   /// <returns></returns>
   public virtual async Task DeviceCommand(long deviceId, CommandEnum command)
   {
      var user = await _workContext.GetCurrentUserAsync();

      // security
      if (!await _userService.IsAdminAsync(user))
      {
         var devices = await _deviceService.GetAllDevicesAsync(new() { UserId = user.Id });
         if (!devices.Select(x => x.Id).Contains(deviceId))
            return;
      }

      await _communicator.AddMessageToDeviceQueueAsync(deviceId, () => new() { Sender = Context.ConnectionId, Command = new() { CommandId = (int)command } });
   }

   /// <summary>
   /// Subscribes the current user to common log messages 
   /// </summary>
   /// <returns></returns>
   public virtual async Task SubscribeToCommonLog()
   {
      var user = await _workContext.GetCurrentUserAsync();

      var sensorIds = await _sensorService.GetCommonLogSensorIdsAsync(user.Id);
     
      //if (!sensorIds.Any())
      //   return;

      await _communicator.AddClientToGroupsAsync(Context.ConnectionId, sensorIds.Select(x => $"{nameof(Sensor)}_{x}"));

      var devices = await _deviceService.GetAllDevicesAsync(new() { UserId = user.Id });

      //if (devices.Any())
      //   return;

      await _communicator.AddClientToGroupsAsync(Context.ConnectionId, devices.Select(x => $"{nameof(Device)}_{x.Id}"));

   }

   /// <summary>
   /// Unsubscribes the current user from common log messages 
   /// </summary>
   /// <returns></returns>
   public virtual async Task UnsubscribeFromCommonLog()
   {
      var user = await _workContext?.GetCurrentUserAsync();

      var sensorIds = await _sensorService.GetCommonLogSensorIdsAsync(user.Id);
     
      //if (!sensorIds.Any())
      //   return;
      
      await _communicator.RemoveClientFromGroupsAsync(Context.ConnectionId, sensorIds.Select(x => $"{nameof(Sensor)}_{x}"));

      var devices = await _deviceService.GetAllDevicesAsync(new() { UserId = user.Id });

      //if (devices.Any())
      //   return;

      await _communicator.RemoveClientFromGroupsAsync(Context.ConnectionId, devices.Select(x => $"{nameof(Device)}_{x.Id}"));
   }

   /// <summary>
   /// Subscribes to a sensor data stream
   /// </summary>
   /// <param name="sensorIds">Sensor identifiers</param>
   /// <returns></returns>
   public virtual async Task SubscribeToSensorDataStream(IEnumerable<long> sensorIds)
   {
      if (sensorIds?.Any() != true)
         return;

      var user = await _workContext.GetCurrentUserAsync();
      var sensors = await _sensorService.GetSensorsAsync(new()
      {
         UserId = await _userService.IsAdminAsync(user) ? null : user.Id,
         SensorIds = sensorIds
      });

      await _communicator.AddClientToGroupsAsync(Context.ConnectionId, sensors.Select(x => $"{nameof(Sensor)}_{x.Id}"));
   }

   /// <summary>
   /// Unsubscribes from a sensor data stream
   /// </summary>
   /// <param name="sensorIds">Sensor identifiers</param>
   /// <returns></returns>
   public virtual async Task UnsubscribeFromSensorDataStream(IEnumerable<long> sensorIds)
   {
      if (sensorIds?.Any() != true)
         return;

      var user = await _workContext.GetCurrentUserAsync();
      var sensors = await _sensorService.GetSensorsAsync(new() 
      { 
         UserId = await _userService.IsAdminAsync(user) ? null : user.Id,
         SensorIds = sensorIds
      });

      await _communicator.RemoveClientFromGroupsAsync(Context.ConnectionId, sensors.Select(x => $"{nameof(Sensor)}_{x.Id}"));
   }
   #endregion
}