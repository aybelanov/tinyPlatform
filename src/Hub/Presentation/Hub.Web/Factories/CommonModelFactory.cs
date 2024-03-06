using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Hub.Core.Infrastructure;
using Hub.Data.Extensions;
using Hub.Services.Blogs;
using Hub.Services.Common;
using Hub.Services.Directory;
using Hub.Services.Forums;
using Hub.Services.Localization;
using Hub.Services.Media;
using Hub.Services.News;
using Hub.Services.Security;
using Hub.Services.Seo;
using Hub.Services.Themes;
using Hub.Services.Topics;
using Hub.Services.Users;
using Hub.Web.Framework.Themes;
using Hub.Web.Framework.UI;
using Hub.Web.Infrastructure.Cache;
using Hub.Web.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the common models factory
/// </summary>
public partial class CommonModelFactory : ICommonModelFactory
{
   #region Fields

   private readonly BlogSettings _blogSettings;
   private readonly CaptchaSettings _captchaSettings;
   private readonly CommonSettings _commonSettings;
   private readonly AppInfoSettings _appSettings;
   private readonly UserSettings _userSettings;
   private readonly DisplayDefaultFooterItemSettings _displayDefaultFooterItemSettings;
   private readonly DisplayDefaultMenuItemSettings _displayDefaultMenuItemSettings;
   private readonly ForumSettings _forumSettings;
   private readonly IActionContextAccessor _actionContextAccessor;
   private readonly IBlogService _blogService;
   private readonly ICurrencyService _currencyService;
   private readonly IUserService _userService;
   private readonly IForumService _forumService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IHttpContextAccessor _httpContextAccessor;
   private readonly ILanguageService _languageService;
   private readonly ILocalizationService _localizationService;
   private readonly INewsService _newsService;
   private readonly IAppFileProvider _fileProvider;
   private readonly IAppHtmlHelper _appHtmlHelper;
   private readonly IPermissionService _permissionService;
   private readonly IPictureService _pictureService;
   private readonly ISitemapGenerator _sitemapGenerator;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IThemeContext _themeContext;
   private readonly IThemeProvider _themeProvider;
   private readonly ITopicService _topicService;
   private readonly IUrlHelperFactory _urlHelperFactory;
   private readonly IUrlRecordService _urlRecordService;
   private readonly IWebHelper _webHelper;
   private readonly IWorkContext _workContext;
   private readonly LocalizationSettings _localizationSettings;
   private readonly MediaSettings _mediaSettings;
   private readonly NewsSettings _newsSettings;
   private readonly SitemapSettings _sitemapSettings;
   private readonly SitemapXmlSettings _sitemapXmlSettings;
   private readonly AppInfoSettings _appInformationSettings;

   #endregion

   #region Ctor

