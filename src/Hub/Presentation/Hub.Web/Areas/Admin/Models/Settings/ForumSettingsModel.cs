using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a forum settings model
   /// </summary>
   public partial record ForumSettingsModel : BaseAppModel, ISettingsModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.ForumsEnabled")]
      public bool ForumsEnabled { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.RelativeDateTimeFormattingEnabled")]
      public bool RelativeDateTimeFormattingEnabled { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.ShowUsersPostCount")]
      public bool ShowUsersPostCount { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.AllowGuestsToCreatePosts")]
      public bool AllowGuestsToCreatePosts { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.AllowGuestsToCreateTopics")]
      public bool AllowGuestsToCreateTopics { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.AllowUsersToEditPosts")]
      public bool AllowUsersToEditPosts { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.AllowUsersToDeletePosts")]
      public bool AllowUsersToDeletePosts { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.AllowPostVoting")]
      public bool AllowPostVoting { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.MaxVotesPerDay")]
      public int MaxVotesPerDay { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.AllowUsersToManageSubscriptions")]
      public bool AllowUsersToManageSubscriptions { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.TopicsPageSize")]
      public int TopicsPageSize { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.PostsPageSize")]
      public int PostsPageSize { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.ForumEditor")]
      public int ForumEditor { get; set; }
      public SelectList ForumEditorValues { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.SignaturesEnabled")]
      public bool SignaturesEnabled { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.AllowPrivateMessages")]
      public bool AllowPrivateMessages { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.ShowAlertForPM")]
      public bool ShowAlertForPM { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.NotifyAboutPrivateMessages")]
      public bool NotifyAboutPrivateMessages { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.ActiveDiscussionsFeedEnabled")]
      public bool ActiveDiscussionsFeedEnabled { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.ActiveDiscussionsFeedCount")]
      public int ActiveDiscussionsFeedCount { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.ForumFeedsEnabled")]
      public bool ForumFeedsEnabled { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.ForumFeedCount")]
      public int ForumFeedCount { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.SearchResultsPageSize")]
      public int SearchResultsPageSize { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.Forums.ActiveDiscussionsPageSize")]
      public int ActiveDiscussionsPageSize { get; set; }

      #endregion
   }
}