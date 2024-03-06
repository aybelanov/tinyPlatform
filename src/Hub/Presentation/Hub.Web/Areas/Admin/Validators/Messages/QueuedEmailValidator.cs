using FluentValidation;
using Hub.Core.Domain.Messages;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Messages
{
   public partial class QueuedEmailValidator : BaseAppValidator<QueuedEmailModel>
   {
      public QueuedEmailValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
      {
         RuleFor(x => x.From).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.From.Required"));
         RuleFor(x => x.To).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.To.Required"));

         RuleFor(x => x.SentTries).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.SentTries.Required"))
                                 .InclusiveBetween(0, 99999).WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.SentTries.Range"));

         SetDatabaseValidationRules<QueuedEmail>(mappingEntityAccessor);

      }
   }
}