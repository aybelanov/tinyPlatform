namespace Hub.Web.Framework.Events;

/// <summary>
/// Site search event
/// </summary>
public class SiteSearchEvent
{
   /// <summary>
   /// Search term
   /// </summary>
   public string SearchTerm { get; set; }
   /// <summary>
   /// Search in descriptions
   /// </summary>
   public bool SearchInDescriptions { get; set; }
   /// <summary>
   /// Language identifier
   /// </summary>
   public int WorkingLanguageId { get; set; }
}
