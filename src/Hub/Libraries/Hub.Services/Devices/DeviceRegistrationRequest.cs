using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;

namespace Hub.Services.Devices;

/// <summary>
/// Device registration request
/// </summary>
public class DeviceRegistrationRequest
{
   /// <summary>
   /// Ctor
   /// </summary>
   /// <param name="device">Device</param>
   /// <param name="deviceId">Device identifier</param>
   /// <param name="password">Password</param>
   /// <param name="passwordFormat">Password format</param>
   /// <param name="isApproved">Is approved</param>
   public DeviceRegistrationRequest(Device device, string deviceId,
       string password,
       PasswordFormat passwordFormat,
       bool isApproved = true)
   {
      Device = device;
      SystemName = deviceId;
      Password = password;
      PasswordFormat = passwordFormat;
      IsApproved = isApproved;
   }

   /// <summary>
   /// Device
   /// </summary>
   public Device Device { get; set; }

   /// <summary>
   /// Device unique name
   /// </summary>
   public string SystemName { get; set; }

   /// <summary>
   /// Password
   /// </summary>
   public string Password { get; set; }

   /// <summary>
   /// Password format
   /// </summary>
   public PasswordFormat PasswordFormat { get; set; }

   /// <summary>
   /// Is approved
   /// </summary>
   public bool IsApproved { get; set; }
}
