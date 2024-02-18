using FluentValidation;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Core.Domain.Messages;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Messages
{
   public partial class EmailAccountValidator : BaseAppValidator<EmailAccountModel>
   {
      public EmailAccountValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
      {
         RuleFor(x => x.Email).NotEmpty();
         RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));

         RuleFor(x => x.DisplayName).NotEmpty();

         SetDatabaseValidationRules<EmailAccount>(mappingEntityAccessor);
      }
   }
}