using FluentValidation;
using Hub.Core.Domain.Forums;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Forums;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Forums;

public partial class ForumValidator : BaseAppValidator<ForumModel>
{
   public ForumValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Fields.Name.Required"));
      RuleFor(x => x.ForumGroupId).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Fields.ForumGroupId.Required"));

      SetDatabaseValidationRules<Forum>(mappingEntityAccessor);
   }
}