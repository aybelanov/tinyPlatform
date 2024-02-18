namespace Hub.Web.Areas.OpenId.Models;

/// <summary>
/// Represents a model for check session
/// </summary>
public partial record CheckSessionModel
{
   /// <summary>
   /// Session cookie name
   /// </summary>
   public string CookieName { get; set; }
}
