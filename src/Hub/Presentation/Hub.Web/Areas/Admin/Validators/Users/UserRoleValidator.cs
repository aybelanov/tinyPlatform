using FluentValidation;
using Hub.Core.Domain.Users;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Users;

public partial class UserRoleValidator : BaseAppValidator<UserRoleModel>
{
   public UserRoleValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.UserRoles.Fields.Name.Required"));

      SetDatabaseValidationRules<UserRole>(mappingEntityAccessor);
   }
}