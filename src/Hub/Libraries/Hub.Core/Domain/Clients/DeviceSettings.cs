using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Configuration;
using Hub.Core.Domain.Users;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Device settings
/// </summary>
public class DeviceSettings : ISettings
{
    // TODO store device IP addres grpc interceptor (or add when token is provided)
    /// <summary>
    /// Store (and show) device ip adress
    /// </summary>
    public bool StoreIpAddresses { get; set; }

    /// <summary>
    /// Default password format for devices
    /// </summary>
    public PasswordFormat DefaultPasswordFormat { get; set; }

    /// <summary>
    /// Gets or sets a device password format (SHA1, MD5) when passwords are hashed (DO NOT edit in production environment)
    /// </summary>
    public string HashedPasswordFormat { get; set; }

    /// <summary>
    /// Gets or sets a minimum password length
    /// </summary>
    public int PasswordMinLength { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether password are have least one lowercase
    /// </summary>
    public bool PasswordRequireLowercase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether password are have least one uppercase
    /// </summary>
    public bool PasswordRequireUppercase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether password are have least one non alphanumeric character
    /// </summary>
    public bool PasswordRequireNonAlphanumeric { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether password are have least one digit
    /// </summary>
    public bool PasswordRequireDigit { get; set; }

    /// <summary>
    /// Gets or sets a number of passwords that should not be the same as the previous one; 0 if the devices can use the same password time after time
    /// </summary>
    public int UnduplicatedPasswordNumber { get; set; }

    /// <summary>
    /// Gets or sets maximum login failures to lockout account. Set 0 to disable this feature
    /// </summary>
    public int FailedPasswordAllowedAttempts { get; set; }

    /// <summary>
    /// Gets or sets a number of minutes to lockout devices (for login failures).
    /// </summary>
    public int FailedPasswordLockoutMinutes { get; set; }

    /// <summary>
    /// Block devices if the owner is not active
    /// </summary>
    public bool BlockDevicesIfOwnerNotActive { get; set; }

    /// <summary>
    /// Device registration type
    /// </summary>
    public DeviceRegistrationType DeviceRegistrationType { get; set; }

    /// <summary>
    /// Check system name awailability enabler
    /// </summary>
    public bool CheckSystemNameAvailabilityEnabled { get; set; }

    /// <summary>
    /// Minimum system name lenght
    /// </summary>
    public int SystemNameMinLength { get; set; }

   /// <summary>
   /// Maximum system name lenght
   /// </summary>
   public int SystemNameMaxLength { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether 'New device'
   /// notification message should be sent to a platform owner 
   /// </summary>
   public bool NotifyNewDeviceRegistration { get; set; }

    /// <summary>
    /// The maximum number of records in the database,
    /// after which the task is to delete the extra records.
    /// Set to 0 if it is unlimited.
    /// </summary>
    public int MaxSensorDatasInDb { get; set; }

    /// <summary>
    /// Expiration time of video file storing in minutes,
    /// Set to 0 if it doesn't expire.
    /// </summary>
    public int VideoFileExpiration { get; set; }

    /// <summary>
    /// Device data cleaning interval in minutes (in db and in file system)
    /// </summary>
    public int CleanSensorDataInterval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the number of minutes for 'online device' module
    /// </summary>
    public int BeenRecentlyMinutes { get; set; }
}
