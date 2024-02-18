﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Hub.Core;
using Hub.Services.Common;
using Hub.Services.Logging;

namespace Hub.Services.Plugins.Marketplace
{
   /// <summary>
   /// Represents the official feed manager (plugins from tinyPlatform marketplace)
   /// </summary>
   public partial class OfficialFeedManager
   {
      #region Fields

      private readonly ILogger _logger;
      private readonly AppHttpClient _appHttpClient;

      #endregion

      #region Ctor

      /// <summary>
      /// IoC Ctor
      /// </summary>

      public OfficialFeedManager(ILogger logger,
            AppHttpClient appHttpClient)
      {
         _logger = logger;
         _appHttpClient = appHttpClient;
      }

      #endregion

      #region Utilities

      /// <summary>
      /// Get element value
      /// </summary>
      /// <param name="node">XML node</param>
      /// <param name="elementName">Element name</param>
      /// <returns>Value (text)</returns>
      protected virtual string GetElementValue(XmlNode node, string elementName)
      {
         return node?.SelectSingleNode(elementName)?.InnerText;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Get available categories of marketplace extensions
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the result
      /// </returns>
      public virtual async Task<IList<OfficialFeedCategory>> GetCategoriesAsync()
      {
         //load XML
         var xml = new XmlDocument();
         try
         {
            xml.LoadXml(await _appHttpClient.GetExtensionsCategoriesAsync());
         }
         catch (Exception ex)
         {
            await _logger.ErrorAsync("No access to the list of plugins. Website www.tinyplat.com is not available.", ex);
         }

         //get list of categories from the XML
         return xml.SelectNodes(@"//categories/category").Cast<XmlNode>().Select(node => new OfficialFeedCategory
         {
            Id = int.Parse(GetElementValue(node, @"id")),
            ParentCategoryId = int.Parse(GetElementValue(node, @"parentCategoryId")),
            Name = GetElementValue(node, @"name")
         }).ToList();
      }

      /// <summary>
      /// Get available versions of marketplace extensions
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the result
      /// </returns>
      public virtual async Task<IList<OfficialFeedVersion>> GetVersionsAsync()
      {
         //load XML
         var xml = new XmlDocument();
         try
         {
            xml.LoadXml(await _appHttpClient.GetExtensionsVersionsAsync());
         }
         catch (Exception ex)
         {
            await _logger.ErrorAsync("No access to the list of plugins. Website www.tinyplat.com is not available.", ex);
         }

         //get list of versions from the XML
         return xml.SelectNodes(@"//versions/version").Cast<XmlNode>().Select(node => new OfficialFeedVersion
         {
            Id = long.Parse(GetElementValue(node, @"id")),
            Name = GetElementValue(node, @"name")
         }).ToList();
      }

      /// <summary>
      /// Get all plugins
      /// </summary>
      /// <param name="categoryId">Category identifier</param>
      /// <param name="versionId">Version identifier</param>
      /// <param name="price">Price; 0 - all, 10 - free, 20 - paid</param>
      /// <param name="searchTerm">Search term</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the plugins
      /// </returns>
      public virtual async Task<IPagedList<OfficialFeedPlugin>> GetAllPluginsAsync(long categoryId = 0,
          int versionId = 0, int price = 0, string searchTerm = "",
          int pageIndex = 0, int pageSize = int.MaxValue)
      {
         //load XML
         var xml = new XmlDocument();
         try
         {
            xml.LoadXml(await _appHttpClient.GetExtensionsAsync(categoryId, versionId, price, searchTerm, pageIndex, pageSize));
         }
         catch (Exception ex)
         {
            await _logger.ErrorAsync("No access to the list of plugins. Website www.tinyplat.com is not available.", ex);
         }

         //get list of extensions from the XML
         var list = xml.SelectNodes(@"//extensions/extension").Cast<XmlNode>().Select(node => new OfficialFeedPlugin
         {
            Name = GetElementValue(node, @"name"),
            Url = GetElementValue(node, @"url"),
            PictureUrl = GetElementValue(node, @"picture"),
            Category = GetElementValue(node, @"category"),
            SupportedVersions = GetElementValue(node, @"versions"),
            Price = GetElementValue(node, @"price")
         }).ToList();

         _ = int.TryParse(GetElementValue(xml.SelectNodes(@"//totalRecords")?[0], @"value"), out var totalRecords);

         return new PagedList<OfficialFeedPlugin>(list, pageIndex, pageSize, totalRecords);
      }

      #endregion
   }
}