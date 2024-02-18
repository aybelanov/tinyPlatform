using FluentValidation;
using Hub.Web.Models.PrivateMessages;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Validators.PrivateMessages;

public partial class SendPrivateMessageValidator : BaseAppValidator<SendPrivateMessageModel>
{
   public SendPrivateMessageValidator(ILocalizationService localizationService)
   {
      RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("PrivateMessages.SubjectCannotBeEmpty"));
      RuleFor(x => x.Message).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("PrivateMessages.MessageCannotBeEmpty"));
   }
}