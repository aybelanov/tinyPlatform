using FluentValidation;
using Hub.Core.Domain.Users;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;
using Hub.Web.Models.User;

namespace Hub.Web.Validators.User;

public partial class LoginValidator : BaseAppValidator<LoginModel>
{
   public LoginValidator(ILocalizationService localizationService, UserSettings userSettings)
   {
      if (!userSettings.UsernamesEnabled)
      {
         //login by email
         RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Login.Fields.Email.Required"));
         RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
      }
      else
      {
         // login by username
         RuleFor(x => x.Username).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Username.Required"));
      }

      RuleFor(x => x.Password).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Validation.Password.IsNotEmpty"));
   }
}