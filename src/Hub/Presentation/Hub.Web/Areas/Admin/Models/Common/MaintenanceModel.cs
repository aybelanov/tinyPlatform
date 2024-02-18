using System;
using System.ComponentModel.DataAnnotations;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

public partial record MaintenanceModel : BaseAppModel
{
   public MaintenanceModel()
   {
      DeleteGuests = new DeleteGuestsModel();
      DeleteAbandonedDevices = new DeleteAbandonedCartsModel();
      DeleteExportedFiles = new DeleteExportedFilesModel();
      BackupFileSearchModel = new BackupFileSearchModel();
      DeleteAlreadySentQueuedEmails = new DeleteAlreadySentQueuedEmailsModel();
   }

   public DeleteGuestsModel DeleteGuests { get; set; }

   public DeleteAbandonedCartsModel DeleteAbandonedDevices { get; set; }

   public DeleteExportedFilesModel DeleteExportedFiles { get; set; }

   public BackupFileSearchModel BackupFileSearchModel { get; set; }

   public DeleteAlreadySentQueuedEmailsModel DeleteAlreadySentQueuedEmails { get; set; }

   public bool BackupSupported { get; set; }

   #region Nested classes

   public partial record DeleteGuestsModel : BaseAppModel
   {
      [AppResourceDisplayName("Admin.System.Maintenance.DeleteGuests.StartDate")]
      [UIHint("DateNullable")]
      public DateTime? StartDate { get; set; }

      [AppResourceDisplayName("Admin.System.Maintenance.DeleteGuests.EndDate")]
      [UIHint("DateNullable")]
      public DateTime? EndDate { get; set; }

      public int? NumberOfDeletedUsers { get; set; }
   }

   public partial record DeleteAbandonedCartsModel : BaseAppModel
   {
      // TODo delete abandoned devices
      [AppResourceDisplayName("Admin.System.Maintenance.DeleteAbandonedDevices.OlderThan")]
      [UIHint("Date")]
      public DateTime OlderThan { get; set; }

      public int? NumberOfDeletedItems { get; set; }
   }

   public partial record DeleteExportedFilesModel : BaseAppModel
   {
      [AppResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.StartDate")]
      [UIHint("DateNullable")]
      public DateTime? StartDate { get; set; }

      [AppResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.EndDate")]
      [UIHint("DateNullable")]
      public DateTime? EndDate { get; set; }

      public int? NumberOfDeletedFiles { get; set; }
   }

   public partial record DeleteAlreadySentQueuedEmailsModel : BaseAppModel
   {
      [AppResourceDisplayName("Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.StartDate")]
      [UIHint("DateNullable")]
      public DateTime? StartDate { get; set; }

      [AppResourceDisplayName("Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.EndDate")]
      [UIHint("DateNullable")]
      public DateTime? EndDate { get; set; }

      public int? NumberOfDeletedEmails { get; set; }
   }

   #endregion
}
