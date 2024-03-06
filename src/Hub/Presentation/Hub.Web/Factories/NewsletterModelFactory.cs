using Hub.Core.Domain.Users;
using Hub.Services.Localization;
using Hub.Web.Models.Newsletter;
using System.Threading.Tasks;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the newsletter model factory
/// </summary>
public partial class NewsletterModelFactory : INewsletterModelFactory
{
   #region Fields

   private readonly UserSettings _userSettings;
   private readonly ILocalizationService _localizationService;

   #endregion

   #region Ctor

   public NewsletterModelFactory(UserSettings userSettings, ILocalizationService localizationService)
   {
      _userSettings = userSettings;
      _localizationService = localizationService;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare the newsletter box model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the newsletter box model
   /// </returns>
   public virtual Task<NewsletterBoxModel> PrepareNewsletterBoxModelAsync()
   {
      var model = new NewsletterBoxModel
      {
         AllowToUnsubscribe = _userSettings.NewsletterBlockAllowToUnsubscribe
      };

      return Task.FromResult(model);
   }

   /// <summary>
   /// Prepare the subscription activation model
   /// </summary>
   /// <param name="active">Whether the subscription has been activated</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the subscription activation model
   /// </returns>
   public virtual async Task<SubscriptionActivationModel> PrepareSubscriptionActivationModelAsync(bool active)
   {
      var model = new SubscriptionActivationModel
      {
         Result = active
          ? await _localizationService.GetResourceAsync("Newsletter.ResultActivated")
          : await _localizationService.GetResourceAsync("Newsletter.ResultDeactivated")
      };

      return model;
   }

   #endregion
}
