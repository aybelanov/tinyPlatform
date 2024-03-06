using FluentValidation;
using Hub.Core.Domain.Directory;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Directory;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Directory;

public partial class MeasureDimensionValidator : BaseAppValidator<MeasureDimensionModel>
{
   public MeasureDimensionValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Dimensions.Fields.Name.Required"));
      RuleFor(x => x.SystemKeyword).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Dimensions.Fields.SystemKeyword.Required"));

      SetDatabaseValidationRules<MeasureDimension>(mappingEntityAccessor);
   }
}