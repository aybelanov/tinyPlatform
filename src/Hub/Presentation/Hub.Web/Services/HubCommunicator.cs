using Grpc.Core;
using Hub.Core.Domain.Clients;
using Hub.Services.Clients;
using Hub.Services.Logging;
using Hub.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Shared.Clients.SignalR;
using Shared.Common;
using Shared.Devices.Proto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hub.Web.Services;

/// <summary>
/// Communicator service implementation
/// </summary>
public class HubCommunicator : ICommunicator
{
   #region fields

   private readonly IHubContext<DashboardHub> _hubContext;
   private readonly IServiceScopeFactory _serviceScopeFactory;

   private readonly ConcurrentDictionary<long, P2PQueue> _deviceQueues;
   private readonly ConcurrentDictionary<string, ClientConnectionInfo> _userConnections;
   private readonly ConcurrentDictionary<long, long> _lastTimestampsSensorRecords;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public HubCommunicator(IHubContext<DashboardHub> hubContext, IServiceScopeFactory serviceScopeFactory)
   {
      _hubContext = hubContext;
      _serviceScopeFactory = serviceScopeFactory;
      _deviceQueues = new();
      _userConnections = new();
      _lastTimestampsSensorRecords = new();
   }

   #endregion

   #region Methods

   /// <summary>
   /// Gets client users that connected to hub 
   /// </summary>
   /// <returns>Online user identifiers that connected via clients right now</returns>
   public Task<IList<long>> GetOnlineUserIdsAsync()
   {
      var userIds = _userConnections.Select(x => x.Value.UserId).Distinct();
      return Task.FromResult<IList<long>>(userIds.ToList());
   }

   /// <summary>
   /// Gets devices that connected to hub
   /// </summary>
   /// <returns>Online device identifiers that connected right now</returns>
   public Task<IList<long>> GetOnlineDeviceIdsAsync()
   {
      var deviceIds = _deviceQueues.Select(x => x.Key);
      return Task.FromResult<IList<long>>(deviceIds.ToList());
   }

   /// <summary>
   /// Gets all connection contexts for a user
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>Connection conntext collection for he pointed user</returns>
   public Task<IList<ClientConnectionInfo>> GetUserConnectionsInfoAsync(long userId)
   {
      var info = _userConnections.Select(x => x.Value).Where(x=>x.UserId == userId).ToList();  
      return Task.FromResult<IList<ClientConnectionInfo>>(info);
   }

   /// <summary>
   /// Gets client connection info by signalr connection identifier
   /// </summary>
   /// <param name="connectionId">Signalr connection identifier</param>
   /// <returns>User identifier</returns>
   public Task<ClientConnectionInfo> GetConnectionInfoByConnectionIdAsync(string connectionId)
   {
      _userConnections.TryGetValue(connectionId, out var connectionInfo);
      return Task.FromResult(connectionInfo);
   }

   /// <summary>
   /// Gets connection info colection by the group name
   /// </summary>
   /// <param name="groupName">Group name</param>
   /// <returns>Client connection info collection</returns>
   public Task<IList<ClientConnectionInfo>> GetConnectionInfoByGroupNameAsync(string groupName)
   {
      var info = _userConnections.Select(x=>x.Value).Where(x=>x.SubcribedGroups.Contains(groupName)).ToList();
      return Task.FromResult<IList<ClientConnectionInfo>>(info);
   }

   /// <summary>
   /// Gets connection info colection by group names
   /// </summary>
   /// <param name="groupNames">Group names</param>
   /// <returns>Client connection info collection</returns>
   public Task<IList<ClientConnectionInfo>> GetConnectionInfoByGroupNamesAsync(IEnumerable<string> groupNames)
   {
      var info = _userConnections.Select(x => x.Value).Where(x => groupNames.Any(n => x.SubcribedGroups.Contains(n))).DistinctBy(x=>x.ConnectionId).ToList();
      return Task.FromResult<IList<ClientConnectionInfo>>(info);
   }

   /// <summary>
   /// Gets connection info colection by the template string collection
   /// </summary>
   /// <param name="templates">Template string collection</param>
   /// <returns>Client connection info collection</returns>
   public Task<IList<ClientConnectionInfo>> GetConnectionInfoByTemplatesAsync(IEnumerable<string> templates)
   {
      var info = _userConnections.Select(x => x.Value).Where(x => templates.Any(n => x.SubcribedGroups.Any(z => z.Contains(n)))).DistinctBy(x => x.ConnectionId).ToList();
      return Task.FromResult<IList<ClientConnectionInfo>>(info);
   }

   /// <summary>
   /// Registers user connection
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="context">Connection context</param>
   public Task RegisterClientConnectionAsync(long userId, HubLifetimeContext context)
   {
      _userConnections.GetOrAdd(context.Context.ConnectionId, key => new ClientConnectionInfo(userId, context));
      return Task.CompletedTask;
   }

