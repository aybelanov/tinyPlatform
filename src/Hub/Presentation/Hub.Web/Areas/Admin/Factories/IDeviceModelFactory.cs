using Hub.Core.Domain.Clients;
using Hub.Web.Areas.Admin.Models.Devices;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents a device model factory interface
/// </summary>
public interface IDeviceModelFactory
{
   /// <summary>
   /// Prepares device search model
   /// </summary>
   /// <param name="searchModel">Device search model</param>
   /// <param name="popup">For popup tables</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device search model
   /// </returns>
   Task<DeviceSearchModel> PrepareDeviceSearchModelAsync(DeviceSearchModel searchModel, bool popup = false);

   /// <summary>
   /// Prepares paged device list model
   /// </summary>
   /// <param name="searchModel">Device search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device list model
   /// </returns>
   Task<DeviceListModel> PrepareDeviceListModelAsync(DeviceSearchModel searchModel);

   /// <summary>
   /// Prepares device model
   /// </summary>
   /// <param name="model">Device model</param>
   /// <param name="device">Device</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device model
   /// </returns>
   Task<DeviceModel> PrepareDeviceModelAsync(DeviceModel model, Device device);

   /// <summary>
   /// Prepare paged device activity log list model
   /// </summary>
   /// <param name="searchModel">Device activity log search model</param>
   /// <param name="device">Device</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device activity log list model
   /// </returns>
   Task<DeviceActivityLogListModel> PrepareDeviceActivityLogListModelAsync(DeviceActivityLogSearchModel searchModel, Device device);

   /// <summary>
   /// Prepare online device search model
   /// </summary>
   /// <param name="searchModel">Online device search model</param>
   /// <param name="popup">For popup tables</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the online device search model
   /// </returns>
   Task<OnlineDeviceSearchModel> PrepareOnlineDeviceSearchModelAsync(OnlineDeviceSearchModel searchModel);

   /// <summary>
   /// Prepare paged online device list model
   /// </summary>
   /// <param name="searchModel">Online device search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the online device list model
   /// </returns>
   Task<OnlineDeviceListModel> PrepareOnlineDeviceListModelAsync(OnlineDeviceSearchModel searchModel);

   /// <summary>
   /// Prepare paged device users list model
   /// </summary>
   /// <param name="searchModel">Device user search model</param>
   /// <param name="device">Device</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user list model
   /// </returns>
   Task<DeviceUserListModel> PrepareDeviceUserListModelAsync(DeviceUserSearchModel searchModel, Device device);
}
