using FluentValidation;
using Hub.Core.Domain.Common;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;
using Hub.Web.Models.Common;

namespace Hub.Web.Validators.Common;

public partial class ContactUsValidator : BaseAppValidator<ContactUsModel>
{
   public ContactUsValidator(ILocalizationService localizationService, CommonSettings commonSettings)
   {
      RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("ContactUs.Email.Required"));
      RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
      RuleFor(x => x.FullName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("ContactUs.FullName.Required"));
      if (commonSettings.SubjectFieldOnContactUsForm)
         RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("ContactUs.Subject.Required"));
      RuleFor(x => x.Enquiry).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("ContactUs.Enquiry.Required"));
   }
}