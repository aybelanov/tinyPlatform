using Hub.Core.Configuration;

namespace Hub.Core.Domain.Forums;

/// <summary>
/// Forum settings
/// </summary>
public class ForumSettings : ISettings
{
   /// <summary>
   /// Gets or sets a value indicating whether forums are enabled
   /// </summary>
   public bool ForumsEnabled { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether relative date and time formatting is enabled (e.g. 2 hours ago, a month ago)
   /// </summary>
   public bool RelativeDateTimeFormattingEnabled { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether users are allowed to edit posts that they created
   /// </summary>
   public bool AllowUsersToEditPosts { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether users are allowed to manage their subscriptions
   /// </summary>
   public bool AllowUsersToManageSubscriptions { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether guests are allowed to create posts
   /// </summary>
   public bool AllowGuestsToCreatePosts { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether guests are allowed to create topics
   /// </summary>
   public bool AllowGuestsToCreateTopics { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether users are allowed to delete posts that they created
   /// </summary>
   public bool AllowUsersToDeletePosts { get; set; }

   /// <summary>
   /// Gets or sets the whether users can vote for posts
   /// </summary>
   public bool AllowPostVoting { get; set; }

   /// <summary>
   /// Gets or sets the maximum number of votes for user per day
   /// </summary>
   public int MaxVotesPerDay { get; set; }

   /// <summary>
   /// Gets or sets maximum length of topic subject
   /// </summary>
   public int TopicSubjectMaxLength { get; set; }

   /// <summary>
   /// Gets or sets the maximum length for stripped forum topic names
   /// </summary>
   public int StrippedTopicMaxLength { get; set; }

   /// <summary>
   /// Gets or sets maximum length of post
   /// </summary>
   public int PostMaxLength { get; set; }

   /// <summary>
   /// Gets or sets the page size for topics in forums
   /// </summary>
   public int TopicsPageSize { get; set; }

   /// <summary>
   /// Gets or sets the page size for posts in topics
   /// </summary>
   public int PostsPageSize { get; set; }

   /// <summary>
   /// Gets or sets the page size for search result
   /// </summary>
   public int SearchResultsPageSize { get; set; }

   /// <summary>
   /// Gets or sets the page size for the Active Discussions page
   /// </summary>
   public int ActiveDiscussionsPageSize { get; set; }

   /// <summary>
   /// Gets or sets the page size for latest user posts
   /// </summary>
   public int LatestUserPostsPageSize { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to show users forum post count
   /// </summary>
   public bool ShowUsersPostCount { get; set; }

   /// <summary>
   /// Gets or sets a forum editor type
   /// </summary>
   public EditorType ForumEditor { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether users are allowed to specify a signature
   /// </summary>
   public bool SignaturesEnabled { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether private messages are allowed
   /// </summary>
   public bool AllowPrivateMessages { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether an alert should be shown for new private messages
   /// </summary>
   public bool ShowAlertForPM { get; set; }

   /// <summary>
   /// Gets or sets the page size for private messages
   /// </summary>
   public int PrivateMessagesPageSize { get; set; }

   /// <summary>
   /// Gets or sets the page size for (My Account) Forum Subscriptions
   /// </summary>
   public int ForumSubscriptionsPageSize { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether a user should be notified about new private messages
   /// </summary>
   public bool NotifyAboutPrivateMessages { get; set; }

   /// <summary>
   /// Gets or sets maximum length of pm subject
   /// </summary>
   public int PMSubjectMaxLength { get; set; }

   /// <summary>
   /// Gets or sets maximum length of pm message
   /// </summary>
   public int PMTextMaxLength { get; set; }

   /// <summary>
   /// Gets or sets the number of items to display for Active Discussions on forums home page
   /// </summary>
   public int HomepageActiveDiscussionsTopicCount { get; set; }

   /// <summary>
   /// Gets or sets the number of items to display for Active Discussions RSS Feed
   /// </summary>
   public int ActiveDiscussionsFeedCount { get; set; }

   /// <summary>
   /// Gets or sets the whether the Active Discussions RSS Feed is enabled
   /// </summary>
   public bool ActiveDiscussionsFeedEnabled { get; set; }

   /// <summary>
   /// Gets or sets the whether Forums have an RSS Feed enabled
   /// </summary>
   public bool ForumFeedsEnabled { get; set; }

   /// <summary>
   /// Gets or sets the number of items to display for Forum RSS Feed
   /// </summary>
   public int ForumFeedCount { get; set; }

   /// <summary>
   /// Gets or sets the minimum length for search term
   /// </summary>
   public int ForumSearchTermMinimumLength { get; set; }
}
