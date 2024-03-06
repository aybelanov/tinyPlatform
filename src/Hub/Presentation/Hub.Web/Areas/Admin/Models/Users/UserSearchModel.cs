using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user search model
/// </summary>
public partial record UserSearchModel : BaseSearchModel, IAclSupportedModel
{
   #region Ctor

   public UserSearchModel()
   {
      SelectedUserRoleIds = new List<long>();
      AvailableUserRoles = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.Users.Users.List.UserRoles")]
   public IList<long> SelectedUserRoleIds { get; set; }

   public IList<SelectListItem> AvailableUserRoles { get; set; }

   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Admin.Users.Users.List.SearchEmail")]
   public string SearchEmail { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.List.SearchUsername")]
   public string SearchUsername { get; set; }

   public bool UsernamesEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.List.SearchFirstName")]
   public string SearchFirstName { get; set; }
   public bool FirstNameEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.List.SearchLastName")]
   public string SearchLastName { get; set; }
   public bool LastNameEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.List.SearchDateOfBirth")]
   public string SearchDayOfBirth { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.List.SearchDateOfBirth")]
   public string SearchMonthOfBirth { get; set; }

   public bool DateOfBirthEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.List.SearchCompany")]
   public string SearchCompany { get; set; }

   public bool CompanyEnabled { get; set; }

   [DataType(DataType.PhoneNumber)]
   [AppResourceDisplayName("Admin.Users.Users.List.SearchPhone")]
   public string SearchPhone { get; set; }

   public bool PhoneEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.List.SearchZipCode")]
   public string SearchZipPostalCode { get; set; }

   public bool ZipPostalCodeEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.List.SearchIpAddress")]
   public string SearchIpAddress { get; set; }

   public bool AvatarEnabled { get; internal set; }

   #endregion
}