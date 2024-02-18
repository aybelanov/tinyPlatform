namespace Hub.Core.Domain.Messages;

/// <summary>
/// Represents message template system names
/// </summary>
public static partial class MessageTemplateSystemNames
{
   #region User

   /// <summary>
   /// Represents system name of notification about new registration
   /// </summary>
   public const string UserRegisteredNotification = "NewUser.Notification";

   /// <summary>
   /// Represents system name of user welcome message
   /// </summary>
   public const string UserWelcomeMessage = "User.WelcomeMessage";

   /// <summary>
   /// Represents system name of email validation message
   /// </summary>
   public const string UserEmailValidationMessage = "User.EmailValidationMessage";

   /// <summary>
   /// Represents system name of email revalidation message
   /// </summary>
   public const string UserEmailRevalidationMessage = "User.EmailRevalidationMessage";

   /// <summary>
   /// Represents system name of password recovery message
   /// </summary>
   public const string UserPasswordRecoveryMessage = "User.PasswordRecovery";

   #endregion

   #region Device

   /// <summary>
   /// Represents system name of notification about new registration
   /// </summary>
   public const string DeviceRegisteredNotification = "NewDevice.Notification";

   #endregion

   #region Newsletter

   /// <summary>
   /// Represents system name of subscription activation message
   /// </summary>
   public const string NewsletterSubscriptionActivationMessage = "NewsLetterSubscription.ActivationMessage";

   /// <summary>
   /// Represents system name of subscription deactivation message
   /// </summary>
   public const string NewsletterSubscriptionDeactivationMessage = "NewsLetterSubscription.DeactivationMessage";

   #endregion

   #region To friend

   /// <summary>
   /// Represents system name of 'Email a friend' message
   /// </summary>
   public const string EmailAFriendMessage = "Service.EmailAFriend";

   /// <summary>
   /// Represents system name of 'Email a friend' message with wishlist
   /// </summary>
   public const string WishlistToFriendMessage = "Wishlist.EmailAFriend";

   #endregion

   #region Forum

   /// <summary>
   /// Represents system name of notification about new forum topic
   /// </summary>
   public const string NewForumTopicMessage = "Forums.NewForumTopic";

   /// <summary>
   /// Represents system name of notification about new forum post
   /// </summary>
   public const string NewForumPostMessage = "Forums.NewForumPost";

   /// <summary>
   /// Represents system name of notification about new private message
   /// </summary>
   public const string PrivateMessageNotification = "User.NewPM";

   #endregion

   #region Misc

   /// <summary>
   /// Represents system name of notification platform owner about submitting new VAT
   /// </summary>
   public const string NewVatSubmittedPlatformOwnerNotification = "NewVATSubmitted.PlatformOwnerNotification";

   /// <summary>
   /// Represents system name of notification platform owner about new blog comment
   /// </summary>
   public const string BlogCommentNotification = "Blog.BlogComment";

   /// <summary>
   /// Represents system name of notification platform owner about new news comment
   /// </summary>
   public const string NewsCommentNotification = "News.NewsComment";

   /// <summary>
   /// Represents system name of 'Contact us' message
   /// </summary>
   public const string ContactUsMessage = "Service.ContactUs";

   #endregion
}