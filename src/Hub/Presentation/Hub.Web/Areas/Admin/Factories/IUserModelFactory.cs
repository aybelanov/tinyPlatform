using Hub.Core.Domain.Common;
using Hub.Core.Domain.Users;
using Hub.Web.Areas.Admin.Models.Gdpr;
using Hub.Web.Areas.Admin.Models.Users;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the user model factory
/// </summary>
public partial interface IUserModelFactory
{
   /// <summary>
   /// Prepare user search model
   /// </summary>
   /// <param name="searchModel">User search model</param>
   /// <param name="popup">For popup tables</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user search model
   /// </returns>
   Task<UserSearchModel> PrepareUserSearchModelAsync(UserSearchModel searchModel, bool popup = false);

   /// <summary>
   /// Prepare paged user list model
   /// </summary>
   /// <param name="searchModel">User search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user list model
   /// </returns>
   Task<UserListModel> PrepareUserListModelAsync(UserSearchModel searchModel);

   /// <summary>
   /// Prepare user model
   /// </summary>
   /// <param name="model">User model</param>
   /// <param name="user">User</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user model
   /// </returns>
   Task<UserModel> PrepareUserModelAsync(UserModel model, User user, bool excludeProperties = false);

   /// <summary>
   /// Prepare paged user own device list model
   /// </summary>
   /// <param name="searchModel">User device search model</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user own device list model
   /// </returns>
   Task<UserOwnDeviceListModel> PrepareUserOwnDeviceListModelAsync(UserDeviceSearchModel searchModel, User user);

   /// <summary>
   /// Prepare paged user shared device list model
   /// </summary>
   /// <param name="searchModel">User device search model</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the shared user device list model
   /// </returns>
   Task<UserSharedDeviceListModel> PrepareUserSharedDeviceListModelAsync(UserDeviceSearchModel searchModel, User user);

   /// <summary>
   /// Prepare paged user address list model
   /// </summary>
   /// <param name="searchModel">User address search model</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user address list model
   /// </returns>
   Task<UserAddressListModel> PrepareUserAddressListModelAsync(UserAddressSearchModel searchModel, User user);

   /// <summary>
   /// Prepare user address model
   /// </summary>
   /// <param name="model">User address model</param>
   /// <param name="user">User</param>
   /// <param name="address">Address</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user address model
   /// </returns>
   Task<UserAddressModel> PrepareUserAddressModelAsync(UserAddressModel model,
       User user, Address address, bool excludeProperties = false);

   /// <summary>
   /// Prepare paged user activity log list model
   /// </summary>
   /// <param name="searchModel">User activity log search model</param>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user activity log list model
   /// </returns>
   Task<UserActivityLogListModel> PrepareUserActivityLogListModelAsync(UserActivityLogSearchModel searchModel, User user);

   /// <summary>
   /// Prepare online user search model
   /// </summary>
   /// <param name="searchModel">Online user search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the online user search model
   /// </returns>
   Task<OnlineUserSearchModel> PrepareOnlineUserSearchModelAsync(OnlineUserSearchModel searchModel);

   /// <summary>
   /// Prepare paged online user list model
   /// </summary>
   /// <param name="searchModel">Online user search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the online user list model
   /// </returns>
   Task<OnlineUserListModel> PrepareOnlineUserListModelAsync(OnlineUserSearchModel searchModel);

   /// <summary>
   /// Prepare GDPR request (log) search model
   /// </summary>
   /// <param name="searchModel">GDPR request search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR request search model
   /// </returns>
   Task<GdprLogSearchModel> PrepareGdprLogSearchModelAsync(GdprLogSearchModel searchModel);

   /// <summary>
   /// Prepare paged GDPR request list model
   /// </summary>
   /// <param name="searchModel">GDPR request search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR request list model
   /// </returns>
   Task<GdprLogListModel> PrepareGdprLogListModelAsync(GdprLogSearchModel searchModel);
}