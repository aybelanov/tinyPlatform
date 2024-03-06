using Hub.Core;
using Hub.Services.Html;
using Hub.Services.Localization;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Users;

/// <summary>
/// User attributes formatter
/// </summary>
public partial class UserAttributeFormatter : IUserAttributeFormatter
{
   #region Fields

   private readonly IUserAttributeParser _userAttributeParser;
   private readonly IUserAttributeService _userAttributeService;
   private readonly IHtmlFormatter _htmlFormatter;
   private readonly ILocalizationService _localizationService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   /// <summary> IoC Ctor </summary>
   public UserAttributeFormatter(IUserAttributeParser userAttributeParser,
       IUserAttributeService userAttributeService,
       IHtmlFormatter htmlFormatter,
       ILocalizationService localizationService,
       IWorkContext workContext)
   {
      _userAttributeParser = userAttributeParser;
      _userAttributeService = userAttributeService;
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
   public virtual async Task<string> FormatAttributesAsync(string attributesXml, string separator = "<br />", bool htmlEncode = true)
   {
      var result = new StringBuilder();
      var currentLanguage = await _workContext.GetWorkingLanguageAsync();
      var attributes = await _userAttributeParser.ParseUserAttributesAsync(attributesXml);
      for (var i = 0; i < attributes.Count; i++)
      {
         var attribute = attributes[i];
         var valuesStr = _userAttributeParser.ParseValues(attributesXml, attribute.Id);
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
                  var attributeValue = await _userAttributeService.GetUserAttributeValueByIdAsync(attributeValueId);
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