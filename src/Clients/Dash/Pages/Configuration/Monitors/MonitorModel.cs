using Clients.Dash.Models;
using Shared.Clients.Domain;

namespace Clients.Dash.Pages.Configuration.Monitors;

/// <summary>
/// Represents a monitor model class
/// </summary>
public class MonitorModel : BaseEntityModel, IMonitor
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the short description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Device picture url
    /// </summary>
    public string PictureUrl { get; set; }

    /// <summary>
    /// Title of the monitor page
    /// </summary>
    public string MenuItem { get; set; }

    /// <summary>
    /// Gets or sets a display order.
    /// This value is used when sorting on the monitor presentstion collcetion
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is published
    /// </summary>
    public bool ShowInMenu { get; set; }

    /// <summary>
    /// Device user-owner identifier
    /// </summary>
    public string OwnerId { get; set; }

    /// <summary>
    /// Device user-owner name
    /// </summary>
    public string OwnerName { get; set; }
}
