using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Core.Domain.Users;
using Hub.Data.Mapping;
using Hub.Services.Users;
using Hub.Services.Directory;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;
using Hub.Web.Areas.Admin.Models.Devices;
using Hub.Services.Devices;
using Hub.Core.Domain.Clients;

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