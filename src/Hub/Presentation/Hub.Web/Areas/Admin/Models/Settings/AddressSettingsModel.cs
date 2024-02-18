using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents an address settings model
/// </summary>
public partial record AddressSettingsModel : BaseAppModel, ISettingsModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CompanyEnabled")]
   public bool CompanyEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CompanyRequired")]
   public bool CompanyRequired { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddressEnabled")]
   public bool StreetAddressEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddressRequired")]
   public bool StreetAddressRequired { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddress2Enabled")]
   public bool StreetAddress2Enabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddress2Required")]
   public bool StreetAddress2Required { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.ZipPostalCodeEnabled")]
   public bool ZipPostalCodeEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.ZipPostalCodeRequired")]
   public bool ZipPostalCodeRequired { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CityEnabled")]
   public bool CityEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CityRequired")]
   public bool CityRequired { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CountyEnabled")]
   public bool CountyEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CountyRequired")]
   public bool CountyRequired { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CountryEnabled")]
   public bool CountryEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StateProvinceEnabled")]
   public bool StateProvinceEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.PhoneEnabled")]
   public bool PhoneEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.PhoneRequired")]
   public bool PhoneRequired { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.FaxEnabled")]
   public bool FaxEnabled { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.FaxRequired")]
   public bool FaxRequired { get; set; }

   #endregion
}