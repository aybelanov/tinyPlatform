using System;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Home;

/// <summary>
/// Represents a tinyPlatform news details model
/// </summary>
public partial record TinyPlatformNewsDetailsModel : BaseAppModel
{
   #region Properties

   public string Title { get; set; }

   public string Url { get; set; }

   public string Summary { get; set; }

   public DateTimeOffset PublishDate { get; set; }

   #endregion
}