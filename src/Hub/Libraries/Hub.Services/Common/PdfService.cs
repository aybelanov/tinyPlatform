// RTL Support provided by Credo inc (www.credo.co.il  ||   info@credo.co.il)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Localization;
using Hub.Core.Infrastructure;
using Hub.Services.Configuration;
using Hub.Services.Directory;
using Hub.Services.Helpers;
using Hub.Services.Html;
using Hub.Services.Localization;
using Hub.Services.Media;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Hub.Services.Common;

/// <summary>
/// PDF service
/// </summary>
public partial class PdfService : IPdfService
{
   #region Fields

   private readonly AddressSettings _addressSettings;
   private readonly CurrencySettings _currencySettings;
   private readonly IAddressAttributeFormatter _addressAttributeFormatter;
   private readonly IAddressService _addressService;
   private readonly ICountryService _countryService;
   private readonly ICurrencyService _currencyService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IHtmlFormatter _htmlFormatter;
   private readonly ILanguageService _languageService;
   private readonly ILocalizationService _localizationService;
   private readonly IMeasureService _measureService;
   private readonly IAppFileProvider _fileProvider;
   private readonly IPictureService _pictureService;
   private readonly ISettingService _settingService;
   private readonly IStateProvinceService _stateProvinceService;
   private readonly IWorkContext _workContext;
   private readonly MeasureSettings _measureSettings;
   private readonly PdfSettings _pdfSettings;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="addressSettings"></param>
   /// <param name="currencySettings"></param>
   /// <param name="addressAttributeFormatter"></param>
   /// <param name="addressService"></param>
   /// <param name="countryService"></param>
   /// <param name="currencyService"></param>
   /// <param name="dateTimeHelper"></param>
   /// <param name="htmlFormatter"></param>
   /// <param name="languageService"></param>
   /// <param name="localizationService"></param>
   /// <param name="measureService"></param>
   /// <param name="fileProvider"></param>
   /// <param name="pictureService"></param>
   /// <param name="settingService"></param>
   /// <param name="stateProvinceService"></param>
   /// <param name="workContext"></param>
   /// <param name="measureSettings"></param>
   /// <param name="pdfSettings"></param>
   public PdfService(AddressSettings addressSettings,
       CurrencySettings currencySettings,
       IAddressAttributeFormatter addressAttributeFormatter,
       IAddressService addressService,
       ICountryService countryService,
       ICurrencyService currencyService,
       IDateTimeHelper dateTimeHelper,
       IHtmlFormatter htmlFormatter,
       ILanguageService languageService,
       ILocalizationService localizationService,
       IMeasureService measureService,
       IAppFileProvider fileProvider,
       IPictureService pictureService,
       ISettingService settingService,
       IStateProvinceService stateProvinceService,
       IWorkContext workContext,
       MeasureSettings measureSettings,
       PdfSettings pdfSettings)
   {
      _addressSettings = addressSettings;
      _addressService = addressService;
      _countryService = countryService;
      _currencySettings = currencySettings;
      _addressAttributeFormatter = addressAttributeFormatter;
      _currencyService = currencyService;
      _dateTimeHelper = dateTimeHelper;
      _htmlFormatter = htmlFormatter;
      _languageService = languageService;
      _localizationService = localizationService;
      _measureService = measureService;
      _fileProvider = fileProvider;
      _pictureService = pictureService;
      _settingService = settingService;
      _stateProvinceService = stateProvinceService;
      _workContext = workContext;
      _measureSettings = measureSettings;
      _pdfSettings = pdfSettings;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Get font
   /// </summary>
   /// <returns>Font</returns>
   protected virtual Font GetFont()
   {
      //application supports Unicode characters
      //application uses Free Serif font by default (~/App_Data/Pdf/FreeSerif.ttf file)
      //It was downloaded from http://savannah.gnu.org/projects/freefont
      return GetFont(_pdfSettings.FontFileName);
   }

   /// <summary>
   /// Get font
   /// </summary>
   /// <param name="fontFileName">Font file name</param>
   /// <returns>Font</returns>
   protected virtual Font GetFont(string fontFileName)
   {
      if (fontFileName == null)
         throw new ArgumentNullException(nameof(fontFileName));

      var fontPath = _fileProvider.Combine(_fileProvider.MapPath("~/App_Data/Pdf/"), fontFileName);
      var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
      var font = new Font(baseFont, 10, Font.NORMAL);
      return font;
   }

   /// <summary>
   /// Get font direction
   /// </summary>
   /// <param name="lang">Language</param>
   /// <returns>Font direction</returns>
   protected virtual int GetDirection(Language lang)
   {
      return lang.Rtl ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;
   }

   /// <summary>
   /// Get element alignment
   /// </summary>
   /// <param name="lang">Language</param>
   /// <param name="isOpposite">Is opposite?</param>
   /// <returns>Element alignment</returns>
   protected virtual int GetAlignment(Language lang, bool isOpposite = false)
   {
      //if we need the element to be opposite, like logo etc`.
      if (!isOpposite)
         return lang.Rtl ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;

      return lang.Rtl ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
   }

   /// <summary>
   /// Get PDF cell
   /// </summary>
   /// <param name="resourceKey">Locale</param>
   /// <param name="lang">Language</param>
   /// <param name="font">Font</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the pDF cell
   /// </returns>
   protected virtual async Task<PdfPCell> GetPdfCellAsync(string resourceKey, Language lang, Font font)
   {
      return new PdfPCell(new Phrase(await _localizationService.GetResourceAsync(resourceKey, lang.Id), font));
   }

   /// <summary>
   /// Get PDF cell
   /// </summary>
   /// <param name="text">Text</param>
   /// <param name="font">Font</param>
   /// <returns>PDF cell</returns>
   protected virtual PdfPCell GetPdfCell(object text, Font font)
   {
      return new PdfPCell(new Phrase(text.ToString(), font));
   }

   /// <summary>
   /// Get paragraph
   /// </summary>
   /// <param name="resourceKey">Locale</param>
   /// <param name="lang">Language</param>
   /// <param name="font">Font</param>
   /// <param name="args">Locale arguments</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the paragraph
   /// </returns>
   protected virtual async Task<Paragraph> GetParagraphAsync(string resourceKey, Language lang, Font font, params object[] args)
   {
      return await GetParagraphAsync(resourceKey, string.Empty, lang, font, args);
   }

   /// <summary>
   /// Get paragraph
   /// </summary>
   /// <param name="resourceKey">Locale</param>
   /// <param name="indent">Indent</param>
   /// <param name="lang">Language</param>
   /// <param name="font">Font</param>
   /// <param name="args">Locale arguments</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the paragraph
   /// </returns>
   protected virtual async Task<Paragraph> GetParagraphAsync(string resourceKey, string indent, Language lang, Font font, params object[] args)
   {
      var formatText = await _localizationService.GetResourceAsync(resourceKey, lang.Id);
      return new Paragraph(indent + (args.Any() ? string.Format(formatText, args) : formatText), font);
   }

   /// <summary>
   /// Print footer
   /// </summary>
   /// <param name="pdfSettingsByStore">PDF settings</param>
   /// <param name="pdfWriter">PDF writer</param>
   /// <param name="pageSize">Page size</param>
   /// <param name="lang">Language</param>
   /// <param name="font">Font</param>
   protected virtual void PrintFooter(PdfSettings pdfSettingsByStore, PdfWriter pdfWriter, Rectangle pageSize, Language lang, Font font)
   {
      if (string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1) && string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2))
         return;

