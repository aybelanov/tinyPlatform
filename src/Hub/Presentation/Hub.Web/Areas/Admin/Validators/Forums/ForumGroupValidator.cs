using FluentValidation;
using Hub.Web.Areas.Admin.Models.Forums;
using Hub.Core.Domain.Forums;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Forums;

public partial class ForumGroupValidator : BaseAppValidator<ForumGroupModel>
{
   public ForumGroupValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Fields.Name.Required"));

      SetDatabaseValidationRules<ForumGroup>(mappingEntityAccessor);
   }
}