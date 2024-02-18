using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Core.Domain.Localization;

/// <summary>
/// Represents a localizable attribute for property localization
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LocalizableAttribute : Attribute
{
}
