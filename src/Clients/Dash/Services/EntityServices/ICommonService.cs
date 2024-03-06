using Clients.Dash.Domain;
using Shared.Clients;
using Shared.Clients.Proto;
using System.Threading.Tasks;

namespace Clients.Dash.Services.EntityServices;

/// <summary>
/// Represents a common service interface
/// </summary>
public interface ICommonService
{
   /// <summary>
   /// Gets users by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamoc filter</param>
   /// <returns>User collection</returns>
   Task<IFilterableList<User>> GetUsersAsync(DynamicFilter filter);

   /// <summary>
   /// Gets user select items by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamoc filter</param>
   /// <returns>User select item collection</returns>
   Task<IFilterableList<UserSelectItem>> GetUserSelecItemsAsync(DynamicFilter filter);

   /// <summary>
   /// Gets activity log for a device 
   /// </summary>
   /// <param name="filter"></param>
   /// <returns>Activity log record collection</returns>
   Task<IFilterableList<ActivityLogRecord>> GetDeviceActivityLogAsync(DynamicFilter filter);


   /// <summary>
   /// Gets user activity log for a user
   /// </summary>
   /// <param name="filter"></param>
   /// <returns>Activity log record collection</returns>
   Task<IFilterableList<ActivityLogRecord>> GetUserActivityLogAsync(DynamicFilter filter);

   /// <summary>
   /// Checks username availability
   /// </summary>
   /// <param name="userName">Username</param>
   /// <returns>Check result</returns>
   Task<CommonResponse> CheckUserNameAvailabilityAsync(string userName);
}