﻿using System.Threading.Tasks;

namespace Hub.Services.Users
{
   /// <summary>
   /// User attribute helper
   /// </summary>
   public partial interface IUserAttributeFormatter
   {
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
      Task<string> FormatAttributesAsync(string attributesXml, string separator = "<br />", bool htmlEncode = true);
   }
}
