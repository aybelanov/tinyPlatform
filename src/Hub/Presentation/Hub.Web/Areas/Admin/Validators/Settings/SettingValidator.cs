using FluentValidation;
using Hub.Web.Areas.Admin.Models.Settings;
using Hub.Core.Domain.Configuration;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Settings;

public partial class SettingValidator : BaseAppValidator<SettingModel>
{
   public SettingValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.Name.Required"));

      SetDatabaseValidationRules<Setting>(mappingEntityAccessor);
   }
}