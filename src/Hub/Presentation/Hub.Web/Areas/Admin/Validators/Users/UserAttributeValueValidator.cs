using FluentValidation;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Core.Domain.Users;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Users;

public partial class UserAttributeValueValidator : BaseAppValidator<UserAttributeValueModel>
{
   public UserAttributeValueValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.UserAttributes.Values.Fields.Name.Required"));

      SetDatabaseValidationRules<UserAttributeValue>(mappingEntityAccessor);
   }
}