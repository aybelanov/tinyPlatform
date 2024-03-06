using FluentValidation;
using Hub.Core.Domain.Common;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Common;

public partial class AddressValidator : BaseAppValidator<AddressModel>
{
   public AddressValidator(AddressSettings addressSettings, ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.FirstName)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.FirstName.Required"))
          .When(x => x.FirstNameRequired);
      RuleFor(x => x.LastName)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.LastName.Required"))
          .When(x => x.LastNameRequired);
      RuleFor(x => x.Email)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Email.Required"))
          .When(x => x.EmailRequired);
      RuleFor(x => x.Email)
          .EmailAddress()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"))
          .When(x => x.EmailRequired);
      RuleFor(x => x.Company)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Company.Required"))
          .When(x => addressSettings.CompanyEnabled && x.CompanyRequired);
      RuleFor(x => x.CountryId)
          .NotNull()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Country.Required"))
          .When(x => addressSettings.CountryEnabled && x.CountryRequired);
      RuleFor(x => x.CountryId)
          .NotEqual(0)
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Country.Required"))
          .When(x => addressSettings.CountryEnabled && x.CountryRequired);
      RuleFor(x => x.County)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.County.Required"))
          .When(x => addressSettings.CountyEnabled && x.CountyRequired);
      RuleFor(x => x.City)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.City.Required"))
          .When(x => addressSettings.CityEnabled && x.CityRequired);
      RuleFor(x => x.Address1)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Address1.Required"))
          .When(x => addressSettings.StreetAddressEnabled && x.StreetAddressRequired);
      RuleFor(x => x.Address2)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Address2.Required"))
          .When(x => addressSettings.StreetAddress2Enabled && x.StreetAddress2Required);
      RuleFor(x => x.ZipPostalCode)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.ZipPostalCode.Required"))
          .When(x => addressSettings.ZipPostalCodeEnabled && x.ZipPostalCodeRequired);
      RuleFor(x => x.PhoneNumber)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.PhoneNumber.Required"))
          .When(x => addressSettings.PhoneEnabled && x.PhoneRequired);
      RuleFor(x => x.FaxNumber)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.FaxNumber.Required"))
          .When(x => addressSettings.FaxEnabled && x.FaxRequired);

      SetDatabaseValidationRules<Address>(mappingEntityAccessor);
   }
}