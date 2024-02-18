using System;
using System.Linq;
using FluentValidation;
using Hub.Web.Models.User;
using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Services.Directory;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Validators.User
{
   public partial class UserInfoValidator : BaseAppValidator<UserInfoModel>
   {
      public UserInfoValidator(ILocalizationService localizationService,
          IStateProvinceService stateProvinceService,
          UserSettings userSettings)
      {
         RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Email.Required"));
         RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
         if (userSettings.FirstNameEnabled && userSettings.FirstNameRequired)
            RuleFor(x => x.FirstName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.FirstName.Required"));
         if (userSettings.LastNameEnabled && userSettings.LastNameRequired)
            RuleFor(x => x.LastName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.LastName.Required"));

         if (userSettings.UsernamesEnabled && userSettings.AllowUsersToChangeUsernames)
         {
            RuleFor(x => x.Username).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Username.Required"));
            RuleFor(x => x.Username).IsUsername(userSettings).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Username.NotValid"));
         }

         //form fields
         if (userSettings.CountryEnabled && userSettings.CountryRequired)
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Country.Required"));
         if (userSettings.CountryEnabled &&
             userSettings.StateProvinceEnabled &&
             userSettings.StateProvinceRequired)
            RuleFor(x => x.StateProvinceId).MustAwait(async (x, context) =>
            {
               //does selected country have states?
               var hasStates = (await stateProvinceService.GetStateProvincesByCountryIdAsync(x.CountryId)).Any();
               if (hasStates)
                  //if yes, then ensure that a state is selected
                  if (x.StateProvinceId == 0)
                     return false;

               return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.StateProvince.Required"));
         if (userSettings.DateOfBirthEnabled && userSettings.DateOfBirthRequired)
         {
            //entered?
            RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
            {
               var dateOfBirth = x.ParseDateOfBirth();
               if (!dateOfBirth.HasValue)
                  return false;

               return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.DateOfBirth.Required"));

            //minimum age
            RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
            {
               var dateOfBirth = x.ParseDateOfBirth();
               if (dateOfBirth.HasValue && userSettings.DateOfBirthMinimumAge.HasValue &&
                       CommonHelper.GetDifferenceInYears(dateOfBirth.Value, DateTime.Today) <
                       userSettings.DateOfBirthMinimumAge.Value)
                  return false;

               return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.DateOfBirth.MinimumAge"), userSettings.DateOfBirthMinimumAge);
         }
         if (userSettings.CompanyRequired && userSettings.CompanyEnabled)
            RuleFor(x => x.Company).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Company.Required"));
         if (userSettings.StreetAddressRequired && userSettings.StreetAddressEnabled)
            RuleFor(x => x.StreetAddress).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.StreetAddress.Required"));
         if (userSettings.StreetAddress2Required && userSettings.StreetAddress2Enabled)
            RuleFor(x => x.StreetAddress2).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.StreetAddress2.Required"));
         if (userSettings.ZipPostalCodeRequired && userSettings.ZipPostalCodeEnabled)
            RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.ZipPostalCode.Required"));
         if (userSettings.CountyRequired && userSettings.CountyEnabled)
            RuleFor(x => x.County).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.County.Required"));
         if (userSettings.CityRequired && userSettings.CityEnabled)
            RuleFor(x => x.City).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.City.Required"));
         if (userSettings.PhoneRequired && userSettings.PhoneEnabled)
            RuleFor(x => x.Phone).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Phone.Required"));
         if (userSettings.PhoneEnabled)
            RuleFor(x => x.Phone).IsPhoneNumber(userSettings).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Phone.NotValid"));
         if (userSettings.FaxRequired && userSettings.FaxEnabled)
            RuleFor(x => x.Fax).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Fax.Required"));
      }
   }
}