      var column1Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1)
          ? new List<string>()
          : pdfSettingsByStore.InvoiceFooterTextColumn1
              .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
              .ToList();
      var column2Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2)
          ? new List<string>()
          : pdfSettingsByStore.InvoiceFooterTextColumn2
              .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
              .ToList();

      if (!column1Lines.Any() && !column2Lines.Any())
         return;

      var totalLines = Math.Max(column1Lines.Count, column2Lines.Count);
      const float margin = 43;

      //if you have really a lot of lines in the footer, then replace 9 with 10 or 11
      var footerHeight = totalLines * 9;
      var directContent = pdfWriter.DirectContent;
      directContent.MoveTo(pageSize.GetLeft(margin), pageSize.GetBottom(margin) + footerHeight);
      directContent.LineTo(pageSize.GetRight(margin), pageSize.GetBottom(margin) + footerHeight);
      directContent.Stroke();

      var footerTable = new PdfPTable(2)
      {
         WidthPercentage = 100f,
         RunDirection = GetDirection(lang)
      };
      footerTable.SetTotalWidth(new float[] { 250, 250 });

      //column 1
      if (column1Lines.Any())
      {
         var column1 = new PdfPCell(new Phrase())
         {
            Border = Rectangle.NO_BORDER,
            HorizontalAlignment = Element.ALIGN_LEFT
         };

         foreach (var footerLine in column1Lines)
         {
            column1.Phrase.Add(new Phrase(footerLine, font));
            column1.Phrase.Add(new Phrase(Environment.NewLine));
         }

         footerTable.AddCell(column1);
      }
      else
      {
         var column = new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER };
         footerTable.AddCell(column);
      }

      //column 2
      if (column2Lines.Any())
      {
         var column2 = new PdfPCell(new Phrase())
         {
            Border = Rectangle.NO_BORDER,
            HorizontalAlignment = Element.ALIGN_LEFT
         };

         foreach (var footerLine in column2Lines)
         {
            column2.Phrase.Add(new Phrase(footerLine, font));
            column2.Phrase.Add(new Phrase(Environment.NewLine));
         }

         footerTable.AddCell(column2);
      }
      else
      {
         var column = new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER };
         footerTable.AddCell(column);
      }

      footerTable.WriteSelectedRows(0, totalLines, pageSize.GetLeft(margin), pageSize.GetBottom(margin) + footerHeight, directContent);
   }


   #endregion

   #region Methods

   #endregion
}