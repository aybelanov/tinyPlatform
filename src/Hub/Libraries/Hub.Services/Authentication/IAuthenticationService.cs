using System.Threading.Tasks;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;

namespace Hub.Services.Authentication;

/// <summary>
/// Authentication service interface
/// </summary>
public partial interface IAuthenticationService
{
   /// <summary>
   /// User sign in
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task SignInAsync(User user, bool isPersistent);

   /// <summary>
   /// Sign out
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task SignOutAsync();

   /// <summary>
   /// Get authenticated user
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user
   /// </returns>
   Task<User> GetAuthenticatedUserAsync();

   /// <summary>
   /// Get authenticated device
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device
   /// </returns>
   Task<Device> GetAuthenticatedDeviceAsync();
}