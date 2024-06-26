﻿namespace Hub.Services.Plugins.Marketplace
{
   /// <summary>
   /// Feed of plugins from tinyplat.com marketplace
   /// </summary>
   public class OfficialFeedPlugin
   {
      /// <summary>
      /// FieName
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// URL
      /// </summary>
      public string Url { get; set; }

      /// <summary>
      /// Picture URL
      /// </summary>
      public string PictureUrl { get; set; }

      /// <summary>
      /// Category
      /// </summary>
      public string Category { get; set; }

      /// <summary>
      /// Supported versions
      /// </summary>
      public string SupportedVersions { get; set; }

      /// <summary>
      /// Price
      /// </summary>
      public string Price { get; set; }
   }
}
