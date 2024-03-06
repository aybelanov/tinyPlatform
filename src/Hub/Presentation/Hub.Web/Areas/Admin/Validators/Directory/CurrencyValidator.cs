using FluentValidation;
using Hub.Core.Domain.Directory;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Directory;
using Hub.Web.Framework.Validators;
using System.Globalization;

namespace Hub.Web.Areas.Admin.Validators.Directory;

public partial class CurrencyValidator : BaseAppValidator<CurrencyModel>
{
   public CurrencyValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name)
          .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Currencies.Fields.Name.Required"))
          .Length(1, 50).WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Currencies.Fields.Name.Range"));
      RuleFor(x => x.CurrencyCode)
          .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Currencies.Fields.CurrencyCode.Required"))
          .Length(1, 5).WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Currencies.Fields.CurrencyCode.Range"));
      RuleFor(x => x.Rate)
          .GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Currencies.Fields.Rate.Range"));
      RuleFor(x => x.CustomFormatting)
          .Length(0, 50).WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Currencies.Fields.CustomFormatting.Validation"));
      RuleFor(x => x.DisplayLocale)
          .Must(x =>
          {
             try
             {
                if (string.IsNullOrEmpty(x))
                   return true;
                //let's try to create a CultureInfo object
                //if "DisplayLocale" is wrong, then exception will be thrown
                var unused = new CultureInfo(x);
                return true;
             }
             catch
             {
                return false;
             }
          })
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Currencies.Fields.DisplayLocale.Validation"));

      SetDatabaseValidationRules<Currency>(mappingEntityAccessor);
   }
}