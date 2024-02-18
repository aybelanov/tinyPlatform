using FluentValidation;
using Hub.Web.Models.Boards;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Validators.Boards;

public partial class EditForumPostValidator : BaseAppValidator<EditForumPostModel>
{
   public EditForumPostValidator(ILocalizationService localizationService)
   {
      RuleFor(x => x.Text).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Forum.TextCannotBeEmpty"));
   }
}