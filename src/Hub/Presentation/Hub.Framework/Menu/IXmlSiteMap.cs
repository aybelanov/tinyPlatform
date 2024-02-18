﻿using System.Threading.Tasks;

namespace Hub.Web.Framework.Menu;

/// <summary>
/// XML sitemap interface
/// </summary>
public interface IXmlSiteMap
{
   /// <summary>
   /// Root node
   /// </summary>
   SiteMapNode RootNode { get; set; }

   /// <summary>
   /// Load sitemap
   /// </summary>
   /// <param name="physicalPath">Filepath to load a sitemap</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task LoadFromAsync(string physicalPath);
}