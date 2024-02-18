namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represents the device registration type formatting enumeration
/// </summary>
public enum DeviceRegistrationType
{
    /// <summary>
    /// Standard account creation
    /// </summary>
    Standard = 1,

    /// <summary>
    /// A user should be approved by administrator
    /// </summary>
    AdminApproval = 3,

    /// <summary>
    /// Registration is disabled
    /// </summary>
    Disabled = 4
}
