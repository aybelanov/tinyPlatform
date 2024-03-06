using Hub.Core.Domain.Clients;
using Hub.Services.Users;
using System.Threading.Tasks;

namespace Hub.Services.Devices;

/// <summary>
/// Represents a device registration service interface 
/// </summary>
public interface IDeviceRegistrationService
{
   /// <summary>
   /// Validate device
   /// </summary>
   /// <param name="systemName">Device identifier</param>
   /// <param name="password">Password</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   Task<DeviceLoginResults> ValidateDeviceCredentialsAsync(string systemName, string password);

   /// <summary>
   /// Register device
   /// </summary>
   /// <param name="request">Request</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   Task<DeviceRegistrationResult> RegisterDeviceAsync(DeviceRegistrationRequest request);

   /// <summary>
   /// Change password
   /// </summary>
   /// <param name="request">Request</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request);

   /// <summary>
   /// validates device systemname
   /// </summary>
   /// <param name="systemName">Validating systemname</param>
   /// <returns>Validating result</returns>
   Task<DeviceRegistrationResult> ValidateSystemNameAsync(string systemName);

   /// <summary>
   /// Validates device password
   /// </summary>
   /// <param name="newPassword">Validated password</param>
   /// <returns>Validating result</returns>
   Task<DeviceRegistrationResult> ValidatePasswordFormatAsync(string newPassword);
}

