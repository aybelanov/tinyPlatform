using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a media settings model
/// </summary>
public partial record MediaSettingsModel : BaseAppModel, ISettingsModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.Settings.Media.PicturesStoredIntoDatabase")]
   public bool PicturesStoredIntoDatabase { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Media.AvatarPictureSize")]
   public int AvatarPictureSize { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Media.MaximumImageSize")]
   public int MaximumImageSize { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Media.MultipleThumbDirectories")]
   public bool MultipleThumbDirectories { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Media.DefaultImageQuality")]
   public int DefaultImageQuality { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Media.ImportImagesUsingHash")]
   public bool ImportImagesUsingHash { get; set; }

   [AppResourceDisplayName("Admin.Configuration.Settings.Media.DefaultPictureZoomEnabled")]
   public bool DefaultPictureZoomEnabled { get; set; }

   #endregion
}