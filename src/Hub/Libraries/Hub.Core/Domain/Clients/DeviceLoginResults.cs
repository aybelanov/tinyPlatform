namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represents the device login result enumeration
/// </summary>
public enum DeviceLoginResults
{
    /// <summary>
    /// Login successful
    /// </summary>
    Successful = 1,

    /// <summary>
    /// Device does not exist (email or devicename)
    /// </summary>
    DeviceNotExist = 2,

    /// <summary>
    /// Wrong password
    /// </summary>
    WrongPassword = 3,

    /// <summary>
    /// Account have not been activated
    /// </summary>
    DeviceNotActive = 4,

    /// <summary>
    /// Device has been deleted 
    /// </summary>
    DeviceDeleted = 5,

    /// <summary>
    /// Locked out
    /// </summary>
    DeviceLockedOut = 7,

    /// <summary>
    /// User does not exist (email or username)
    /// </summary>
    UserNotExist = 8,

    /// <summary>
    /// Account have not been activated
    /// </summary>
    UserNotActive = 9,

    /// <summary>
    /// User has been deleted 
    /// </summary>
    UserDeleted = 10,

    /// <summary>
    /// User not registered 
    /// </summary>
    UserNotRegistered = 11,

    /// <summary>
    /// Locked out
    /// </summary>
    UserLockedOut = 12,
}
