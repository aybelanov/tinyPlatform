using System.Collections.Generic;

namespace Clients.Dash.Shared.Menu;

/// <summary>
/// Represents a category model class
/// </summary>
public class CategoryMenuModel
{
    /// <summary>
    /// The new menu item's flag
    /// </summary>
    public bool New { get; set; }

    /// <summary>
    /// The updated menu item's flag
    /// </summary>
    public bool Updated { get; set; }

    /// <summary>
    /// Menu item's name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Menu item's icon
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Menu item's path
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Menu item's title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Menu item's description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Item's submenu expanded flag
    /// </summary>
    public bool Expanded { get; set; }

    /// <summary>
    /// Item's submenu item collection
    /// </summary>
    public IList<CategoryMenuModel> Children { get; set; }

    /// <summary>
    /// Menu item's tag collection
    /// </summary>
    public IList<string> Tags { get; set; }

    /// <summary>
    /// The item's order id
    /// </summary>
    public int OrderId { get; set; }
}