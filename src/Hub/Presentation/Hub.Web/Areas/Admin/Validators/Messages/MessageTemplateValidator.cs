using FluentValidation;
using Hub.Core.Domain.Messages;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Messages
{
   public partial class MessageTemplateValidator : BaseAppValidator<MessageTemplateModel>
   {
      public MessageTemplateValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
      {
         RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Fields.Subject.Required"));
         RuleFor(x => x.Body).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Fields.Body.Required"));

         SetDatabaseValidationRules<MessageTemplate>(mappingEntityAccessor);
      }
   }
}