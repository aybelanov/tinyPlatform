using Hub.Web.Framework.Models;
using System.Collections.Generic;
using System.Linq;

namespace Hub.Web.Models.Common;

public partial record TopMenuModel : BaseAppModel
{
   public TopMenuModel()
   {
      Topics = new List<TopicModel>();
   }

   public IList<TopicModel> Topics { get; set; }

   public bool BlogEnabled { get; set; }
   public bool ForumEnabled { get; set; }

   public bool DisplayHomepageMenuItem { get; set; }
   public bool DisplaySearchMenuItem { get; set; }
   public bool DisplayUserInfoMenuItem { get; set; }
   public bool DisplayBlogMenuItem { get; set; }
   public bool DisplayForumsMenuItem { get; set; }
   public bool DisplayContactUsMenuItem { get; set; }

   public bool UseAjaxMenu { get; set; }

   public bool HasOnlyCategories => !Topics.Any()
                  && !DisplayHomepageMenuItem
                  && !DisplayUserInfoMenuItem
                  && !(DisplayBlogMenuItem && BlogEnabled)
                  && !(DisplayForumsMenuItem && ForumEnabled)
                  && !DisplayContactUsMenuItem;

   #region Nested classes

   public record TopicModel : BaseAppEntityModel
   {
      public string Name { get; set; }
      public string SeName { get; set; }
   }
   #endregion
}