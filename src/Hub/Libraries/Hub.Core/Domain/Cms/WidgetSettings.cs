using Hub.Core.Configuration;
using System.Collections.Generic;

namespace Hub.Core.Domain.Cms;

/// <summary>
/// Widget settings
/// </summary>
public class WidgetSettings : ISettings
{
   /// <summary>
   /// Default Ctor
   /// </summary>
   public WidgetSettings()
   {
      ActiveWidgetSystemNames = new List<string>();
   }

   /// <summary>
   /// Gets or sets a system names of active widgets
   /// </summary>
   public List<string> ActiveWidgetSystemNames { get; set; }
}