   /// <summary>
   /// Unregisters user connection
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="context">Unregistering connection contex</param>
   public Task UnregisterUserConnectionAsync(string connectionId)
   {
      _userConnections.TryRemove(connectionId, out _);
      return Task.CompletedTask;
   }

   /// <summary>
   /// Clear user connections
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="context">Unregistering connection contex</param>
   public Task ClearUserConnectionsAsync(long userId)
   {
      var connections = _userConnections.Where(x => x.Value.UserId == userId);
      
      foreach (var connection in connections)
         _userConnections.TryRemove(connection);

      return Task.CompletedTask;
   }

   /// <summary>
   /// Gets subscribed groups for user by the user identifier
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>Group name collection</returns>
   public Task<IList<string>> GetUserGroupsAsync(long userId)
   {
      var groups = _userConnections.Where(x => x.Value.UserId == userId).SelectMany(x => x.Value.SubcribedGroups).Distinct();
      return Task.FromResult<IList<string>>(groups.ToList()); 
   }

   /// <summary>
   /// Adds groups for the user connection identifier
   /// </summary>
   /// <param name="connectionId">Connection identifier</param>
   /// <param name="groups">Group name collection</param>
   public async Task AddClientToGroupsAsync(string connectionId, IEnumerable<string> groups)
   {
      if (_userConnections.TryGetValue(connectionId, out var info))
      {
         foreach (var group in groups)
         {
            await _hubContext.Groups.AddToGroupAsync(connectionId, group); 
            info.SubcribedGroups.Add(group);
         }
      }
   }

