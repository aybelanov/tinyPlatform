﻿using Hub.Web.Models.Common;
using System.Threading.Tasks;

namespace Hub.Web.Factories
{
   /// <summary>
   /// Represents the interface of the common models factory
   /// </summary>
   public partial interface ICommonModelFactory
   {
      /// <summary>
      /// Prepare the logo model
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the logo model
      /// </returns>
      Task<LogoModel> PrepareLogoModelAsync();

      /// <summary>
      /// Prepare the language selector model
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the language selector model
      /// </returns>
      Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync();

      /// <summary>
      /// Prepare the currency selector model
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the currency selector model
      /// </returns>
      Task<CurrencySelectorModel> PrepareCurrencySelectorModelAsync();

      /// <summary>
      /// Prepare the header links model
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the header links model
      /// </returns>
      Task<HeaderLinksModel> PrepareHeaderLinksModelAsync();

      /// <summary>
      /// Prepare the admin header links model
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the admin header links model
      /// </returns>
      Task<AdminHeaderLinksModel> PrepareAdminHeaderLinksModelAsync();

      /// <summary>
      /// Prepare the social model
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the social model
      /// </returns>
      Task<SocialModel> PrepareSocialModelAsync();

      /// <summary>
      /// Prepare the footer model
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the footer model
      /// </returns>
      Task<FooterModel> PrepareFooterModelAsync();

      /// <summary>
      /// Prepare the contact us model
      /// </summary>
      /// <param name="model">Contact us model</param>
      /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the contact us model
      /// </returns>
      Task<ContactUsModel> PrepareContactUsModelAsync(ContactUsModel model, bool excludeProperties);

      /// <summary>
      /// Prepare the sitemap model
      /// </summary>
      /// <param name="pageModel">Sitemap page model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the sitemap model
      /// </returns>
      Task<SitemapModel> PrepareSitemapModelAsync(SitemapPageModel pageModel);

      /// <summary>
      /// Get the sitemap in XML format
      /// </summary>
      /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the sitemap as string in XML format
      /// </returns>
      Task<string> PrepareSitemapXmlAsync(int? id);

      /// <summary>
      /// Prepare the platform theme selector model
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the platform theme selector model
      /// </returns>
      Task<HubThemeSelectorModel> PreparePlatformThemeSelectorModelAsync();

      /// <summary>
      /// Prepare the favicon model
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the favicon model
      /// </returns>
      Task<FaviconAndAppIconsModel> PrepareFaviconAndAppIconsModelAsync();

      /// <summary>
      /// Get robots.txt file
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the robots.txt file as string
      /// </returns>
      Task<string> PrepareRobotsTextFileAsync();

      /// <summary>
      /// Prepare top menu model
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the op menu model
      /// </returns>
      Task<TopMenuModel> PrepareTopMenuModelAsync();
   }
}
