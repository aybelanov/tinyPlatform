using FluentValidation;
using Hub.Web.Models.User;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Validators.User
{
   public partial class PasswordRecoveryValidator : BaseAppValidator<PasswordRecoveryModel>
   {
      public PasswordRecoveryValidator(ILocalizationService localizationService)
      {
         RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.PasswordRecovery.Email.Required"));
         RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
      }
   }
}