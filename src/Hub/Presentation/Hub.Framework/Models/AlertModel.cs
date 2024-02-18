﻿namespace Hub.Web.Framework.Models;

/// <summary>
/// Alert model
/// </summary>
public record ActionAlertModel : BaseAppEntityModel
{
   /// <summary>
   /// Window ID
   /// </summary>
   public string WindowId { get; set; }
   /// <summary>
   /// Alert ID
   /// </summary>
   public string AlertId { get; set; }
   /// <summary>
   /// Alert message
   /// </summary>
   public string AlertMessage { get; set; }
}
