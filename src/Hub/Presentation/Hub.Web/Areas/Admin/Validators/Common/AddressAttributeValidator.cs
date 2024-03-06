using FluentValidation;
using Hub.Core.Domain.Common;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Common;

public partial class AddressAttributeValidator : BaseAppValidator<AddressAttributeModel>
{
   public AddressAttributeValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Fields.Name.Required"));

      SetDatabaseValidationRules<AddressAttribute>(mappingEntityAccessor);
   }
}