using Hub.Core.Domain.Common;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Users;

/// <summary>
/// Represents a user model
/// </summary>
public partial record UserModel : BaseAppEntityModel, IAclSupportedModel
{
   #region Ctor

   public UserModel()
   {
      AvailableTimeZones = new List<SelectListItem>();
      SendEmail = new SendEmailModel() { SendImmediately = true };
      SendPm = new SendPmModel();

      SelectedUserRoleIds = new List<long>();
      AvailableUserRoles = new List<SelectListItem>();

      AvailableCountries = new List<SelectListItem>();
      AvailableStates = new List<SelectListItem>();
      UserAttributes = new List<UserAttributeModel>();
      UserAddressSearchModel = new UserAddressSearchModel();
      UserDeviceSearchModel = new UserDeviceSearchModel();
      UserActivityLogSearchModel = new UserActivityLogSearchModel();
      UserAssociatedExternalAuthRecordsSearchModel = new UserAssociatedExternalAuthRecordsSearchModel();
   }

   #endregion

   #region Properties

   public bool UsernamesEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.Username")]
   public string Username { get; set; }

   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Admin.Users.Users.Fields.Email")]
   public string Email { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.Password")]
   [DataType(DataType.Password)]
   public string Password { get; set; }

   //form fields & properties
   public bool GenderEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.Gender")]
   public string Gender { get; set; }

   public bool FirstNameEnabled { get; set; }
   [AppResourceDisplayName("Admin.Users.Users.Fields.FirstName")]
   public string FirstName { get; set; }

   public bool LastNameEnabled { get; set; }
   [AppResourceDisplayName("Admin.Users.Users.Fields.LastName")]
   public string LastName { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.FullName")]
   public string FullName { get; set; }

   public bool DateOfBirthEnabled { get; set; }

   [UIHint("DateNullable")]
   [AppResourceDisplayName("Admin.Users.Users.Fields.DateOfBirth")]
   public DateTime? DateOfBirth { get; set; }

   public bool CompanyEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.Company")]
   public string Company { get; set; }

   public bool StreetAddressEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.StreetAddress")]
   public string StreetAddress { get; set; }

   public bool StreetAddress2Enabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.StreetAddress2")]
   public string StreetAddress2 { get; set; }

   public bool ZipPostalCodeEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.ZipPostalCode")]
   public string ZipPostalCode { get; set; }

   public bool CityEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.City")]
   public string City { get; set; }

   public bool CountyEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.County")]
   public string County { get; set; }

   public bool CountryEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.Country")]
   public long CountryId { get; set; }

   public IList<SelectListItem> AvailableCountries { get; set; }

   public bool StateProvinceEnabled { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.StateProvince")]
   public long StateProvinceId { get; set; }

   public IList<SelectListItem> AvailableStates { get; set; }

   public bool PhoneEnabled { get; set; }

   [DataType(DataType.PhoneNumber)]
   [AppResourceDisplayName("Admin.Users.Users.Fields.Phone")]
   public string Phone { get; set; }

   public bool FaxEnabled { get; set; }

   [DataType(DataType.PhoneNumber)]
   [AppResourceDisplayName("Admin.Users.Users.Fields.Fax")]
   public string Fax { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.NewsletterSubscribed")]
   public bool NewsletterSubscribed { get; set; }

   public List<UserAttributeModel> UserAttributes { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.AdminComment")]
   public string AdminComment { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.Active")]
   public bool IsActive { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.Affiliate")]
   public long AffiliateId { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.Affiliate")]
   public string AffiliateName { get; set; }

   //time zone
   [AppResourceDisplayName("Admin.Users.Users.Fields.TimeZoneId")]
   public string TimeZoneId { get; set; }

   public bool AllowUsersToSetTimeZone { get; set; }

   public IList<SelectListItem> AvailableTimeZones { get; set; }

   //registration date
   [AppResourceDisplayName("Admin.Users.Users.Fields.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.LastActivityDate")]
   public DateTime LastActivityDate { get; set; }

   //IP address
   [AppResourceDisplayName("Admin.Users.Users.Fields.IPAddress")]
   public string LastIpAddress { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.LastVisitedPage")]
   public string LastVisitedPage { get; set; }

   //user roles
   [AppResourceDisplayName("Admin.Users.Users.Fields.UserRoles")]
   public string UserRoleNames { get; set; }

   //binding with multi-factor authentication provider
   [AppResourceDisplayName("Admin.Users.Users.Fields.MultiFactorAuthenticationProvider")]
   public string MultiFactorAuthenticationProvider { get; set; }

   public IList<SelectListItem> AvailableUserRoles { get; set; }

   [AppResourceDisplayName("Admin.Users.Users.Fields.UserRoles")]
   public IList<long> SelectedUserRoleIds { get; set; }

   //send email model
   public SendEmailModel SendEmail { get; set; }

   //send PM model
   public SendPmModel SendPm { get; set; }

   //send a private message
   public bool AllowSendingOfPrivateMessage { get; set; }

   //send the welcome message
   public bool AllowSendingOfWelcomeMessage { get; set; }

   //re-send the activation message
   public bool AllowReSendingOfActivationMessage { get; set; }

   //GDPR enabled
   public bool GdprEnabled { get; set; }

   public string AvatarUrl { get; internal set; }

   public UserDeviceSearchModel UserDeviceSearchModel { get; set; }

   public UserAddressSearchModel UserAddressSearchModel { get; set; }

   public UserActivityLogSearchModel UserActivityLogSearchModel { get; set; }

   public UserAssociatedExternalAuthRecordsSearchModel UserAssociatedExternalAuthRecordsSearchModel { get; set; }

   public UserStatisticsModel UserStatisticsModel { get; set; }

   #endregion

   #region Nested classes

   public partial record SendEmailModel : BaseAppModel
   {
      [AppResourceDisplayName("Admin.Users.Users.SendEmail.Subject")]
      public string Subject { get; set; }

      [AppResourceDisplayName("Admin.Users.Users.SendEmail.Body")]
      public string Body { get; set; }

      [AppResourceDisplayName("Admin.Users.Users.SendEmail.SendImmediately")]
      public bool SendImmediately { get; set; }

      [AppResourceDisplayName("Admin.Users.Users.SendEmail.DontSendBeforeDate")]
      [UIHint("DateTimeNullable")]
      public DateTime? DontSendBeforeDate { get; set; }
   }

   public partial record SendPmModel : BaseAppModel
   {
      [AppResourceDisplayName("Admin.Users.Users.SendPM.Subject")]
      public string Subject { get; set; }

      [AppResourceDisplayName("Admin.Users.Users.SendPM.Message")]
      public string Message { get; set; }
   }

   public partial record UserAttributeModel : BaseAppEntityModel
   {
      public UserAttributeModel()
      {
         Values = new List<UserAttributeValueModel>();
      }

      public string Name { get; set; }

      public bool IsRequired { get; set; }

      /// <summary>
      /// Default value for textboxes
      /// </summary>
      public string DefaultValue { get; set; }

      public AttributeControlType AttributeControlType { get; set; }

      public IList<UserAttributeValueModel> Values { get; set; }
   }

   public partial record UserAttributeValueModel : BaseAppEntityModel
   {
      public string Name { get; set; }

      public bool IsPreSelected { get; set; }
   }

   #endregion
}