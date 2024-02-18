namespace Hub.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents path and routes defaults
/// </summary>
public static partial class AppPathRouteDefaults
{
   /// <summary>
   /// Gets default key for action field
   /// </summary>
   public static string ActionFieldKey => "action";

   /// <summary>
   /// Gets default key for controller field
   /// </summary>
   public static string ControllerFieldKey => "controller";

   /// <summary>
   /// Gets default key for permanent redirect field
   /// </summary>
   public static string PermanentRedirectFieldKey => "permanentRedirect";

   /// <summary>
   /// Gets default key for url field
   /// </summary>
   public static string UrlFieldKey => "url";

   /// <summary>
   /// Gets default key for blogpost id field
   /// </summary>
   public static string BlogPostIdFieldKey => "blogpostId";

   /// <summary>
   /// Gets default key for newsitem id field
   /// </summary>
   public static string NewsItemIdFieldKey => "newsitemId";

   /// <summary>
   /// Gets default key for se name field
   /// </summary>
   public static string SeNameFieldKey => "sename";

   /// <summary>
   /// Gets default key for topic id field
   /// </summary>
   public static string TopicIdFieldKey => "topicid";

   /// <summary>
   /// Gets language route value
   /// </summary>
   public static string LanguageRouteValue => "language";

   /// <summary>
   /// Gets language parameter transformer
   /// </summary>
   public static string LanguageParameterTransformer => "lang";
}