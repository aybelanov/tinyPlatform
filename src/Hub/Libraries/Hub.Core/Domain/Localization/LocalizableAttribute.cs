using System;

namespace Hub.Core.Domain.Localization;

/// <summary>
/// Represents a localizable attribute for property localization
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LocalizableAttribute : Attribute
{
}
