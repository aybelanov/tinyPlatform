using FluentValidation;
using Hub.Core.Domain.Directory;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Directory;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Directory;

public partial class MeasureWeightValidator : BaseAppValidator<MeasureWeightModel>
{
   public MeasureWeightValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Weights.Fields.Name.Required"));
      RuleFor(x => x.SystemKeyword).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Weights.Fields.SystemKeyword.Required"));

      SetDatabaseValidationRules<MeasureWeight>(mappingEntityAccessor);
   }
}