   public CommonModelFactory(BlogSettings blogSettings,
       CaptchaSettings captchaSettings,
       CommonSettings commonSettings,
       AppInfoSettings appSettings,
       UserSettings userSettings,
       DisplayDefaultFooterItemSettings displayDefaultFooterItemSettings,
       DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
       ForumSettings forumSettings,
       IActionContextAccessor actionContextAccessor,
       IBlogService blogService,
       ICurrencyService currencyService,
       IUserService userService,
       IForumService forumService,
       IGenericAttributeService genericAttributeService,
       IHttpContextAccessor httpContextAccessor,
       ILanguageService languageService,
       ILocalizationService localizationService,
       INewsService newsService,
       IAppFileProvider fileProvider,
       IAppHtmlHelper appHtmlHelper,
       IPermissionService permissionService,
       IPictureService pictureService,
       ISitemapGenerator sitemapGenerator,
       IStaticCacheManager staticCacheManager,
       IThemeContext themeContext,
       IThemeProvider themeProvider,
       ITopicService topicService,
       IUrlHelperFactory urlHelperFactory,
       IUrlRecordService urlRecordService,
       IWebHelper webHelper,
       IWorkContext workContext,
       LocalizationSettings localizationSettings,
       MediaSettings mediaSettings,
       NewsSettings newsSettings,
       SitemapSettings sitemapSettings,
       SitemapXmlSettings sitemapXmlSettings,
       AppInfoSettings appInformationSettings)
   {
      _blogSettings = blogSettings;
      _captchaSettings = captchaSettings;
      _commonSettings = commonSettings;
      _appSettings = appSettings;
      _userSettings = userSettings;
      _displayDefaultFooterItemSettings = displayDefaultFooterItemSettings;
      _displayDefaultMenuItemSettings = displayDefaultMenuItemSettings;
      _forumSettings = forumSettings;
      _actionContextAccessor = actionContextAccessor;
      _blogService = blogService;
      _currencyService = currencyService;
      _userService = userService;
      _forumService = forumService;
      _genericAttributeService = genericAttributeService;
      _httpContextAccessor = httpContextAccessor;
      _languageService = languageService;
      _localizationService = localizationService;
      _newsService = newsService;
      _fileProvider = fileProvider;
      _appHtmlHelper = appHtmlHelper;
      _permissionService = permissionService;
      _pictureService = pictureService;
      _sitemapGenerator = sitemapGenerator;
      _staticCacheManager = staticCacheManager;
      _themeContext = themeContext;
      _themeProvider = themeProvider;
      _topicService = topicService;
      _urlHelperFactory = urlHelperFactory;
      _urlRecordService = urlRecordService;
      _webHelper = webHelper;
      _workContext = workContext;
      _mediaSettings = mediaSettings;
      _localizationSettings = localizationSettings;
      _newsSettings = newsSettings;
      _sitemapSettings = sitemapSettings;
      _sitemapXmlSettings = sitemapXmlSettings;
      _appInformationSettings = appInformationSettings;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Get the number of unread private messages
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the number of private messages
   /// </returns>
   protected virtual async Task<int> GetUnreadPrivateMessagesAsync()
   {
      var result = 0;
      var user = await _workContext.GetCurrentUserAsync();
      if (_forumSettings.AllowPrivateMessages && !await _userService.IsGuestAsync(user))
      {
         var privateMessages = await _forumService.GetAllPrivateMessagesAsync(0, user.Id, false, null, false, string.Empty, 0, 1);

         if (privateMessages.TotalCount > 0)
            result = privateMessages.TotalCount;
      }

      return result;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare the logo model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the logo model
   /// </returns>
   public virtual async Task<LogoModel> PrepareLogoModelAsync()
   {
      var model = new LogoModel
      {
         AppName = _appSettings.Name
      };

      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AppModelCacheDefaults.AppLogoPath
          , await _themeContext.GetWorkingThemeNameAsync(), _webHelper.IsCurrentConnectionSecured());
      model.LogoPath = await _staticCacheManager.GetAsync(cacheKey, async () =>
      {
         var logo = string.Empty;
         var logoPictureId = _appInformationSettings.LogoPictureId;

         if (logoPictureId > 0)
            logo = await _pictureService.GetPictureUrlAsync(logoPictureId, showDefaultPicture: false);

         if (string.IsNullOrEmpty(logo))
         {
            //use default logo
            var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;
            var platfromLocation = _mediaSettings.UseAbsoluteImagePath ? _webHelper.GetAppLocation() : $"{pathBase}/";
            logo = $"{platfromLocation}Themes/{await _themeContext.GetWorkingThemeNameAsync()}/Content/images/logo_gray.svg";
         }

         return logo;
      });

      return model;
   }

   /// <summary>
   /// Prepare the language selector model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the language selector model
   /// </returns>
   public virtual async Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync()
   {
      var availableLanguages = (await _languageService
              .GetAllLanguagesAsync())
              .Select(x => new LanguageModel
              {
                 Id = x.Id,
                 Name = x.Name,
                 FlagImageFileName = x.FlagImageFileName,
              }).ToList();

      var model = new LanguageSelectorModel
      {
         CurrentLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
         AvailableLanguages = availableLanguages,
         UseImages = _localizationSettings.UseImagesForLanguageSelection
      };

      return model;
   }

   /// <summary>
   /// Prepare the currency selector model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the currency selector model
   /// </returns>
   public virtual async Task<CurrencySelectorModel> PrepareCurrencySelectorModelAsync()
   {
      var availableCurrencies = await (await _currencyService
          .GetAllCurrenciesAsync())
          .SelectAwait(async x =>
          {
             //currency char
             var currencySymbol = !string.IsNullOrEmpty(x.DisplayLocale)
                    ? new RegionInfo(x.DisplayLocale).CurrencySymbol
                    : x.CurrencyCode;

             //model
             var currencyModel = new CurrencyModel
             {
                Id = x.Id,
                Name = await _localizationService.GetLocalizedAsync(x, y => y.Name),
                CurrencySymbol = currencySymbol
             };

             return currencyModel;
          }).ToListAsync();

      var model = new CurrencySelectorModel
      {
         CurrentCurrencyId = (await _workContext.GetWorkingCurrencyAsync()).Id,
         AvailableCurrencies = availableCurrencies
      };

      return model;
   }

   /// <summary>
   /// Prepare the header links model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the header links model
   /// </returns>
   public virtual async Task<HeaderLinksModel> PrepareHeaderLinksModelAsync()
   {
      var user = await _workContext.GetCurrentUserAsync();

      var unreadMessageCount = await GetUnreadPrivateMessagesAsync();
      var unreadMessage = string.Empty;
      var alertMessage = string.Empty;
      if (unreadMessageCount > 0)
      {
         unreadMessage = string.Format(await _localizationService.GetResourceAsync("PrivateMessages.TotalUnread"), unreadMessageCount);

         //notifications here
         if (_forumSettings.ShowAlertForPM &&
             !await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.NotifiedAboutNewPrivateMessagesAttribute))
         {
            await _genericAttributeService.SaveAttributeAsync(user, AppUserDefaults.NotifiedAboutNewPrivateMessagesAttribute, true);
            alertMessage = string.Format(await _localizationService.GetResourceAsync("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
         }
      }

      var model = new HeaderLinksModel
      {
         RegistrationType = _userSettings.UserRegistrationType,
         IsAuthenticated = await _userService.IsRegisteredAsync(user),
         HasToClientAcccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessToClient),
         UserName = await _userService.IsRegisteredAsync(user) ? await _userService.FormatUsernameAsync(user) : string.Empty,
         AllowPrivateMessages = await _userService.IsRegisteredAsync(user) && _forumSettings.AllowPrivateMessages,
         UnreadPrivateMessages = unreadMessage,
         AlertMessage = alertMessage,
      };

      return model;
   }

   /// <summary>
   /// Prepare the admin header links model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the admin header links model
   /// </returns>
   public virtual async Task<AdminHeaderLinksModel> PrepareAdminHeaderLinksModelAsync()
   {
      var user = await _workContext.GetCurrentUserAsync();

      var model = new AdminHeaderLinksModel
      {
         ImpersonatedUserName = await _userService.IsRegisteredAsync(user) ? await _userService.FormatUsernameAsync(user) : string.Empty,
         IsUserImpersonated = _workContext.OriginalUserIfImpersonated != null,
         DisplayAdminLink = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel),
         EditPageUrl = _appHtmlHelper.GetEditPageUrl()
      };

      return model;
   }

