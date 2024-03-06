using Hub.Core;
using Hub.Core.Domain.Messages;
using Hub.Services.Localization;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Web.Factories;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Controllers
{
   [AutoValidateAntiforgeryToken]
   public partial class NewsletterController : BasePublicController
   {
      private readonly ILocalizationService _localizationService;
      private readonly INewsletterModelFactory _newsletterModelFactory;
      private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
      private readonly IPermissionService _permissionService;
      private readonly IWorkContext _workContext;
      private readonly IWorkflowMessageService _workflowMessageService;

      public NewsletterController(ILocalizationService localizationService,
          INewsletterModelFactory newsletterModelFactory,
          INewsLetterSubscriptionService newsLetterSubscriptionService,
          IPermissionService permissionService,
          IWorkContext workContext,
          IWorkflowMessageService workflowMessageService)
      {
         _localizationService = localizationService;
         _newsletterModelFactory = newsletterModelFactory;
         _newsLetterSubscriptionService = newsLetterSubscriptionService;
         _permissionService = permissionService;
         _workContext = workContext;
         _workflowMessageService = workflowMessageService;
      }

      //available even when a platform is closed
      [CheckAccessPublicPlatform(true)]
      [HttpPost]
      public virtual async Task<IActionResult> SubscribeNewsletter(string email, bool subscribe)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessNews))
            return new ChallengeResult();

         string result;
         var success = false;

         if (!CommonHelper.IsValidEmail(email))
            result = await _localizationService.GetResourceAsync("Newsletter.Email.Wrong");
         else
         {
            email = email.Trim();
            var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAsync(email);
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            if (subscription != null)
               if (subscribe)
               {
                  if (!subscription.Active)
                     await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription, currentLanguage.Id);
                  result = await _localizationService.GetResourceAsync("Newsletter.SubscribeEmailSent");
               }
               else
               {
                  if (subscription.Active)
                     await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(subscription, currentLanguage.Id);
                  result = await _localizationService.GetResourceAsync("Newsletter.UnsubscribeEmailSent");
               }
            else if (subscribe)
            {
               subscription = new NewsLetterSubscription
               {
                  NewsLetterSubscriptionGuid = Guid.NewGuid(),
                  Email = email,
                  Active = false,
                  CreatedOnUtc = DateTime.UtcNow
               };
               await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
               await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription, currentLanguage.Id);

               result = await _localizationService.GetResourceAsync("Newsletter.SubscribeEmailSent");
            }
            else
               result = await _localizationService.GetResourceAsync("Newsletter.UnsubscribeEmailSent");
            success = true;
         }

         return Json(new
         {
            Success = success,
            Result = result,
         });
      }

      //available even when a platform is closed
      [CheckAccessPublicPlatform(true)]
      public virtual async Task<IActionResult> SubscriptionActivation(Guid token, bool active)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessNews))
            return new ChallengeResult();

         var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByGuidAsync(token);
         if (subscription == null)
            return RedirectToRoute("Homepage");

         if (active)
         {
            subscription.Active = true;
            await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
         }
         else
            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

         var model = await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(active);
         return View(model);
      }
   }
}