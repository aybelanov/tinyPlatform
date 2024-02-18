namespace Shared.Clients.Configuration;

/// <summary>
/// Represents default values related to users data
/// </summary>
public static partial class UserDefaults
{
   #region System user roles

   /// <summary>
   /// Gets a system name of 'administrators' user role
   /// </summary>
   public const string AdministratorsRoleName = "Administrators";

   /// <summary>
   /// Gets a system name of 'forum moderators' user role
   /// </summary>
   public const string ForumModeratorsRoleName = "ForumModerators";

   /// <summary>
   /// Gets a system name of 'registered' user role
   /// </summary>
   public const string RegisteredRoleName = "Registered";

   /// <summary>
   /// Gets a system name of 'guests' user role
   /// </summary>
   public const string GuestsRoleName = "Guests";

   /// <summary>
   /// Gets a system name for device owner's user role
   /// </summary>
   public const string OwnersRoleName = "Owners";

   /// <summary>
   /// Gets a system name for device operator's user role
   /// </summary>
   public const string OperatorsRoleName = "Operators";

   /// <summary>
   /// Gets a system name for device's user role
   /// </summary>
   public const string DevicesRoleName = "Devices";

   /// <summary>
   /// Gets a system name for device's user role
   /// </summary>
   public const string DemoRoleName = "Demo";

   /// <summary>
   /// All telemetry roles
   /// </summary>
   public const string TelemetryRoles = "Administrators,Owners,Operators";

   /// <summary>
   /// Telemetry admin roles
   /// </summary>
   public const string TelemetryAdminRoles = "Administrators,Owners";

   /// <summary>
   /// Telemetry user roles
   /// </summary>
   public const string TelemetryUserRoles = "Owners,Operators";

   #endregion
}