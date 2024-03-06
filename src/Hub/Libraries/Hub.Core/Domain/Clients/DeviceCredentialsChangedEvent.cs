namespace Hub.Core.Domain.Clients;

/// <summary>
/// Device credential changed event
/// </summary>
public class DeviceCredentialsChangedEvent
{
   /// <summary>
   /// Ctor
   /// </summary>
   /// <param name="password">Password</param>
   public DeviceCredentialsChangedEvent(DeviceCredential password)
   {
      Password = password;
   }

   /// <summary>
   /// Device password
   /// </summary>
   public DeviceCredential Password { get; }
}