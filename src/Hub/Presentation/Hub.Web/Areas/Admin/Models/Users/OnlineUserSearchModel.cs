using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents an online user search model
/// </summary>
public partial record OnlineUserSearchModel : BaseSearchModel
{
   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Admin.Users.Online.List.SearchEmail")]
   public string SearchEmail { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.List.SearchCompany")]
   public string SearchCompany { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.List.SearchIpAddress")]
   public string SearchIpAddress { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.List.SearchUserRoles")]
   public IList<long> SelectedUserRoleIds { get; set; } = new List<long>();
   public IList<SelectListItem> AvailableUserRoles { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.List.SearchLastActivityFrom")]
   [UIHint("DateTimeNullable")]
   public DateTime? SearchLastActivityFrom { get; set; }
   
   [AppResourceDisplayName("Admin.Users.Online.List.SearchLastActivityTo")]
   [UIHint("DateTimeNullable")]
   public DateTime? SearchLastActivityTo { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.List.SearchOnline")]
   public bool SearchOnline { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.List.SearchBeenRecently")]
   public bool SearchBeenRecently { get; set; }

   [AppResourceDisplayName("Admin.Users.Online.List.SearchOffline")]
   public bool SearchOffline { get; set; }
}