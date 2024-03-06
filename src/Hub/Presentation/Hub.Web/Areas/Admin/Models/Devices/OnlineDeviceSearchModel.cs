using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Devices;

/// <summary>
/// represents a online device model
/// </summary>
public partial record OnlineDeviceSearchModel : BaseSearchModel
{
   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Admin.Devices.Online.List.SearchUserEmail")]
   public string SearchUserEmail { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.List.SearchUserCompany")]
   public string SearchUserCompany { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.List.SearchSystemName")]
   public string SearchSystemName { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.List.SearchIpAddress")]
   public string SearchIpAddress { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.List.SearchLastActivityFrom")]
   [UIHint("DateTimeNullable")]
   public DateTime? SearchLastActivityFrom { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.List.SearchLastActivityTo")]
   [UIHint("DateTimeNullable")]
   public DateTime? SearchLastActivityTo { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.List.SearchOnline")]
   [UIHint("Boolean")]
   public bool SearchOnline { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.List.SearchBeenRecently")]
   public bool SearchBeenRecently { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.List.SearchOffline")]
   public bool SearchOffline { get; set; }

   [AppResourceDisplayName("Admin.Devices.Online.List.SearchNoActivities")]
   public bool SearchNoActivities { get; set; }
}
