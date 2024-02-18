namespace Clients.Dash.Shared.Widgets;

/// <summary>
/// value helper for column and bar chart
/// </summary>
public class ValueHelper
{
   /// <summary>
   /// Category (it's required by Radzen widget) 
   /// </summary>
   public string Category { get; set; } = string.Empty;

   /// <summary>
   /// Current value
   /// </summary>
   public double Value { get; set; }
}
