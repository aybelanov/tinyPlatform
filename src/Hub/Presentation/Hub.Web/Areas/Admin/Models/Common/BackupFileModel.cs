using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents a backup file model
/// </summary>
public partial record BackupFileModel : BaseAppModel
{
   #region Properties

   public string Name { get; set; }

   public string Length { get; set; }

   public string Link { get; set; }

   #endregion
}