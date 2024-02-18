using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a blog settings model
/// </summary>
public partial record BlogSettingsModel : BaseAppModel, ISettingsModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.Blog.Enabled")]
   public bool Enabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Blog.PostsPageSize")]
   public int PostsPageSize { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Blog.AllowNotRegisteredUsersToLeaveComments")]
   public bool AllowNotRegisteredUsersToLeaveComments { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Blog.NotifyAboutNewBlogComments")]
   public bool NotifyAboutNewBlogComments { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Blog.NumberOfTags")]
   public int NumberOfTags { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Blog.ShowHeaderRSSUrl")]
   public bool ShowHeaderRssUrl { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Blog.BlogCommentsMustBeApproved")]
   public bool BlogCommentsMustBeApproved { get; set; }

   #endregion
}