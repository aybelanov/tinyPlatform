using FluentValidation;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Messages
{
   public partial class TestMessageTemplateValidator : BaseAppValidator<TestMessageTemplateModel>
   {
      public TestMessageTemplateValidator(ILocalizationService localizationService)
      {
         RuleFor(x => x.SendTo).NotEmpty();
         RuleFor(x => x.SendTo).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
      }
   }
}