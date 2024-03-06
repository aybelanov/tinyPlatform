using FluentValidation;
using Hub.Core.Domain.Topics;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Templates;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Templates
{
   public partial class TopicTemplateValidator : BaseAppValidator<TopicTemplateModel>
   {
      public TopicTemplateValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
      {
         RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Topic.Name.Required"));
         RuleFor(x => x.ViewPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Topic.ViewPath.Required"));

         SetDatabaseValidationRules<TopicTemplate>(mappingEntityAccessor);
      }
   }
}