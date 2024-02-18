using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Home;

/// <summary>
/// Represents a service developer news model
/// </summary>
public partial record TinyPlatformNewsModel : BaseAppModel
{
   #region Ctor

   public TinyPlatformNewsModel()
   {
      Items = new List<TinyPlatformNewsDetailsModel>();
   }

   #endregion

   #region Properties

   public List<TinyPlatformNewsDetailsModel> Items { get; set; }

   public bool HasNewItems { get; set; }

   public bool HideAdvertisements { get; set; }

   #endregion
}