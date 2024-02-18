using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents a backup file list model
/// </summary>
public partial record BackupFileListModel : BasePagedListModel<BackupFileModel>
{
}