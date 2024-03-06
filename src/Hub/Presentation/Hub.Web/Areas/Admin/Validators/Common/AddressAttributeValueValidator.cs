using FluentValidation;
using Hub.Core.Domain.Common;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Common;

public partial class AddressAttributeValueValidator : BaseAppValidator<AddressAttributeValueModel>
{
   public AddressAttributeValueValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Values.Fields.Name.Required"));

      SetDatabaseValidationRules<AddressAttributeValue>(mappingEntityAccessor);
   }
}