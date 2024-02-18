using Hub.Core.Configuration;

namespace Hub.Core.Domain.Common;

 /// <summary>
 /// PPDF settings
 /// </summary>
 public class PdfSettings : ISettings
 {
     /// <summary>
     /// PDF logo picture identifier
     /// </summary>
     public long LogoPictureId { get; set; }

     /// <summary>
     /// Gets or sets whether letter page size is enabled
     /// </summary>
     public bool LetterPageSizeEnabled { get; set; }

     /// <summary>
     /// Gets or sets a value indicating whether to render order notes in PDf reports
     /// </summary>
     public bool RenderOrderNotes { get; set; }

     /// <summary>
     /// Gets or sets the font file name that will be used
     /// </summary>
     public string FontFileName { get; set; }

     /// <summary>
     /// Gets or sets the text that will appear at the bottom of invoices (column 1)
     /// </summary>
     public string InvoiceFooterTextColumn1 { get; set; }

     /// <summary>
     /// Gets or sets the text that will appear at the bottom of invoices (column 1)
     /// </summary>
     public string InvoiceFooterTextColumn2 { get; set; }
 }