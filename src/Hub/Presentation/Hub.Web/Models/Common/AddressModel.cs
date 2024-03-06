using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Models.Common;

public partial record AddressModel : BaseAppEntityModel
{
   public AddressModel()
   {
      AvailableCountries = new List<SelectListItem>();
      AvailableStates = new List<SelectListItem>();
      CustomAddressAttributes = new List<AddressAttributeModel>();
   }

   [AppResourceDisplayName("Address.Fields.FirstName")]
   public string FirstName { get; set; }
   [AppResourceDisplayName("Address.Fields.LastName")]
   public string LastName { get; set; }
   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Address.Fields.Email")]
   public string Email { get; set; }


   public bool CompanyEnabled { get; set; }
   public bool CompanyRequired { get; set; }
   [AppResourceDisplayName("Address.Fields.Company")]
   public string Company { get; set; }

   public bool CountryEnabled { get; set; }
   [AppResourceDisplayName("Address.Fields.Country")]
   public long? CountryId { get; set; }
   [AppResourceDisplayName("Address.Fields.Country")]
   public string CountryName { get; set; }

   public bool StateProvinceEnabled { get; set; }
   [AppResourceDisplayName("Address.Fields.StateProvince")]
   public long? StateProvinceId { get; set; }
   [AppResourceDisplayName("Address.Fields.StateProvince")]
   public string StateProvinceName { get; set; }

   public bool CountyEnabled { get; set; }
   public bool CountyRequired { get; set; }
   [AppResourceDisplayName("Address.Fields.County")]
   public string County { get; set; }

   public bool CityEnabled { get; set; }
   public bool CityRequired { get; set; }
   [AppResourceDisplayName("Address.Fields.City")]
   public string City { get; set; }

   public bool StreetAddressEnabled { get; set; }
   public bool StreetAddressRequired { get; set; }
   [AppResourceDisplayName("Address.Fields.Address1")]
   public string Address1 { get; set; }

   public bool StreetAddress2Enabled { get; set; }
   public bool StreetAddress2Required { get; set; }
   [AppResourceDisplayName("Address.Fields.Address2")]
   public string Address2 { get; set; }

   public bool ZipPostalCodeEnabled { get; set; }
   public bool ZipPostalCodeRequired { get; set; }
   [AppResourceDisplayName("Address.Fields.ZipPostalCode")]
   public string ZipPostalCode { get; set; }

   public bool PhoneEnabled { get; set; }
   public bool PhoneRequired { get; set; }
   [DataType(DataType.PhoneNumber)]
   [AppResourceDisplayName("Address.Fields.PhoneNumber")]
   public string PhoneNumber { get; set; }

   public bool FaxEnabled { get; set; }
   public bool FaxRequired { get; set; }
   [AppResourceDisplayName("Address.Fields.FaxNumber")]
   public string FaxNumber { get; set; }

   public IList<SelectListItem> AvailableCountries { get; set; }
   public IList<SelectListItem> AvailableStates { get; set; }

   public string FormattedCustomAddressAttributes { get; set; }
   public IList<AddressAttributeModel> CustomAddressAttributes { get; set; }
}