using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core.Domain.Users;

namespace Hub.Services.Users
{
   /// <summary>
   /// User attribute parser interface
   /// </summary>
   public partial interface IUserAttributeParser
   {
      /// <summary>
      /// Gets selected user attributes
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the selected user attributes
      /// </returns>
      Task<IList<UserAttribute>> ParseUserAttributesAsync(string attributesXml);

      /// <summary>
      /// Get user attribute values
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user attribute values
      /// </returns>
      Task<IList<UserAttributeValue>> ParseUserAttributeValuesAsync(string attributesXml);

      /// <summary>
      /// Gets selected user attribute value
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <param name="userAttributeId">User attribute identifier</param>
      /// <returns>User attribute value</returns>
      IList<string> ParseValues(string attributesXml, long userAttributeId);

      /// <summary>
      /// Adds an attribute
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <param name="ca">User attribute</param>
      /// <param name="value">Value</param>
      /// <returns>Attributes</returns>
      string AddUserAttribute(string attributesXml, UserAttribute ca, string value);

      /// <summary>
      /// Validates user attributes
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the warnings
      /// </returns>
      Task<IList<string>> GetAttributeWarningsAsync(string attributesXml);
   }
}
