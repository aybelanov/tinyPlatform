using Clients.Dash.Domain;
using Clients.Dash.Services.ErrorServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Shared.Clients.SignalR;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Clients.Dash.Services.Connection;

/// <summary>
/// Represents a hub connection service
/// </summary>
public class HubService : IDisposable, IAsyncDisposable
{
   #region fields

   private readonly IServiceScopeFactory _scopeFactory;
   private readonly ILogger<HubService> _logger;
   private readonly IJSRuntime _js;
   private readonly NavigationManager _navigationManager;
   private readonly HubConnection _hubConnection;

   private readonly DotNetObjectReference<HubService> _hubRef;

   private static TaskCompletionSource<string> _readyTaskCompletionSource = new();
   private static readonly object _lock = new();
   private static readonly SemaphoreSlim _semaphore = new(1, 1);

   #endregion

   #region Events

   /// <summary>
   /// If the service is connecting
   /// </summary>
   public event Func<Task> Connecting;

   /// <summary>
   /// If the service has connected
   /// </summary>
   public event Func<string, Task> Connected;

   /// <summary>
   /// If the service has disconnceted
   /// </summary>
   public event Func<Exception, Task> Disconnected;

   /// <summary>
   /// If the device connected status has changed
   /// </summary>
   public event Func<DeviceConnectionStatus, Task> DeviceStatusChanged;

   /// <summary>
   /// If the sensor data has received
   /// </summary>
   public event Func<IEnumerable<SensorRecord>, Task> SensorDataReceived;

   /// <summary>
   /// If the entered user has some messsage received
   /// </summary>
   public event Func<int, string, Task> ClientMessageReceived;

   /// <summary>
   /// Download task status changed message event
   /// </summary>
   public event Func<DownloadTask, Task> DownloadTaskStatusChanged;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public HubService(ILogger<HubService> logger,
      IJSRuntime js,
      NavigationManager navigationManager,
      HubConnection hubConnection,
      IServiceScopeFactory scopeFactory)
   {
      _scopeFactory = scopeFactory;
      _hubConnection = hubConnection;
      _logger = logger;
      _navigationManager = navigationManager;
      _logger = logger;
      _js = js;

      // events
      _hubConnection.Reconnecting += OnConnecting;
      _hubConnection.Reconnected += OnConnected;
      _hubConnection.Closed += OnClosed;
      _hubConnection.On<DeviceConnectionStatus>(SignalRDefaults.DeviceStatusChanged, OnDeviceStatusChanged);
      _hubConnection.On<IEnumerable<SensorRecord>>(SignalRDefaults.SensorDataMethod, OnSensorDataReceived);
      _hubConnection.On<DownloadTask>(SignalRDefaults.DownloadTaskStatusChanged, OnDownloadTaskStatusChanged);
      _hubConnection.On<int, string>(SignalRDefaults.ClientMessageMethod, OnClientMessageReceived);

      //hubRef = DotNetObjectReference.Create(this);
      //((IJSInProcessRuntime)_js).InvokeVoid("setHub", hubRef);
   }


   #endregion

   #region Utilities

   /// <summary>
   /// Sarts the connection process
   /// </summary>
   /// <returns></returns>
   private async Task StartAsync()
   {
      await _semaphore.WaitAsync();

      try
      {
         if (Connecting != null)
            await Connecting.Invoke();

         await _hubConnection.StartAsync();

         if (Connected != null)
            await Connected.Invoke(_hubConnection.ConnectionId);

         SetConnectedStatuses(_hubConnection.ConnectionId);
      }
      finally
      {
         _semaphore.Release();
      }

   }

   #endregion

   #region Event handlers


   private async Task OnConnecting(Exception exception)
   {
      if (Connecting != null)
         await Connecting.Invoke();

      _logger.LogInformation(exception, "Hub is recconecting");
   }

