using FluentValidation;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Core.Domain.Messages;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Messages
{
   public partial class NewsLetterSubscriptionValidator : BaseAppValidator<NewsletterSubscriptionModel>
   {
      public NewsLetterSubscriptionValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
      {
         RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Milticast.NewsLetterSubscriptions.Fields.Email.Required"));
         RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));

         SetDatabaseValidationRules<NewsLetterSubscription>(mappingEntityAccessor);
      }
   }
}