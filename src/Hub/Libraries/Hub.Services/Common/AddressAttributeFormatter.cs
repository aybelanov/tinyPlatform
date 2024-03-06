using Hub.Core;
using Hub.Services.Html;
using Hub.Services.Localization;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Common
{
   /// <summary>
   /// Address attribute helper
   /// </summary>
   public partial class AddressAttributeFormatter : IAddressAttributeFormatter
   {
      #region Fields

      private readonly IAddressAttributeParser _addressAttributeParser;
      private readonly IAddressAttributeService _addressAttributeService;
      private readonly IHtmlFormatter _htmlFormatter;
      private readonly ILocalizationService _localizationService;
      private readonly IWorkContext _workContext;

      #endregion

      #region Ctor

      /// <summary>
      /// IoC Ctor
      /// </summary>
      /// <param name="addressAttributeParser"></param>
      /// <param name="addressAttributeService"></param>
      /// <param name="htmlFormatter"></param>
      /// <param name="localizationService"></param>
      /// <param name="workContext"></param>
      public AddressAttributeFormatter(IAddressAttributeParser addressAttributeParser,
          IAddressAttributeService addressAttributeService,
          IHtmlFormatter htmlFormatter,
          ILocalizationService localizationService,
          IWorkContext workContext)
      {
         _addressAttributeParser = addressAttributeParser;
         _addressAttributeService = addressAttributeService;
         _htmlFormatter = htmlFormatter;
         _localizationService = localizationService;
         _workContext = workContext;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Formats attributes
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <param name="separator">Separator</param>
      /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the attributes
      /// </returns>
      public virtual async Task<string> FormatAttributesAsync(string attributesXml,
          string separator = "<br />",
          bool htmlEncode = true)
      {
         var result = new StringBuilder();
         var currentLanguage = await _workContext.GetWorkingLanguageAsync();
         var attributes = await _addressAttributeParser.ParseAddressAttributesAsync(attributesXml);
         for (var i = 0; i < attributes.Count; i++)
         {
            var attribute = attributes[i];
            var valuesStr = _addressAttributeParser.ParseValues(attributesXml, attribute.Id);
            for (var j = 0; j < valuesStr.Count; j++)
            {
               var valueStr = valuesStr[j];
               var formattedAttribute = string.Empty;
               if (!attribute.ShouldHaveValues())
               {

                  //other attributes (textbox, datepicker)
                  formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {valueStr}";
                  //encode (if required)
                  if (htmlEncode)
                     formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);

               }
               else
               {
                  if (int.TryParse(valueStr, out var attributeValueId))
                  {
                     var attributeValue = await _addressAttributeService.GetAddressAttributeValueByIdAsync(attributeValueId);
                     if (attributeValue != null)
                     {
                        formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";
                     }
                     //encode (if required)
                     if (htmlEncode)
                        formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                  }
               }

               if (string.IsNullOrEmpty(formattedAttribute))
                  continue;

               if (i != 0 || j != 0)
                  result.Append(separator);

               result.Append(formattedAttribute);
            }
         }

         return result.ToString();
      }

      #endregion
   }
}