using System.Runtime.Serialization;

namespace Hub.Core.Configuration;

/// <summary>
/// Represents distributed cache types enumeration
/// </summary>
public enum DistributedCacheType
{
#pragma warning disable CS1591

   [EnumMember(Value = "memory")]
   Memory,
   [EnumMember(Value = "sqlserver")]
   SqlServer,
   [EnumMember(Value = "redis")]
   Redis

#pragma warning restore CS1591
}