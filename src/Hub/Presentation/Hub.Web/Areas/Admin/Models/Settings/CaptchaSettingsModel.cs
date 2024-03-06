using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a CAPTCHA settings model
/// </summary>
public partial record CaptchaSettingsModel : BaseAppModel, ISettingsModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaEnabled")]
   public bool Enabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage")]
   public bool ShowOnLoginPage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnRegistrationPage")]
   public bool ShowOnRegistrationPage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnContactUsPage")]
   public bool ShowOnContactUsPage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage")]
   public bool ShowOnEmailWishlistToFriendPage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailToFriendPage")]
   public bool ShowOnEmailToFriendPage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnBlogCommentPage")]
   public bool ShowOnBlogCommentPage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsCommentPage")]
   public bool ShowOnNewsCommentPage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForgotPasswordPage")]
   public bool ShowOnForgotPasswordPage { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForum")]
   public bool ShowOnForum { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPublicKey")]
   public string ReCaptchaPublicKey { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPrivateKey")]
   public string ReCaptchaPrivateKey { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaType")]
   public int CaptchaType { get; set; }
   public SelectList CaptchaTypeValues { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaV3ScoreThreshold")]
   public decimal ReCaptchaV3ScoreThreshold { get; set; }

   #endregion
}