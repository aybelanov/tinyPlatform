using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Widgets;

/// <summary>
/// Widget select item for client UI elemets like a dropdown list
/// </summary>
public class WidgetSelectItem : BaseEntity
{
    /// <summary>
    /// Widget name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Widget owner name
    /// </summary>
    public string OwnerName { get; set; }
}