   /// <summary>
   /// Prepare the social model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the social model
   /// </returns>
   public virtual async Task<SocialModel> PrepareSocialModelAsync()
   {
      var model = new SocialModel
      {
         FacebookLink = _appInformationSettings.FacebookLink,
         TwitterLink = _appInformationSettings.TwitterLink,
         YoutubeLink = _appInformationSettings.YoutubeLink,
         WorkingLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
         NewsEnabled = _newsSettings.Enabled,
      };

      return model;
   }

   /// <summary>
   /// Prepare the footer model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the footer model
   /// </returns>
   public virtual async Task<FooterModel> PrepareFooterModelAsync()
   {
      //footer topics
      var topicModels = await (await _topicService.GetAllTopicsAsync())
              .Where(t => t.IncludeInFooterColumn1 || t.IncludeInFooterColumn2 || t.IncludeInFooterColumn3)
              .SelectAwait(async t => new FooterModel.FooterTopicModel
              {
                 Id = t.Id,
                 Name = await _localizationService.GetLocalizedAsync(t, x => x.Title),
                 SeName = await _urlRecordService.GetSeNameAsync(t),
                 IncludeInFooterColumn1 = t.IncludeInFooterColumn1,
                 IncludeInFooterColumn2 = t.IncludeInFooterColumn2,
                 IncludeInFooterColumn3 = t.IncludeInFooterColumn3
              }).ToListAsync();

      //model
      var model = new FooterModel
      {
         ServiceName = _appSettings.Name,
         WishlistEnabled = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist),
         SitemapEnabled = _sitemapSettings.SitemapEnabled,
         // TODO site search implementation
         SearchEnabled = _commonSettings.SearchEnabled,
         WorkingLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
         BlogEnabled = _blogSettings.Enabled,
         ForumEnabled = _forumSettings.ForumsEnabled,
         NewsEnabled = _newsSettings.Enabled,
         HidePoweredBy = _appInformationSettings.HidePoweredBy,
         IsHomePage = _webHelper.GetAppLocation().Equals(_webHelper.GetThisPageUrl(false), StringComparison.InvariantCultureIgnoreCase),
         Topics = topicModels,
         DisplaySitemapFooterItem = _displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
         DisplayContactUsFooterItem = _displayDefaultFooterItemSettings.DisplayContactUsFooterItem,

         DisplayNewsFooterItem = _displayDefaultFooterItemSettings.DisplayNewsFooterItem,

         DisplayBlogFooterItem = _displayDefaultFooterItemSettings.DisplayBlogFooterItem,

         DisplayForumsFooterItem = _displayDefaultFooterItemSettings.DisplayForumsFooterItem,

         DisplayDocumentationFooterItem = _displayDefaultFooterItemSettings.DisplayDocumentationFooterItem,

         DisplayUserInfoFooterItem = _displayDefaultFooterItemSettings.DisplayUserInfoFooterItem,
         DisplayUserAddressesFooterItem = _displayDefaultFooterItemSettings.DisplayUserAddressesFooterItem,
         DisplayWishlistFooterItem = _displayDefaultFooterItemSettings.DisplayWishlistFooterItem,
         DisplaySearchFooterItem = _displayDefaultFooterItemSettings.DisplaySearchFooterItem
      };