   /// <summary>
   /// Handles the conncetion connectred/reconnectd
   /// </summary>
   /// <param name="connectionId">Connection identifier</param>
   /// <returns></returns>
   private async Task OnConnected(string connectionId)
   {
      if (Connecting != null)
         await Connected.Invoke(connectionId);

      SetConnectedStatuses(_hubConnection.ConnectionId);

      _logger.LogInformation(null, "Hub is connected");
   }

   /// <summary>
   /// Handles the conncetion closing
   /// </summary>
   /// <param name="exception">Exception that called the conncetion closing</param>
   /// <returns></returns>
   private async Task OnClosed(Exception exception)
   {
      if (Disconnected != null)
         await Disconnected.Invoke(exception);
   }

   /// <summary>
   /// Handles the device status changing
   /// </summary>
   /// <param name="status">Device connection status</param>
   /// <returns></returns>
   private Task OnDeviceStatusChanged(DeviceConnectionStatus status)
   {
      DeviceStatusChanged?.Invoke(status);
      return Task.CompletedTask;
   }

   /// <summary>
   /// Handles the sensor data receiving
   /// </summary>
   /// <param name="records">Sensor data records</param>
   /// <returns></returns>
   private Task OnSensorDataReceived(IEnumerable<SensorRecord> records)
   {
      SensorDataReceived?.Invoke(records);
      return Task.CompletedTask;
   }

   /// <summary>
   /// Handles the message for teh device
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <param name="message">Message</param>
   /// <returns></returns>
   private Task OnClientMessageReceived(int deviceId, string message)
   {
      ClientMessageReceived?.Invoke(deviceId, message);
      return Task.CompletedTask;
   }

   /// <summary>
   /// Handles the message of a download task status has changed
   /// </summary>
   /// <param name="task"></param>
   /// <returns></returns>
   private async Task OnDownloadTaskStatusChanged(DownloadTask task)
   {
      if (DownloadTaskStatusChanged != null)
         await DownloadTaskStatusChanged.Invoke(task);
   }

   #endregion

   #region Methods

   /// <summary>
   /// Gets/waits hub's connection identifier
   /// </summary>
   /// <returns></returns>
   public Task<string> GetConnectionIdAsync()
   {
      lock (_lock)
      {
         if (_readyTaskCompletionSource.Task.IsCompleted)
            _readyTaskCompletionSource = new();

         if (_hubConnection.State == HubConnectionState.Connected)
            _readyTaskCompletionSource.SetResult(_hubConnection.ConnectionId);

         return _readyTaskCompletionSource.Task;
      }
   }

   private static void SetConnectedStatuses(string connectionId)
   {
      lock (_lock)
      {
         _readyTaskCompletionSource.SetResult(connectionId);
      }
   }

   /// <summary>
   /// Tries the start to connect to the server hub
   /// </summary>
   /// <returns>Result true - connected, false disconnected</returns>
   [JSInvokable]
   public async Task<bool> TryStartAsync()
   {
      try
      {
         if (_hubConnection.State == HubConnectionState.Disconnected)
         {
            await StartAsync();
            return true;
         }
      }
      catch (AggregateException agrEx) when (agrEx.InnerException is TransportFailedException failEx)
      {
         _navigationManager.NavigateTo("connection-access-denied");
      }
      catch (Exception ex)
      {
         _logger.LogWarning(ex, "Cannot start hubconnection.");
      }

      return false;
   }

   /// <summary>
   /// Stops a connection to the server.
   /// </summary>
   /// <returns></returns>
   [JSInvokable]
   public async Task StopAsync()
   {
      try
      {
         await _hubConnection.StopAsync();
      }
      catch { }
   }

   /// <summary>
   /// Subscribes to groups
   /// </summary>
   /// <param name="sender">Sensde</param>
   /// <param name="groups">Group collection</param>
   /// <returns></returns>
   public async Task SubscribeToGroupsAsync(object sender, IEnumerable<string> groups)
   {
      await _readyTaskCompletionSource.Task;

      try
      {
         if (ConnectionState != HubConnectionState.Connected)
            throw new Exception($"Hub is in {ConnectionState} state, not connected.");

         await _hubConnection.SendAsync(SignalRDefaults.SubscibeToGroups, groups.ToList());
      }
      catch (Exception ex)
      {
         using var scope = _scopeFactory.CreateScope();
         var errorService = scope.ServiceProvider.GetRequiredService<ErrorService>();
         await errorService.HandleError(sender, ex, $"Cannot subcribes to group for {sender}: {ex.Message}");
      }
   }


