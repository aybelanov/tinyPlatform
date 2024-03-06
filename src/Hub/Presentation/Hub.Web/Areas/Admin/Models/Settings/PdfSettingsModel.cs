using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a PDF settings model
   /// </summary>
   public partial record PdfSettingsModel : BaseAppModel, ISettingsModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLetterPageSizeEnabled")]
      public bool LetterPageSizeEnabled { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLogo")]
      [UIHint("Picture")]
      public long LogoPictureId { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn1")]
      public string InvoiceFooterTextColumn1 { get; set; }

      [AppResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn2")]
      public string InvoiceFooterTextColumn2 { get; set; }

      #endregion
   }
}