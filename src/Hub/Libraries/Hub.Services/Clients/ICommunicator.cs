using Grpc.Core;
using Hub.Core.Domain.Clients;
using Microsoft.AspNetCore.SignalR;
using Shared.Clients.SignalR;
using Shared.Common;
using Shared.Devices.Proto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Clients;

/// <summary>
/// Represents a communication service interface
/// </summary>
public interface ICommunicator
{
   /// <summary>
   /// Gets client users that connected to hub 
   /// </summary>
   /// <returns>Online user identifiers that connected via clients right now</returns>
   Task<IList<long>> GetOnlineUserIdsAsync();

   /// <summary>
   /// Gets devices that connected to hub
   /// </summary>
   /// <returns>Online device identifiers that connected right now</returns>
   Task<IList<long>> GetOnlineDeviceIdsAsync();

   /// <summary>
   /// Gets all connection contexts for a user
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>Connection conntext collection for he pointed user</returns>
   Task<IList<ClientConnectionInfo>> GetUserConnectionsInfoAsync(long userId);

   /// <summary>
   /// Gets client connection info by signalr connection identifier
   /// </summary>
   /// <param name="connectionId">Signalr connection identifier</param>
   /// <returns>User identifier</returns>
   Task<ClientConnectionInfo> GetConnectionInfoByConnectionIdAsync(string connectionId);

   /// <summary>
   /// Gets connection info colection by the group name
   /// </summary>
   /// <param name="groupName">Group name</param>
   /// <returns>Client connection info collection</returns>
   Task<IList<ClientConnectionInfo>> GetConnectionInfoByGroupNameAsync(string groupName);

   /// <summary>
   /// Gets connection info colection by group names
   /// </summary>
   /// <param name="groupNames">Group names</param>
   /// <returns>Client connection info collection</returns>
   Task<IList<ClientConnectionInfo>> GetConnectionInfoByGroupNamesAsync(IEnumerable<string> groupNames);

   /// <summary>
   /// Gets connection info colection by the template string collection
   /// </summary>
   /// <param name="templates">Template string collection</param>
   /// <returns>Client connection info collection</returns>
   Task<IList<ClientConnectionInfo>> GetConnectionInfoByTemplatesAsync(IEnumerable<string> templates);

   /// <summary>
   /// Registers user connection
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="context">Connection context</param>
   Task RegisterClientConnectionAsync(long userId, HubLifetimeContext context);

   /// <summary>
   /// Unregisters user connection
   /// </summary>
   /// <param name="connectionId">Connection identifier</param>
   Task UnregisterUserConnectionAsync(string connectionId);

   /// <summary>
   /// Clear user connections
   /// </summary>
   /// <param name="userId">User identifier</param>
   Task ClearUserConnectionsAsync(long userId);

   /// <summary>
   /// Gets subscribed groups for user by the user identifier
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>Group name collection</returns>
   Task<IList<string>> GetUserGroupsAsync(long userId);

   /// <summary>
   /// Adds groups for the user connection identifier
   /// </summary>
   /// <param name="connectionId">Connection identifier</param>
   /// <param name="groups">Group name collection</param>
   Task AddClientToGroupsAsync(string connectionId, IEnumerable<string> groups);

   /// <summary>
   /// Removes groups for the user connection identifier
   /// </summary>
   /// <param name="connectionId">Connection identifier</param>
   /// <param name="groups">Group name collection</param>
   Task RemoveClientFromGroupsAsync(string connectionId, IEnumerable<string> groups);

   /// <summary>
   /// Adds groups for the user (and all connection identifiers)
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="groups">Group name collection</param>
   Task AddUserToGroupsAsync(long userId, IEnumerable<string> groups);

   /// <summary>
   /// Removes groups for the user (and all connection identifiers)
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="groups">Group name collection</param>
   Task RemoveUserFromGroupsAsync(long userId, IEnumerable<string> groups);

   /// <summary>
   /// Device status changed notification
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <param name="status">Status object</param>
   /// <returns></returns>
   Task DeviceStatusChangedAsync(long deviceId, DeviceConnectionStatus status);

   /// <summary>
   /// Registers (or overrides) a point-to-point channel
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <param name="context">Server call context</param>
   /// <param name="queueVolume">Volume of the message queue</param>
   Task RegisterDeviceChannelAsync(long deviceId, ServerCallContext context, int queueVolume = 1_000);

   /// <summary>
   /// Gets server call context
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   /// <exception cref="RpcException">P2P Channel for device is not registered</exception>
   Task<ServerCallContext> GetDeviceCallContextAsync(long deviceId);

   /// <summary>
   /// Gets message to sent to a device by identifier
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>Function that return the message</returns>
   Task<Func<ServerMsg>> GetMessageForDeviceAsync(long deviceId);

   /// <summary>
   /// Adds message for a device into the queue
   /// </summary>
   /// <param name="deviceId">device identifier</param>
   /// <param name="message">Message function</param>
   /// <returns></returns>
   Task AddMessageToDeviceQueueAsync(long deviceId, Func<ServerMsg> message);

   /// <summary>
   /// Stop channel for a device by the device identifier
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns></returns>
   Task UnregisterDeviceChannelAsync(long deviceId);

   /// <summary>
   /// Sends a notification to user group
   /// </summary>
   /// <param name="groupName">User group name</param>
   /// <param name="method">Connection method</param>
   /// <param name="message">message</param>
   /// <returns></returns>
   Task ClientsNotifyAsync(string groupName, string method, object message);

   /// <summary>
   /// Sends a notification to user group
   /// </summary>
   /// <param name="connectId">User connection identifier</param>
   /// <param name="entityId">Entity identifier</param>
   /// <param name="method">Connection method</param>
   /// <param name="message">message</param>
   /// <returns></returns>
   Task ClientNotifyAsync(string connectId, string method, long entityId, object message);

   /// <summary>
   /// Sends a notification to user group
   /// </summary>
   /// <param name="entity">Entity for notification</param>
   /// <param name="method">Connection method</param>
   /// <param name="message">message</param>
   /// <returns></returns>
   Task ClientsNotifyAsync<TEntity>(TEntity entity, string method, object message) where TEntity : BaseEntity;

   /// <summary>
   /// Sends a sensor data records
   /// </summary>
   /// <param name="records">Sensor data record collection</param>
   /// <returns></returns>
   Task SensorDataFlowAsync(IList<SensorRecord> records);

   /// <summary>
   /// Sends a new video segments
   /// </summary>
   /// <param name="segment">Video segment</param>
   /// <returns></returns>
   Task SensorDataFlowAsync(VideoSegment segment);
}