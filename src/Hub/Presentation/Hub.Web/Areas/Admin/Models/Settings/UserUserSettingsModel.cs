using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a user user settings model
/// </summary>
public partial record UserUserSettingsModel : BaseAppModel, ISettingsModel
{
   #region Ctor

   public UserUserSettingsModel()
   {
      UserSettings = new UserSettingsModel();
      AddressSettings = new AddressSettingsModel();
      DateTimeSettings = new DateTimeSettingsModel();
      ExternalAuthenticationSettings = new ExternalAuthenticationSettingsModel();
      MultiFactorAuthenticationSettings = new MultiFactorAuthenticationSettingsModel();
      UserAttributeSearchModel = new UserAttributeSearchModel();
      AddressAttributeSearchModel = new AddressAttributeSearchModel();
   }

   #endregion

   #region Properties

   public UserSettingsModel UserSettings { get; set; }

   public AddressSettingsModel AddressSettings { get; set; }

   public DateTimeSettingsModel DateTimeSettings { get; set; }

   public ExternalAuthenticationSettingsModel ExternalAuthenticationSettings { get; set; }

   public MultiFactorAuthenticationSettingsModel MultiFactorAuthenticationSettings { get; set; }

   public UserAttributeSearchModel UserAttributeSearchModel { get; set; }

   public AddressAttributeSearchModel AddressAttributeSearchModel { get; set; }

   #endregion
}