using FluentValidation;
using Hub.Core.Domain.Localization;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Localization;

public partial class LanguageResourceValidator : BaseAppValidator<LocaleResourceModel>
{
   public LanguageResourceValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      //if validation without this set rule is applied, in this case nothing will be validated
      //it's used to prevent auto-validation of child models
      RuleSet(AppValidationDefaults.ValidationRuleSet, () =>
      {
         RuleFor(model => model.ResourceName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.Fields.Name.Required"));

         RuleFor(model => model.ResourceValue)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.Fields.Value.Required"));

         SetDatabaseValidationRules<LocaleStringResource>(mappingEntityAccessor);
      });
   }
}