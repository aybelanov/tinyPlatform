using FluentValidation;
using Hub.Web.Areas.Admin.Models.Topics;
using Hub.Core.Domain.Topics;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Services.Seo;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Topics;

public partial class TopicValidator : BaseAppValidator<TopicModel>
{
   public TopicValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.SeName)
          .Length(0, AppSeoDefaults.ForumTopicLength)
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), AppSeoDefaults.ForumTopicLength);

      RuleFor(x => x.Password)
          .NotEmpty()
          .When(x => x.IsPasswordProtected)
          .WithMessageAwait(localizationService.GetResourceAsync("Validation.Password.IsNotEmpty"));

      SetDatabaseValidationRules<Topic>(mappingEntityAccessor);
   }
}
