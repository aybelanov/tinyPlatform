using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a user settings model
/// </summary>
public partial record DeviceSettingsModel : BaseAppModel, ISettingsModel
{
   /// <summary>
   /// Gets or sets a value indicating we should store IP addresses of users
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.StoreIpAddresses")]
   public bool StoreIpAddresses { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether 'New user' notification message should be sent to a platform owner
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.NotifyNewUserRegistration")]
   public bool NotifyNewDeviceRegistration { get; set; }

   /// <summary>
   /// Default password format for devices
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.DefaultPasswordFormat")]
   public int DefaultPasswordFormat { get; set; }

   /// <summary>
   /// Gets or sets a minimum password length
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.PasswordMinLength")]
   public int PasswordMinLength { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether password are have least one lowercase
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.PasswordRequireLowercase")]
   public bool PasswordRequireLowercase { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether password are have least one uppercase
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.PasswordRequireUppercase")]
   public bool PasswordRequireUppercase { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether password are have least one non alphanumeric character
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.PasswordRequireNonAlphanumeric")]
   public bool PasswordRequireNonAlphanumeric { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether password are have least one digit
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.PasswordRequireDigit")]
   public bool PasswordRequireDigit { get; set; }

   /// <summary>
   /// Gets or sets a number of passwords that should not be the same as the previous one;
   /// 0 if the devices can use the same password time after time
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.UnduplicatedPasswordsNumber")]
   public int UnduplicatedPasswordNumber { get; set; }

   /// <summary>
   /// Gets or sets maximum login failures to lockout account. Set 0 to disable this feature
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.FailedPasswordAllowedAttempts")]
   public int FailedPasswordAllowedAttempts { get; set; }

   /// <summary>
   /// Gets or sets a number of minutes to lockout devices (for login failures).
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.FailedPasswordLockoutMinutes")]
   public int FailedPasswordLockoutMinutes { get; set; }

   /// <summary>
   /// Block devices if the owner is not active
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.BlockDevicesIfOwnerNotActive")]
   public bool BlockDevicesIfOwnerNotActive { get; set; }

   /// <summary>
   /// Device registration type
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.DeviceRegistrationType")]
   public DeviceRegistrationType DeviceRegistrationType { get; set; }

   /// <summary>
   /// Check system name awailability enabler
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.CheckSystemNameAvailabilityEnabled")]
   public bool CheckSystemNameAvailabilityEnabled { get; set; }

   /// <summary>
   /// Minimum system name lenght
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.SystemNameMinLength")]
   public int SystemNameMinLength { get; set; }

   /// <summary>
   /// Max records in DB after which task will remove extra records.
   /// Set to 0 if it is unlimited.
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.MaxSensorDatasInDb")]
   public int MaxSensorDatasInDb { get; set; }

   /// <summary>
   /// Expiration time of video file storing in minutes, 
   /// after which the task is to delete the expired video segments.
   /// Set to 0 if it doesn't expire.
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.VideoFileExpiration")]
   public int VideoFileExpiration { get; set; }

   /// <summary>
   /// Clear sensor record interval in minutes
   /// </summary>
   [AppResourceDisplayName("Admin.Configuration.Settings.Device.CleanSensorDataInterval")]
   public int CleanSensorDataInterval { get; set; }
}