      return model;
   }

   /// <summary>
   /// Prepare the contact us model
   /// </summary>
   /// <param name="model">Contact us model</param>
   /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the contact us model
   /// </returns>
   public virtual async Task<ContactUsModel> PrepareContactUsModelAsync(ContactUsModel model, bool excludeProperties)
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      if (!excludeProperties)
      {
         var user = await _workContext.GetCurrentUserAsync();
         model.Email = user.Email;
         model.FullName = await _userService.GetUserFullNameAsync(user);
      }

      model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
      model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;

      return model;
   }


   /// <summary>
   /// Prepare the sitemap model
   /// </summary>
   /// <param name="pageModel">Sitemap page model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the sitemap model
   /// </returns>
   public virtual async Task<SitemapModel> PrepareSitemapModelAsync(SitemapPageModel pageModel)
   {
      if (pageModel == null)
         throw new ArgumentNullException(nameof(pageModel));

      var language = await _workContext.GetWorkingLanguageAsync();
      var user = await _workContext.GetCurrentUserAsync();
      var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AppModelCacheDefaults.SitemapPageModelKey, language, userRoleIds);

      var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
      {
         //get URL helper
         var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

         var model = new SitemapModel();

         //prepare common items
         var commonGroupTitle = await _localizationService.GetResourceAsync("Sitemap.General");

         //home page
         model.Items.Add(new SitemapModel.SitemapItemModel
         {
            GroupTitle = commonGroupTitle,
            Name = await _localizationService.GetResourceAsync("Homepage"),
            Url = urlHelper.RouteUrl("Homepage")
         });

         //search
         // TODO site search
         //model.Items.Add(new SitemapModel.SitemapItemModel
         //{
         //   GroupTitle = commonGroupTitle,
         //   Name = await _localizationService.GetResourceAsync("Search"),
         //   Url = urlHelper.RouteUrl("SiteSearch")
         //});

         //news
         if (_newsSettings.Enabled)
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
               GroupTitle = commonGroupTitle,
               Name = await _localizationService.GetResourceAsync("News"),
               Url = urlHelper.RouteUrl("NewsArchive")
            });

         //blog
         if (_blogSettings.Enabled)
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
               GroupTitle = commonGroupTitle,
               Name = await _localizationService.GetResourceAsync("Blog"),
               Url = urlHelper.RouteUrl("Blog")
            });

         //forums
         if (_forumSettings.ForumsEnabled)
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
               GroupTitle = commonGroupTitle,
               Name = await _localizationService.GetResourceAsync("Forum.Forums"),
               Url = urlHelper.RouteUrl("Boards")
            });

         //contact us
         model.Items.Add(new SitemapModel.SitemapItemModel
         {
            GroupTitle = commonGroupTitle,
            Name = await _localizationService.GetResourceAsync("ContactUs"),
            Url = urlHelper.RouteUrl("ContactUs")
         });

         //user info
         model.Items.Add(new SitemapModel.SitemapItemModel
         {
            GroupTitle = commonGroupTitle,
            Name = await _localizationService.GetResourceAsync("Account.MyAccount"),
            Url = urlHelper.RouteUrl("UserInfo")
         });

         //at the moment topics are in general category too
         if (_sitemapSettings.SitemapIncludeTopics)
         {
            var topics = (await _topicService.GetAllTopicsAsync())
                   .Where(topic => topic.IncludeInSitemap);

            model.Items.AddRange(await topics.SelectAwait(async topic => new SitemapModel.SitemapItemModel
            {
               GroupTitle = commonGroupTitle,
               Name = await _localizationService.GetLocalizedAsync(topic, x => x.Title),
               Url = urlHelper.RouteUrl("Topic", new { SeName = await _urlRecordService.GetSeNameAsync(topic) })
            }).ToListAsync());
         }

         //blog posts
         if (_sitemapSettings.SitemapIncludeBlogPosts && _blogSettings.Enabled)
         {
            var blogPostsGroupTitle = await _localizationService.GetResourceAsync("Sitemap.BlogPosts");
            var blogPosts = (await _blogService.GetAllBlogPostsAsync())
                   .Where(p => p.IncludeInSitemap);

            model.Items.AddRange(await blogPosts.SelectAwait(async post => new SitemapModel.SitemapItemModel
            {
               GroupTitle = blogPostsGroupTitle,
               Name = post.Title,
               Url = urlHelper.RouteUrl("BlogPost",
                       new { SeName = await _urlRecordService.GetSeNameAsync(post, post.LanguageId, ensureTwoPublishedLanguages: false) })
            }).ToListAsync());
         }

         //news
         if (_sitemapSettings.SitemapIncludeNews && _newsSettings.Enabled)
         {
            var newsGroupTitle = await _localizationService.GetResourceAsync("Sitemap.News");
            var news = await _newsService.GetAllNewsAsync();
            model.Items.AddRange(await news.SelectAwait(async newsItem => new SitemapModel.SitemapItemModel
            {
               GroupTitle = newsGroupTitle,
               Name = newsItem.Title,
               Url = urlHelper.RouteUrl("NewsItem",
                       new { SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) })
            }).ToListAsync());
         }

         return model;
      });

      //prepare model with pagination
      pageModel.PageSize = Math.Max(pageModel.PageSize, _sitemapSettings.SitemapPageSize);
      pageModel.PageNumber = Math.Max(pageModel.PageNumber, 1);

      var pagedItems = new PagedList<SitemapModel.SitemapItemModel>(cachedModel.Items, pageModel.PageNumber - 1, pageModel.PageSize);
      var sitemapModel = new SitemapModel { Items = pagedItems };
      sitemapModel.PageModel.LoadPagedList(pagedItems);

      return sitemapModel;
   }

   /// <summary>
   /// Get the sitemap in XML format
   /// </summary>
   /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the sitemap as string in XML format
   /// </returns>
   public virtual async Task<string> PrepareSitemapXmlAsync(int? id)
   {
      var language = await _workContext.GetWorkingLanguageAsync();
      var user = await _workContext.GetCurrentUserAsync();
      var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AppModelCacheDefaults.SitemapSeoModelKey, id, language, userRoleIds);

      var siteMap = await _staticCacheManager.GetAsync(cacheKey, async () => await _sitemapGenerator.GenerateAsync(id));

      return siteMap;
   }

   /// <summary>
   /// Prepare the platform theme selector model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the platform theme selector model
   /// </returns>
   public virtual async Task<HubThemeSelectorModel> PreparePlatformThemeSelectorModelAsync()
   {
      var model = new HubThemeSelectorModel();

      var currentTheme = await _themeProvider.GetThemeBySystemNameAsync(await _themeContext.GetWorkingThemeNameAsync());
      model.CurrentHubTheme = new HubThemeModel
      {
         Name = currentTheme?.SystemName,
         Title = currentTheme?.FriendlyName
      };

      model.AvailableHubThemes = (await _themeProvider.GetThemesAsync()).Select(x => new HubThemeModel
      {
         Name = x.SystemName,
         Title = x.FriendlyName
      }).ToList();

      return model;
   }

   /// <summary>
   /// Prepare the favicon model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the favicon model
   /// </returns>
   public virtual Task<FaviconAndAppIconsModel> PrepareFaviconAndAppIconsModelAsync()
   {
      var model = new FaviconAndAppIconsModel
      {
         HeadCode = _commonSettings.FaviconAndAppIconsHeadCode
      };

      return Task.FromResult(model);
   }

   /// <summary>
   /// Get robots.txt file
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the robots.txt file as string
   /// </returns>
   public virtual async Task<string> PrepareRobotsTextFileAsync()
   {
      var sb = new StringBuilder();

      //if robots.custom.txt exists, let's use it instead of hard-coded data below
      var robotsFilePath = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.custom.txt");
      if (_fileProvider.FileExists(robotsFilePath))
      {
         //the robots.txt file exists
         var robotsFileContent = await _fileProvider.ReadAllTextAsync(robotsFilePath, Encoding.UTF8);
         sb.Append(robotsFileContent);
      }
      else
      {
         //doesn't exist. Let's generate it (default behavior)
         // TODO refactor
         var disallowPaths = new List<string>
             {
                 "/admin",
                 "/dashboard",
                 "/bin/",
                 "/files/",
                 "/files/exportimport/",
                 "/country/getstatesbycountryid",
                 "/install",
                 "/*?*returnUrl="
             };
         var localizableDisallowPaths = new List<string>
             {
                 "/boards/forumsubscriptions",
                 "/boards/forumwatch",
                 "/boards/postedit",
                 "/boards/postdelete",
                 "/boards/postcreate",
                 "/boards/topicedit",
                 "/boards/topicdelete",
                 "/boards/topiccreate",
                 "/boards/topicmove",
                 "/boards/topicwatch",
                 "/changecurrency",
                 "/changelanguage",
                 "/user/avatar",
                 "/user/activation",
                 "/user/addresses",
                 "/user/changepassword",
                 "/user/checkusernameavailability",
                 "/user/info",
                 "/deletepm",
                 "/emailwishlist",
                 "/eucookielawaccept",
                 "/inboxupdate",
                 "/newsletter/subscriptionactivation",
                 "/passwordrecovery/confirm",
                 "/poll/vote",
                 "/privatemessages",
                 "/search?",
                 "/sendpm",
                 "/sentupdate",
                 "/storeclosed",
                 "/subscribenewsletter",
                 "/topic/authenticate",
                 "/viewpm",
                 "/wishlist",
             };

         const string newLine = "\r\n"; //Environment.NewLine
         sb.Append("User-agent: *");
         sb.Append(newLine);

         //sitemaps
         if (_sitemapXmlSettings.SitemapXmlEnabled)
         {
            sb.AppendFormat("Sitemap: {0}sitemap.xml", _webHelper.GetAppLocation());
            sb.Append(newLine);
         }

         //host
         sb.AppendFormat("Host: {0}", _webHelper.GetAppLocation());
         sb.Append(newLine);

         //usual paths
         foreach (var path in disallowPaths)
         {
            sb.AppendFormat("Disallow: {0}", path);
            sb.Append(newLine);
         }
         //localizable paths (without SEO code)
         foreach (var path in localizableDisallowPaths)
         {
            sb.AppendFormat("Disallow: {0}", path);
            sb.Append(newLine);
         }

         if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
         {
            //URLs are localizable. Append SEO code
            foreach (var language in await _languageService.GetAllLanguagesAsync())
               foreach (var path in localizableDisallowPaths)
               {
                  sb.AppendFormat("Disallow: /{0}{1}", language.UniqueSeoCode, path);
                  sb.Append(newLine);
               }
         }

         //load and add robots.txt additions to the end of file.
         var robotsAdditionsFile = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.additions.txt");
         if (_fileProvider.FileExists(robotsAdditionsFile))
         {
            var robotsFileContent = await _fileProvider.ReadAllTextAsync(robotsAdditionsFile, Encoding.UTF8);
            sb.Append(robotsFileContent);
         }
      }

      return sb.ToString();
   }

   /// <summary>
   /// Prepare top menu model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the op menu model
   /// </returns>
   public virtual async Task<TopMenuModel> PrepareTopMenuModelAsync()
   {
      //top menu topics
      var topicModel = await (await _topicService.GetAllTopicsAsync(onlyIncludedInTopMenu: true))
          .SelectAwait(async t => new TopMenuModel.TopicModel
          {
             Id = t.Id,
             Name = await _localizationService.GetLocalizedAsync(t, x => x.Title),
             SeName = await _urlRecordService.GetSeNameAsync(t)
          }).ToListAsync();

      var model = new TopMenuModel
      {
         Topics = topicModel,
         BlogEnabled = _blogSettings.Enabled,
         ForumEnabled = _forumSettings.ForumsEnabled,
         DisplayHomepageMenuItem = _displayDefaultMenuItemSettings.DisplayHomepageMenuItem,
         DisplaySearchMenuItem = _displayDefaultMenuItemSettings.DisplaySearchMenuItem,
         DisplayUserInfoMenuItem = _displayDefaultMenuItemSettings.DisplayUserInfoMenuItem,
         DisplayBlogMenuItem = _displayDefaultMenuItemSettings.DisplayBlogMenuItem,
         DisplayForumsMenuItem = _displayDefaultMenuItemSettings.DisplayForumsMenuItem,
         DisplayContactUsMenuItem = _displayDefaultMenuItemSettings.DisplayContactUsMenuItem,
         UseAjaxMenu = _commonSettings.UseAjaxLoadMenu
      };

      return model;
   }

   #endregion
}