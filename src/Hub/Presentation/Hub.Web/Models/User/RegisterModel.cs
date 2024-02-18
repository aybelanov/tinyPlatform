﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;
using Hub.Core;

namespace Hub.Web.Models.User;

public partial record RegisterModel : BaseAppModel
{
   public RegisterModel()
   {
      AvailableTimeZones = new List<SelectListItem>();
      AvailableCountries = new List<SelectListItem>();
      AvailableStates = new List<SelectListItem>();
      UserAttributes = new List<UserAttributeModel>();
      GdprConsents = new List<GdprConsentModel>();
   }

   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Account.Fields.Email")]
   public string Email { get; set; }

   public bool EnteringEmailTwice { get; set; }
   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Account.Fields.ConfirmEmail")]
   public string ConfirmEmail { get; set; }

   public bool UsernamesEnabled { get; set; }
   [AppResourceDisplayName("Account.Fields.Username")]
   public string Username { get; set; }

   public bool CheckUsernameAvailabilityEnabled { get; set; }

   [DataType(DataType.Password)]
   [AppResourceDisplayName("Account.Fields.Password")]
   public string Password { get; set; }

   [DataType(DataType.Password)]
   [AppResourceDisplayName("Account.Fields.ConfirmPassword")]
   public string ConfirmPassword { get; set; }

   //form fields & properties
   public bool GenderEnabled { get; set; }
   [AppResourceDisplayName("Account.Fields.Gender")]
   public string Gender { get; set; }

   public bool FirstNameEnabled { get; set; }
   [AppResourceDisplayName("Account.Fields.FirstName")]
   public string FirstName { get; set; }
   public bool FirstNameRequired { get; set; }
   public bool LastNameEnabled { get; set; }
   [AppResourceDisplayName("Account.Fields.LastName")]
   public string LastName { get; set; }
   public bool LastNameRequired { get; set; }

   public bool DateOfBirthEnabled { get; set; }
   [AppResourceDisplayName("Account.Fields.DateOfBirth")]
   public int? DateOfBirthDay { get; set; }
   [AppResourceDisplayName("Account.Fields.DateOfBirth")]
   public int? DateOfBirthMonth { get; set; }
   [AppResourceDisplayName("Account.Fields.DateOfBirth")]
   public int? DateOfBirthYear { get; set; }
   public bool DateOfBirthRequired { get; set; }
   public DateTime? ParseDateOfBirth()
   {
      return CommonHelper.ParseDate(DateOfBirthYear, DateOfBirthMonth, DateOfBirthDay);
   }

   public bool CompanyEnabled { get; set; }
   public bool CompanyRequired { get; set; }
   [AppResourceDisplayName("Account.Fields.Company")]
   public string Company { get; set; }

   public bool StreetAddressEnabled { get; set; }
   public bool StreetAddressRequired { get; set; }
   [AppResourceDisplayName("Account.Fields.StreetAddress")]
   public string StreetAddress { get; set; }

   public bool StreetAddress2Enabled { get; set; }
   public bool StreetAddress2Required { get; set; }
   [AppResourceDisplayName("Account.Fields.StreetAddress2")]
   public string StreetAddress2 { get; set; }

   public bool ZipPostalCodeEnabled { get; set; }
   public bool ZipPostalCodeRequired { get; set; }
   [AppResourceDisplayName("Account.Fields.ZipPostalCode")]
   public string ZipPostalCode { get; set; }

   public bool CityEnabled { get; set; }
   public bool CityRequired { get; set; }
   [AppResourceDisplayName("Account.Fields.City")]
   public string City { get; set; }

   public bool CountyEnabled { get; set; }
   public bool CountyRequired { get; set; }
   [AppResourceDisplayName("Account.Fields.County")]
   public string County { get; set; }

   public bool CountryEnabled { get; set; }
   public bool CountryRequired { get; set; }
   [AppResourceDisplayName("Account.Fields.Country")]
   public long CountryId { get; set; }
   public IList<SelectListItem> AvailableCountries { get; set; }

   public bool StateProvinceEnabled { get; set; }
   public bool StateProvinceRequired { get; set; }
   [AppResourceDisplayName("Account.Fields.StateProvince")]
   public long StateProvinceId { get; set; }
   public IList<SelectListItem> AvailableStates { get; set; }

   public bool PhoneEnabled { get; set; }
   public bool PhoneRequired { get; set; }
   [DataType(DataType.PhoneNumber)]
   [AppResourceDisplayName("Account.Fields.Phone")]
   public string Phone { get; set; }

   public bool FaxEnabled { get; set; }
   public bool FaxRequired { get; set; }
   [DataType(DataType.PhoneNumber)]
   [AppResourceDisplayName("Account.Fields.Fax")]
   public string Fax { get; set; }

   public bool NewsletterEnabled { get; set; }
   [AppResourceDisplayName("Account.Fields.Newsletter")]
   public bool Newsletter { get; set; }

   public bool AcceptPrivacyPolicyEnabled { get; set; }
   public bool AcceptPrivacyPolicyPopup { get; set; }

   //time zone
   [AppResourceDisplayName("Account.Fields.TimeZone")]
   public string TimeZoneId { get; set; }
   public bool AllowUsersToSetTimeZone { get; set; }
   public IList<SelectListItem> AvailableTimeZones { get; set; }

   //EU VAT
   [AppResourceDisplayName("Account.Fields.VatNumber")]
   public string VatNumber { get; set; }
   public bool DisplayVatNumber { get; set; }

   public bool HoneypotEnabled { get; set; }
   public bool DisplayCaptcha { get; set; }

   public IList<UserAttributeModel> UserAttributes { get; set; }

   public IList<GdprConsentModel> GdprConsents { get; set; }
}