using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a news settings model
/// </summary>
public partial record NewsSettingsModel : BaseAppModel, ISettingsModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.News.Enabled")]
   public bool Enabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.News.AllowNotRegisteredUsersToLeaveComments")]
   public bool AllowNotRegisteredUsersToLeaveComments { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.News.NotifyAboutNewNewsComments")]
   public bool NotifyAboutNewNewsComments { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.News.ShowNewsOnMainPage")]
   public bool ShowNewsOnMainPage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.News.MainPageNewsCount")]
   public int MainPageNewsCount { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.News.NewsArchivePageSize")]
   public int NewsArchivePageSize { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.News.ShowHeaderRSSUrl")]
   public bool ShowHeaderRssUrl { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.News.NewsCommentsMustBeApproved")]
   public bool NewsCommentsMustBeApproved { get; set; }

   #endregion
}