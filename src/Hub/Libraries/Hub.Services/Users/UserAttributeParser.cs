﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Hub.Core.Domain.Users;
using Hub.Services.Localization;

namespace Hub.Services.Users
{
   /// <summary>
   /// User attribute parser
   /// </summary>
   public partial class UserAttributeParser : IUserAttributeParser
   {
      #region Fields

      private readonly IUserAttributeService _userAttributeService;
      private readonly ILocalizationService _localizationService;

      #endregion

      #region Ctor

      /// <summary> IoC Ctor </summary>
      public UserAttributeParser(IUserAttributeService userAttributeService,
            ILocalizationService localizationService)
      {
         _userAttributeService = userAttributeService;
         _localizationService = localizationService;
      }

      #endregion

      #region Utilities

      /// <summary>
      /// Gets selected user attribute identifiers
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <returns>Selected user attribute identifiers</returns>
      protected virtual IList<int> ParseUserAttributeIds(string attributesXml)
      {
         var ids = new List<int>();
         if (string.IsNullOrEmpty(attributesXml))
            return ids;

         try
         {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(attributesXml);

            foreach (XmlNode node in xmlDoc.SelectNodes(@"//Attributes/UserAttribute"))
            {
               if (node.Attributes?["ID"] == null)
                  continue;

               var str1 = node.Attributes["ID"].InnerText.Trim();
               if (int.TryParse(str1, out var id))
                  ids.Add(id);
            }
         }
         catch (Exception exc)
         {
            Debug.Write(exc.ToString());
         }

         return ids;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Gets selected user attributes
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the selected user attributes
      /// </returns>
      public virtual async Task<IList<UserAttribute>> ParseUserAttributesAsync(string attributesXml)
      {
         var result = new List<UserAttribute>();
         if (string.IsNullOrEmpty(attributesXml))
            return result;

         var ids = ParseUserAttributeIds(attributesXml);
         foreach (var id in ids)
         {
            var attribute = await _userAttributeService.GetUserAttributeByIdAsync(id);
            if (attribute != null)
               result.Add(attribute);
         }

         return result;
      }

      /// <summary>
      /// Get user attribute values
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user attribute values
      /// </returns>
      public virtual async Task<IList<UserAttributeValue>> ParseUserAttributeValuesAsync(string attributesXml)
      {
         var values = new List<UserAttributeValue>();
         if (string.IsNullOrEmpty(attributesXml))
            return values;

         var attributes = await ParseUserAttributesAsync(attributesXml);
         foreach (var attribute in attributes)
         {
            if (!attribute.ShouldHaveValues())
               continue;

            var valuesStr = ParseValues(attributesXml, attribute.Id);
            foreach (var valueStr in valuesStr)
            {
               if (string.IsNullOrEmpty(valueStr))
                  continue;

               if (!int.TryParse(valueStr, out var id))
                  continue;

               var value = await _userAttributeService.GetUserAttributeValueByIdAsync(id);
               if (value != null)
                  values.Add(value);
            }
         }

         return values;
      }

      /// <summary>
      /// Gets selected user attribute value
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <param name="userAttributeId">User attribute identifier</param>
      /// <returns>User attribute value</returns>
      public virtual IList<string> ParseValues(string attributesXml, long userAttributeId)
      {
         var selectedUserAttributeValues = new List<string>();
         if (string.IsNullOrEmpty(attributesXml))
            return selectedUserAttributeValues;

         try
         {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(attributesXml);

            var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/UserAttribute");
            foreach (XmlNode node1 in nodeList1)
            {
               if (node1.Attributes?["ID"] == null)
                  continue;

               var str1 = node1.Attributes["ID"].InnerText.Trim();

               if (!int.TryParse(str1, out var id))
                  continue;

               if (id != userAttributeId)
                  continue;

               var nodeList2 = node1.SelectNodes(@"UserAttributeValue/Value");
               foreach (XmlNode node2 in nodeList2)
               {
                  var value = node2.InnerText.Trim();
                  selectedUserAttributeValues.Add(value);
               }
            }
         }
         catch (Exception exc)
         {
            Debug.Write(exc.ToString());
         }

         return selectedUserAttributeValues;
      }

      /// <summary>
      /// Adds an attribute
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <param name="ca">User attribute</param>
      /// <param name="value">Value</param>
      /// <returns>Attributes</returns>
      public virtual string AddUserAttribute(string attributesXml, UserAttribute ca, string value)
      {
         var result = string.Empty;
         try
         {
            var xmlDoc = new XmlDocument();
            if (string.IsNullOrEmpty(attributesXml))
            {
               var element1 = xmlDoc.CreateElement("Attributes");
               xmlDoc.AppendChild(element1);
            }
            else
               xmlDoc.LoadXml(attributesXml);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

            XmlElement attributeElement = null;
            //find existing
            var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/UserAttribute");
            foreach (XmlNode node1 in nodeList1)
            {
               if (node1.Attributes?["ID"] == null)
                  continue;

               var str1 = node1.Attributes["ID"].InnerText.Trim();

               if (!int.TryParse(str1, out var id))
                  continue;

               if (id != ca.Id)
                  continue;

               attributeElement = (XmlElement)node1;
               break;
            }

            //create new one if not found
            if (attributeElement == null)
            {
               attributeElement = xmlDoc.CreateElement("UserAttribute");
               attributeElement.SetAttribute("ID", ca.Id.ToString());
               rootElement.AppendChild(attributeElement);
            }

            var attributeValueElement = xmlDoc.CreateElement("UserAttributeValue");
            attributeElement.AppendChild(attributeValueElement);

            var attributeValueValueElement = xmlDoc.CreateElement("Value");
            attributeValueValueElement.InnerText = value;
            attributeValueElement.AppendChild(attributeValueValueElement);

            result = xmlDoc.OuterXml;
         }
         catch (Exception exc)
         {
            Debug.Write(exc.ToString());
         }

         return result;
      }

      /// <summary>
      /// Validates user attributes
      /// </summary>
      /// <param name="attributesXml">Attributes in XML format</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the warnings
      /// </returns>
      public virtual async Task<IList<string>> GetAttributeWarningsAsync(string attributesXml)
      {
         var warnings = new List<string>();

         //ensure it's our attributes
         var attributes1 = await ParseUserAttributesAsync(attributesXml);

         //validate required user attributes (whether they're chosen/selected/entered)
         var attributes2 = await _userAttributeService.GetAllUserAttributesAsync();
         foreach (var a2 in attributes2)
         {
            if (!a2.IsRequired)
               continue;

            var found = false;
            //selected user attributes
            foreach (var a1 in attributes1)
            {
               if (a1.Id != a2.Id)
                  continue;

               var valuesStr = ParseValues(attributesXml, a1.Id);

               found = valuesStr.Any(str1 => !string.IsNullOrEmpty(str1.Trim()));
            }

            if (found)
               continue;

            //if not found
            var notFoundWarning = "Not found";

            warnings.Add(notFoundWarning);
         }

         return warnings;
      }

      #endregion
   }
}