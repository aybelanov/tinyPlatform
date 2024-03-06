using FluentValidation;
using Hub.Core.Domain.Directory;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Directory;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Directory;

public partial class CountryValidator : BaseAppValidator<CountryModel>
{
   public CountryValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.Name.Required"));
      RuleFor(p => p.Name).Length(1, 100);

      RuleFor(x => x.TwoLetterIsoCode)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.TwoLetterIsoCode.Required"));
      RuleFor(x => x.TwoLetterIsoCode)
          .Length(2)
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.TwoLetterIsoCode.Length"));

      RuleFor(x => x.ThreeLetterIsoCode)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode.Required"));
      RuleFor(x => x.ThreeLetterIsoCode)
          .Length(3)
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode.Length"));

      SetDatabaseValidationRules<Country>(mappingEntityAccessor);
   }
}