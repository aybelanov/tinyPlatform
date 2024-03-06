using FluentValidation;
using Hub.Core.Domain.Directory;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Directory;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Directory;

public partial class StateProvinceValidator : BaseAppValidator<StateProvinceModel>
{
   public StateProvinceValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.States.Fields.Name.Required"));

      SetDatabaseValidationRules<StateProvince>(mappingEntityAccessor);
   }
}