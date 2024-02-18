using FluentValidation;
using Hub.Core.Domain.Gdpr;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Settings;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Settings
{
   public partial class GdprConsentValidator : BaseAppValidator<GdprConsentModel>
   {
      public GdprConsentValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
      {
         RuleFor(x => x.Message).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.Message.Required"));
         RuleFor(x => x.RequiredMessage)
             .NotEmpty()
             .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage.Required"))
             .When(x => x.IsRequired);

         SetDatabaseValidationRules<GdprConsent>(mappingEntityAccessor);
      }
   }
}