   /// <summary>
   /// Removes groups for the user connection identifier
   /// </summary>
   /// <param name="connectionId">Connection identifier</param>
   /// <param name="groups">Group name collection</param>
   public async Task RemoveClientFromGroupsAsync(string connectionId, IEnumerable<string> groups)
   {
      if (_userConnections.TryGetValue(connectionId, out var info))
      {
         foreach (var group in groups)
         {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, group);
            info.SubcribedGroups.Remove(group);
         }
      }
   }

   /// <summary>
   /// Adds groups for the user (and all connection identifiers)
   /// </summary>
   /// <param name="connectionId">Connection identifier</param>
   /// <param name="groups">Group name collection</param>
   public async Task AddUserToGroupsAsync(long userId, IEnumerable<string> groups)
   {
      var connections = _userConnections.Where(x => x.Value.UserId == userId);
      foreach (var connection in connections)
         await AddClientToGroupsAsync(connection.Key, groups);
   }

   /// <summary>
   /// Removes groups for the user (and all connection identifiers)
   /// </summary>
   /// <param name="connectionId">Connection identifier</param>
   /// <param name="groups">Group name collection</param>
   public async Task RemoveUserFromGroupsAsync(long userId, IEnumerable<string> groups)
   {
      var connections = _userConnections.Where(x => x.Value.UserId == userId);
      foreach (var connection in connections)
         await RemoveClientFromGroupsAsync(connection.Key, groups);
   }

   /// <summary>
   /// Device status changed notification
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <param name="status">Status object</param>
   /// <returns></returns>
   public async Task DeviceStatusChangedAsync(long deviceId, DeviceConnectionStatus status)
   {
      await _hubContext.Clients
         .Group($"{nameof(Device)}_{deviceId}")
         .SendAsync(SignalRDefaults.DeviceStatusChanged, status);
   }

   /// <summary>
   /// Registers (or overrides) a point-to-point channel
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <param name="context">Server call context</param>
   /// <param name="queueVolume">Volume of the message queue</param>
   public Task RegisterDeviceChannelAsync(long deviceId, ServerCallContext context, int queueVolume = 1_000)
   {
      _deviceQueues.AddOrUpdate(deviceId, new P2PQueue(context, queueVolume), (key, value) => new P2PQueue(context, queueVolume));
      return Task.CompletedTask;
   }

   /// <summary>
   /// Gets server call context
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   /// <exception cref="RpcException">P2P Channel for device is not registered</exception>
   public Task<ServerCallContext> GetDeviceCallContextAsync(long deviceId)
   {
      _deviceQueues.TryGetValue(deviceId, out var queue);
      var result = queue?.CallContext;
      return Task.FromResult(result);
   }

   /// <summary>
   /// Gets message to sent to a device by identifier
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Function that return the message</returns>
   public Task<Func<ServerMsg>> GetMessageForDeviceAsync(long deviceId)
   {
      if (!_deviceQueues.TryGetValue(deviceId, out var queue))
         throw new RpcException(new Status(StatusCode.Aborted, $"P2P Channel for device {deviceId} is not registered"));

      var result = queue.GetMessage();
      return result;
   }

   /// <summary>
   /// Adds message for a device into the queue
   /// </summary>
   /// <param name="deviceId">device identifier</param>
   /// <param name="message">Message function</param>
   /// <returns></returns>
   public Task AddMessageToDeviceQueueAsync(long deviceId, Func<ServerMsg> message)
   {
      if (!_deviceQueues.TryGetValue(deviceId, out var queue))
         throw new RpcException(new Status(StatusCode.Aborted, $"P2P Channel for device {deviceId} is not registered"));

      queue.AddMessage(message);
      return Task.CompletedTask;
   }

   /// <summary>
   /// Stop channel for a device by the device identifier
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   public Task UnregisterDeviceChannelAsync(long deviceId)
   {
      if (_deviceQueues.TryGetValue(deviceId, out var queue))
      {
         queue.Stop();
         queue.Clear();
         _deviceQueues.TryRemove(deviceId, out _);
      }

      return Task.CompletedTask;
   }

   /// <summary>
   /// Sends a notification to user group
   /// </summary>
   /// <param name="groupName">User group name</param>
   /// <param name="method">Connection method</param>
   /// <param name="message">message</param>
   /// <returns></returns>
   public async Task ClientsNotifyAsync(string groupName, string method, object message)
   {
      await _hubContext.Clients.Groups(groupName).SendAsync(method, message);
   }

   /// <summary>
   /// Sends a notification to user group
   /// </summary>
   /// <param name="connectId">User cinnect identifier</param>
   /// <param name="method">Connection method</param>
   /// <param name="message">message</param>
   /// <param name="entityId">Entity identifier</param>
   /// <returns></returns>
   public async Task ClientNotifyAsync(string connectId, string method, long entityId, object message)
   {
      await _hubContext.Clients.Client(connectId).SendAsync(method, entityId, message);
   }

   /// <summary>
   /// Sends a notification to user group
   /// </summary>
   /// <param name="entity">Entity for notification</param>
   /// <param name="method">Connection method</param>
   /// <param name="message">message</param>
   /// <returns></returns>
   public async Task ClientsNotifyAsync<TEntity>(TEntity entity, string method, object message) where TEntity : BaseEntity
   {
      var groupName = $"{entity.GetType().Name}_{entity.Id}";
      await ClientsNotifyAsync(groupName, method, message);
   }

   /// <summary>
   /// Sends a sensor data records
   /// </summary>
   /// <param name="records">Sensor data record collection</param>
   /// <returns></returns>
   public async Task SensorDataFlowAsync(IList<SensorRecord> records)
   {
      if (!records?.Any() ?? true)
         return;

      // send data with created date not before 5 minutes
      var notEarlierTicks = DateTime.UtcNow.AddMinutes(-5).Ticks;

      var recordsGroupsBySensors =
         from r in records
         where r.EventTimestamp > _lastTimestampsSensorRecords.GetOrAdd(r.SensorId, ts => ts > notEarlierTicks ? ts : notEarlierTicks)
         orderby r.EventTimestamp descending
         group r by r.SensorId;

      foreach (var sensorRecords in recordsGroupsBySensors)
      {
         try
         {
            await ClientsNotifyAsync(new Sensor() { Id = sensorRecords.Key }, SignalRDefaults.SensorDataMethod, sensorRecords.ToList());
            var lastrecord = sensorRecords.First();
            _lastTimestampsSensorRecords.AddOrUpdate(lastrecord.SensorId, lastrecord.EventTimestamp, (key, val) => lastrecord.EventTimestamp > val ? lastrecord.EventTimestamp : val);
         }
         catch (Exception ex)
         {
            using var scope = _serviceScopeFactory.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
            await logger.WarningAsync("Sensor data sending is failed", ex);
         }
      }
   }

   /// <summary>
   /// Sends a new video segments
   /// </summary>
   /// <param name="segment">Video segment</param>
   /// <returns></returns>
   public async Task SensorDataFlowAsync(VideoSegment segment)
   {
      if (segment is null)
         return;

      // send data with created date not before 5 minutes
      if (segment.OnCreatedUtc < DateTime.UtcNow.AddMinutes(-5))
         return;

      var record = new SensorRecord()
      {
         CreatedTimeOnUtc = segment.OnReceivedUtc,
         Metadata = JsonSerializer.Serialize(new
         {
            FileName = segment.Guid.ToString("N"),
            segment.Extinf,
            segment.Resolution
         }),
         SensorId = segment.IpcamId,
         EventTimestamp = segment.OnCreatedUtc.Ticks,
         Id = segment.Id,
      };

      try
      {
         await ClientsNotifyAsync(new Sensor() { Id = segment.IpcamId }, SignalRDefaults.SensorDataMethod, new List<SensorRecord>() { record });
      }
      catch (Exception ex)
      {
         using var scope = _serviceScopeFactory.CreateScope();
         var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
         await logger.WarningAsync("Sensor data sending is failed", ex);
      }
      
   }

   #endregion
}