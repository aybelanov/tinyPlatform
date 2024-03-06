using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.TagHelpers.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// Represents a device search model
/// </summary>
public partial record DeviceSearchModel : BaseSearchModel
{
   [AppResourceDisplayName("Admin.Devices.Devices.List.SearchIpAddress")]
   public string SearchIpAddress { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.List.SearchDeviceName")]
   public string SearchDeviceName { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.List.SearchDeviceSystemName")]
   public string SearchDeviceSystemName { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.List.SearchDeviceEnabled")]
   public BooleanNullable SearchDeviceEnabled { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.List.SearchDeviceActive")]
   public BooleanNullable SearchDeviceActive { get; set; }

   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Admin.Devices.Devices.List.SearchUserEmail")]
   public string SearchUserEmail { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.List.SearchUsername")]
   public string SearchUsername { get; set; }

   public bool UsernamesEnabled { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.List.SearchUserCompany")]
   public string SearchUserCompany { get; set; }

   public bool UserCompanyEnabled { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.List.CreatedFrom")]
   [UIHint("DateNullable")]
   public DateTime? CreatedFrom { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.List.CreatedTo")]
   [UIHint("DateNullable")]
   public DateTime? CreatedTo { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.List.UpdatedFrom")]
   [UIHint("DateNullable")]
   public DateTime? UpdatedFrom { get; set; }

   [AppResourceDisplayName("Admin.Devices.Devices.List.UpdatedTo")]
   [UIHint("DateNullable")]
   public DateTime? UpdatedTo { get; set; }
}
