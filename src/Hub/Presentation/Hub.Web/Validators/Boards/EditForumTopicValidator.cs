using FluentValidation;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;
using Hub.Web.Models.Boards;

namespace Hub.Web.Validators.Boards;

public partial class EditForumTopicValidator : BaseAppValidator<EditForumTopicModel>
{
   public EditForumTopicValidator(ILocalizationService localizationService)
   {
      RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Forum.TopicSubjectCannotBeEmpty"));
      RuleFor(x => x.Text).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Forum.TextCannotBeEmpty"));
   }
}