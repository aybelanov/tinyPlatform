namespace Hub.Core.Domain.Forums;

/// <summary>
/// Represents a forum search type
/// </summary>
public enum ForumSearchType
{
   /// <summary>
   /// ForumPost titles and post text
   /// </summary>
   All = 0,

   /// <summary>
   /// ForumPost titles only
   /// </summary>
   TopicTitlesOnly = 10,

   /// <summary>
   /// Post text only
   /// </summary>
   PostTextOnly = 20
}
