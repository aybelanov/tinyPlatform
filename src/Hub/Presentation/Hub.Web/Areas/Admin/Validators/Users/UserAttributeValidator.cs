using FluentValidation;
using Hub.Core.Domain.Users;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Users;

public partial class UserAttributeValidator : BaseAppValidator<UserAttributeModel>
{
   public UserAttributeValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.UserAttributes.Fields.Name.Required"));

      SetDatabaseValidationRules<UserAttribute>(mappingEntityAccessor);
   }
}