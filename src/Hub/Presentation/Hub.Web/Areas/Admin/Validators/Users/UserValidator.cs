using FluentValidation;
using Hub.Core.Domain.Users;
using Hub.Data.Mapping;
using Hub.Services.Directory;
using Hub.Services.Localization;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Validators;
using Shared.Clients.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Validators.Users;

public partial class UserValidator : BaseAppValidator<UserModel>
{
   public UserValidator(UserSettings userSettings,
       IUserService userService,
       ILocalizationService localizationService,
       IMappingEntityAccessor mappingEntityAccessor,
       IStateProvinceService stateProvinceService)
   {
      //ensure that valid email address is entered if Registered role is checked to avoid registered users with empty email address
      RuleFor(x => x.Email)
          .NotEmpty()
          .EmailAddress()
          //.WithMessage("Valid Email is required for user to be in 'Registered' role")
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"))
          //only for registered users
          .WhenAwait(async x => await IsRegisteredUserRoleCheckedAsync(x, userService));

      //form fields
      if (userSettings.CountryEnabled && userSettings.CountryRequired)
         RuleFor(x => x.CountryId)
             .NotEqual(0)
             .WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Country.Required"))
             //only for registered users
             .WhenAwait(async x => await IsRegisteredUserRoleCheckedAsync(x, userService));
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
      if (userSettings.CompanyRequired && userSettings.CompanyEnabled)
         RuleFor(x => x.Company)
             .NotEmpty()
             .WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.Users.Fields.Company.Required"))
             //only for registered users
             .WhenAwait(async x => await IsRegisteredUserRoleCheckedAsync(x, userService));
      if (userSettings.StreetAddressRequired && userSettings.StreetAddressEnabled)
         RuleFor(x => x.StreetAddress)
             .NotEmpty()
             .WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.Users.Fields.StreetAddress.Required"))
             //only for registered users
             .WhenAwait(async x => await IsRegisteredUserRoleCheckedAsync(x, userService));
      if (userSettings.StreetAddress2Required && userSettings.StreetAddress2Enabled)
         RuleFor(x => x.StreetAddress2)
             .NotEmpty()
             .WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.Users.Fields.StreetAddress2.Required"))
             //only for registered users
             .WhenAwait(async x => await IsRegisteredUserRoleCheckedAsync(x, userService));
      if (userSettings.ZipPostalCodeRequired && userSettings.ZipPostalCodeEnabled)
         RuleFor(x => x.ZipPostalCode)
             .NotEmpty()
             .WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.Users.Fields.ZipPostalCode.Required"))
             //only for registered users
             .WhenAwait(async x => await IsRegisteredUserRoleCheckedAsync(x, userService));
      if (userSettings.CityRequired && userSettings.CityEnabled)
         RuleFor(x => x.City)
             .NotEmpty()
             .WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.Users.Fields.City.Required"))
             //only for registered users
             .WhenAwait(async x => await IsRegisteredUserRoleCheckedAsync(x, userService));
      if (userSettings.CountyRequired && userSettings.CountyEnabled)
         RuleFor(x => x.County)
             .NotEmpty()
             .WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.Users.Fields.County.Required"))
             //only for registered users
             .WhenAwait(async x => await IsRegisteredUserRoleCheckedAsync(x, userService));
      if (userSettings.PhoneRequired && userSettings.PhoneEnabled)
         RuleFor(x => x.Phone)
             .NotEmpty()
             .WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.Users.Fields.Phone.Required"))
             //only for registered users
             .WhenAwait(async x => await IsRegisteredUserRoleCheckedAsync(x, userService));
      if (userSettings.FaxRequired && userSettings.FaxEnabled)
         RuleFor(x => x.Fax)
             .NotEmpty()
             .WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.Users.Fields.Fax.Required"))
             //only for registered users
             .WhenAwait(async x => await IsRegisteredUserRoleCheckedAsync(x, userService));

      SetDatabaseValidationRules<User>(mappingEntityAccessor);
   }

   private async Task<bool> IsRegisteredUserRoleCheckedAsync(UserModel model, IUserService userService)
   {
      var allUserRoles = await userService.GetAllUserRolesAsync(true);
      var newUserRoles = new List<UserRole>();
      foreach (var userRole in allUserRoles)
         if (model.SelectedUserRoleIds.Contains(userRole.Id))
            newUserRoles.Add(userRole);

      var isInRegisteredRole = newUserRoles.FirstOrDefault(cr => cr.SystemName == UserDefaults.RegisteredRoleName) != null;
      return isInRegisteredRole;
   }
}