using FluentValidation;
using Hub.Core.Domain.Clients;
using Hub.Data.Mapping;
using Hub.Services.Devices;
using Hub.Services.Localization;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Models.Devices;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Devices;

public partial class DeviceValidator : BaseAppValidator<DeviceModel>
{
   public DeviceValidator(DeviceSettings deviceSettings,
       IUserService userService,
       IHubDeviceService deviceService,
       ILocalizationService localizationService,
       IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.OwnerName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Devices.Fields.OwnerId.Required"));
      RuleFor(x => x.SystemName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Devices.Fields.SystemName.Required"));
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Devices.Fields.Name.Required"));

      SetDatabaseValidationRules<Device>(mappingEntityAccessor);
   }
}