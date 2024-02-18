using FluentValidation;
using Hub.Web.Models.User;
using Hub.Core.Domain.Users;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Validators.User
{
   public partial class PasswordRecoveryConfirmValidator : BaseAppValidator<PasswordRecoveryConfirmModel>
   {
      public PasswordRecoveryConfirmValidator(ILocalizationService localizationService, UserSettings userSettings)
      {
         RuleFor(x => x.NewPassword).IsPassword(localizationService, userSettings);
         RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.PasswordRecovery.ConfirmNewPassword.Required"));
         RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessageAwait(localizationService.GetResourceAsync("Account.PasswordRecovery.NewPassword.EnteredPasswordsDoNotMatch"));
      }
   }
}