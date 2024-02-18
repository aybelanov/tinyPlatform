using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Security
{
   /// <summary>
   /// Represents a permission record model
   /// </summary>
   public partial record PermissionRecordModel : BaseAppModel
   {
      #region Properties

      public string Name { get; set; }

      public string SystemName { get; set; }

      #endregion
   }
}