   /// <summary>
   /// Unsubscribes to groups
   /// </summary>
   /// <param name="sender">Sensde</param>
   /// <param name="groups">Group collection</param>
   /// <returns></returns>
   public async Task UnsubscribeFromGroupsAsync(object sender, IEnumerable<string> groups)
   {
      try
      {
         if (ConnectionState != HubConnectionState.Connected)
            throw new Exception($"Hub is in {ConnectionState} state, not connected.");

         await _hubConnection.SendAsync(SignalRDefaults.SubscibeToGroups, groups.ToList());
      }
      catch (Exception ex)
      {
         using var scope = _scopeFactory.CreateScope();
         var errorService = scope.ServiceProvider.GetRequiredService<ErrorService>();
         await errorService.HandleError(sender, ex, $"Cannot unsubcribes to group for {sender}: {ex.Message}");
      }
   }

   /// <summary>
   /// Subscribes the application to the groups
   /// </summary>
   /// <param name="groups"></param>
   /// <param name="sender">Susbscribed subject</param>
   /// <returns></returns>
   public async Task SubscribeToDeviceStatus(object sender, IEnumerable<long> groups)
   {
      await _readyTaskCompletionSource.Task;

      try
      {
         if (ConnectionState != HubConnectionState.Connected)
            throw new Exception($"Hub is in {ConnectionState} state, not connected.");

         await _hubConnection.SendAsync(SignalRDefaults.SubscribeToDeviceStatus, groups.ToList());
      }
      catch (Exception ex)
      {
         using var scope = _scopeFactory.CreateScope();
         var errorService = scope.ServiceProvider.GetRequiredService<ErrorService>();
         await errorService.HandleError(sender, ex, $"Cannot subcribes to group for {sender}: {ex.Message}");
      }
   }

   /// <summary>
   /// Unsubscribes the application from the groups
   /// </summary>
   /// <param name="groups"></param>
   /// <param name="sender">Susbscribed subject</param>
   /// <returns></returns>
   public async Task UnsubscribeFromDeviceStatus(object sender, IEnumerable<long> groups)
   {
      try
      {
         if (ConnectionState != HubConnectionState.Connected)
            throw new Exception($"Hub is in {ConnectionState} state, not coonected.");

         await _hubConnection.SendAsync(SignalRDefaults.UnsubscribeFromDeviceStatus, groups.ToList());
      }
      catch (Exception ex)
      {
         using var scope = _scopeFactory.CreateScope();
         var errorService = scope.ServiceProvider.GetRequiredService<ErrorService>();
         await errorService.HandleError(sender, ex, $"Cannot subcribes to group for {sender}: {ex.Message}");
      }
   }

   /// <summary>
   /// Subscribes to common loge messages
   /// </summary>
   /// <param name="sender">Subcriber</param>
   /// <returns></returns>
   public async Task SubscribeToCommonLog(object sender)
   {
      await _readyTaskCompletionSource.Task;

      try
      {
         if (ConnectionState != HubConnectionState.Connected)
            throw new Exception($"Hub is in {ConnectionState} state, not coonected.");

         await _hubConnection.SendAsync(SignalRDefaults.SubscribeToCommonLogMessages);
      }
      catch (Exception ex)
      {
         using var scope = _scopeFactory.CreateScope();
         var errorService = scope.ServiceProvider.GetRequiredService<ErrorService>();
         await errorService.HandleError(sender, ex, $"Cannot subscribe to common log messages for {sender}: {ex.Message}");
      }
   }

