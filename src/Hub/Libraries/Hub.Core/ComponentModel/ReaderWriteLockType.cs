namespace Hub.Core.ComponentModel;

/// <summary>
/// Reader/Write locker type
/// </summary>
public enum ReaderWriteLockType
{
#pragma warning disable CS1591

   Read,
   Write,
   UpgradeableRead

#pragma warning restore CS1591
}