   /// <summary>
   /// Subscribes to common loge messages
   /// </summary>
   /// <param name="sender">Unsubscriber</param>
   /// <returns></returns>
   public async Task UnsubscribeFromCommonLog(object sender)
   {
      try
      {
         if (ConnectionState != HubConnectionState.Connected)
            throw new Exception($"Hub is in {ConnectionState} state, not coonected.");

         await _hubConnection.SendAsync(SignalRDefaults.UnsubscribeFromCommonLogMessages);
      }
      catch (Exception ex)
      {
         using var scope = _scopeFactory.CreateScope();
         var errorService = scope.ServiceProvider.GetRequiredService<ErrorService>();
         await errorService.HandleError(sender, ex, $"Cannot unsubcribes to common log for {sender}: {ex.Message}");
      }
   }

   /// <summary>
   /// Creates the new subscription for the sensors
   /// </summary>
   /// <param name="sensorIds">Sensor identifiers</param>
   /// <param name="sender">Sunscriber</param>
   /// <returns></returns>
   public async Task SubscribeToSensorDataStream(object sender, IEnumerable<long> sensorIds)
   {
      await _readyTaskCompletionSource.Task;

      try
      {
         if (ConnectionState != HubConnectionState.Connected)
            throw new Exception($"Hub is in {ConnectionState} state, not coonected.");

         await _hubConnection.SendAsync(SignalRDefaults.SubscribeToSensorDataStream, sensorIds.ToList());
      }
      catch (Exception ex)
      {
         using var scope = _scopeFactory.CreateScope();
         var errorService = scope.ServiceProvider.GetRequiredService<ErrorService>();
         await errorService.HandleError(sender, ex, $"Cannot subscribe to a sensor data stream for {sender}: {ex.Message}");
      }
   }

   /// <summary>
   /// Creates the new subscription for the sensors
   /// </summary>
   /// <param name="sensorIds">Sensor identifiers</param>
   /// <param name="sender">Unsunscriber</param>
   /// <returns></returns>
   public async Task UnsubscribeFromSensorDataStream(object sender, IEnumerable<long> sensorIds)
   {
      try
      {
         if (ConnectionState != HubConnectionState.Connected)
            throw new Exception($"Hub is in {ConnectionState} state, not coonected.");

         await _hubConnection.SendAsync(SignalRDefaults.UnsubscribeFromSensorDataStream, sensorIds.ToList());
      }
      catch (Exception ex)
      {
         using var scope = _scopeFactory.CreateScope();
         var errorService = scope.ServiceProvider.GetRequiredService<ErrorService>();
         await errorService.HandleError(sender, ex, $"Cannot unsubscribe from sensor data stream for {sender}: {ex.Message}");
      }
   }

   /// <summary>
   /// Sends the command to the device via the server
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <param name="command">Command</param>
   /// <returns></returns>
   public async Task SendCommandToDevice(long deviceId, CommandEnum command)
   {
      try
      {
         await _hubConnection.SendAsync(SignalRDefaults.DeviceCommandMethod, deviceId, command);
      }
      catch (Exception ex)
      {
         _logger.LogWarning(ex, "Cannot send command to device.");
      }
   }

#pragma warning disable CS1591

   public void Dispose()
   {
      _hubConnection.Reconnecting -= OnConnecting;
      _hubConnection.Reconnected -= OnConnected;
      _hubConnection.Closed -= OnClosed;

      _hubRef?.Dispose();
      GC.SuppressFinalize(this);
   }


   public async ValueTask DisposeAsync()
   {
      Dispose();

      if (_hubConnection is not null)
         await _hubConnection.DisposeAsync();

      GC.SuppressFinalize(this);
   }

#pragma warning restore CS1591

   #endregion

   #region Properties

   /// <summary>
   /// Gets the connection identifier
   /// </summary>
   public string ConnectionId => _hubConnection?.ConnectionId;

   /// <summary>
   /// gets the hub connection state
   /// </summary>
   public HubConnectionState ConnectionState => _hubConnection?.State ?? HubConnectionState.Disconnected;

   #endregion
}