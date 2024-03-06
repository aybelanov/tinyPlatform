using Hub.Core;
using Hub.Core.Configuration;
using Hub.Core.Domain;
using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Cms;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Logging;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Polls;
using Hub.Core.Domain.ScheduleTasks;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Seo;
using Hub.Core.Domain.Topics;
using Hub.Core.Domain.Users;
using Hub.Core.Infrastructure;
using Hub.Core.Security;
using Hub.Data;
using Hub.Services.Blogs;
using Hub.Services.Clients;
using Hub.Services.Common;
using Hub.Services.Configuration;
using Hub.Services.Devices;
using Hub.Services.ExportImport;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Media;
using Hub.Services.News;
using Hub.Services.Seo;
using Hub.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Clients.Configuration;
using Shared.Clients.Domain;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hub.Services.Installation;

/// <summary>
/// Installation service
/// </summary>
public partial class InstallationService : IInstallationService
{
   #region Fields

   private readonly AppDbContext _dataProvider;
   private readonly IAppFileProvider _fileProvider;
   private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
   private readonly IRepository<Address> _addressRepository;
   private readonly IRepository<Country> _countryRepository;
   private readonly IRepository<Currency> _currencyRepository;
   private readonly IRepository<User> _userRepository;
   private readonly IRepository<UserRole> _userRoleRepository;
   private readonly IRepository<UserUserRole> _userUserRoleRepository;
   private readonly IRepository<EmailAccount> _emailAccountRepository;
   private readonly IRepository<Language> _languageRepository;
   private readonly IRepository<MeasureDimension> _measureDimensionRepository;
   private readonly IRepository<MeasureWeight> _measureWeightRepository;
   private readonly IRepository<StateProvince> _stateProvinceRepository;
   private readonly IRepository<UrlRecord> _urlRecordRepository;
   private readonly IRepository<TopicTemplate> _topicTemplateRepository;
   private readonly AppSettings _appSettings;
   private readonly IWebHelper _webHelper;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public InstallationService(AppDbContext dataProvider,
       IAppFileProvider fileProvider,
       IRepository<ActivityLogType> activityLogTypeRepository,
       IRepository<Address> addressRepository,
       IRepository<Country> countryRepository,
       IRepository<Currency> currencyRepository,
       IRepository<User> userRepository,
       IRepository<UserRole> userRoleRepository,
       IRepository<UserUserRole> userUserRoleRepository,
       IRepository<EmailAccount> emailAccountRepository,
       IRepository<Language> languageRepository,
       IRepository<MeasureDimension> measureDimensionRepository,
       IRepository<MeasureWeight> measureWeightRepository,
       IRepository<StateProvince> stateProvinceRepository,
       IRepository<UrlRecord> urlRecordRepository,
       IRepository<TopicTemplate> topicTemplateRepository,
       AppSettings appSettings,
       IWebHelper webHelper)
   {
      _dataProvider = dataProvider;
      _fileProvider = fileProvider;
      _activityLogTypeRepository = activityLogTypeRepository;
      _addressRepository = addressRepository;
      _countryRepository = countryRepository;
      _currencyRepository = currencyRepository;
      _userRepository = userRepository;
      _userRoleRepository = userRoleRepository;
      _userUserRoleRepository = userUserRoleRepository;
      _emailAccountRepository = emailAccountRepository;
      _languageRepository = languageRepository;
      _measureDimensionRepository = measureDimensionRepository;
      _measureWeightRepository = measureWeightRepository;
      _stateProvinceRepository = stateProvinceRepository;
      _urlRecordRepository = urlRecordRepository;
      _topicTemplateRepository = topicTemplateRepository;
      _appSettings = appSettings;
      _webHelper = webHelper;
   }

   #endregion

   #region Utilities

   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task<T> InsertInstallationDataAsync<T>(T entity) where T : BaseEntity
   {
      _dataProvider.Add(entity);
      await _dataProvider.SaveChangesAsync();
      return entity;
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InsertInstallationDataAsync<T>(params T[] entities) where T : BaseEntity
   {
      await _dataProvider.BulkInsertAsync(entities);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InsertInstallationDataAsync<T>(IList<T> entities) where T : BaseEntity
   {
      if (!entities.Any())
         return;

      await InsertInstallationDataAsync(entities.ToArray());
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task UpdateInstallationDataAsync<T>(T entity) where T : BaseEntity
   {
      _dataProvider.Update(entity);
      await _dataProvider.SaveChangesAsync();
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task UpdateInstallationDataAsync<T>(IList<T> entities) where T : BaseEntity
   {
      if (!entities.Any())
         return;

      await _dataProvider.BulkUpdateAsync(entities);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task<string> ValidateSeNameAsync<T>(T entity, string seName) where T : BaseEntity
   {
      //duplicate of ValidateSeName method of \App.Services\Seo\UrlRecordService.cs (we cannot inject it here)
      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      //validation
      var okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
      seName = seName.Trim().ToLowerInvariant();

      var sb = new StringBuilder();
      foreach (var c in seName.ToCharArray())
      {
         var c2 = c.ToString();
         if (okChars.Contains(c2))
            sb.Append(c2);
      }

      seName = sb.ToString();
      seName = seName.Replace(" ", "-");
      while (seName.Contains("--"))
         seName = seName.Replace("--", "-");
      while (seName.Contains("__"))
         seName = seName.Replace("__", "_");

      //max length
      seName = CommonHelper.EnsureMaximumLength(seName, AppSeoDefaults.SearchEngineNameLength);

      //ensure this sename is not reserved yet
      var i = 2;
      var tempSeName = seName;
      while (true)
      {
         //check whether such slug already exists (and that is not the current entity)

         var query = from ur in _urlRecordRepository.Table
                     where tempSeName != null && ur.Slug == tempSeName
                     select ur;
         var urlRecord = await query.FirstOrDefaultAsync();

         var entityName = entity.GetType().Name;
         var reserved = urlRecord != null && !(urlRecord.EntityId == entity.Id && urlRecord.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
         if (!reserved)
            break;

         tempSeName = $"{seName}-{i}";
         i++;
      }

      seName = tempSeName;

      return seName;
   }

   /// <summary>
   /// Gets the sample path (picture and other resources)
   /// </summary>
   /// <returns></returns>
   protected virtual string GetSamplesPath()
   {
      return _fileProvider.GetAbsolutePath(AppInstallationDefaults.SampleImagesPath);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected async Task InstallAppInformationAsync()
   {
      // set some  information about application owner
      var appInfo = new AppInfoSettings()
      {
         LogoPictureId = 0,
         AllowUserToSelectTheme = true,
         CompanyAddress = "your company country, state, zip, street, etc",
         CompanyName = "Your company name.",
         CompanyPhoneNumber = "+65 123-456-78",
         CompanyVat = "1234567890",
         CompanyWebsiteUrl = _webHelper.GetAppLocation(),
         SslEnabled = _webHelper.IsCurrentConnectionSecured(),
         Hosts = "yourplatform.com,www.yourplatform.com",
         DefaultAppTheme = "DefautTheme",
         DisplayEuCookieLawWarning = true,
         FacebookLink = "https://facebook.com",
         GitHubLink = "https://github.com",
         HidePoweredBy = false,
         Name = "tinyPlatform",
         PlatformClosed = false,
         OkLink = "https://ok.ru",
         VkLink = "https://vk.com",
         RutubeLink = "https://rutube.ru",
         TelegramLink = "https://web.telegram.org",
         TwitterLink = "https://twitter.com",
         YoutubeLink = "https://youtube.com"
      };

      var settingService = EngineContext.Current.Resolve<ISettingService>();
      await settingService.SaveSettingAsync(appInfo);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallMeasuresAsync(RegionInfo regionInfo)
   {
      var isMetric = regionInfo?.IsMetric ?? false;

      var measureDimensions = new List<MeasureDimension>
      {
         new MeasureDimension
         {
            Name = "inch(es)",
            SystemKeyword = "inches",
            Ratio = isMetric ? 39.3701M : 1M,
            DisplayOrder = isMetric ? 1 : 0
         },
         new MeasureDimension
         {
            Name = "feet",
            SystemKeyword = "feet",
            Ratio = isMetric ? 3.28084M : 0.08333333M,
            DisplayOrder = isMetric ? 1 : 0
         },
         new MeasureDimension
         {
            Name = "meter(s)",
            SystemKeyword = "meters",
            Ratio = isMetric ? 1M : 0.0254M,
            DisplayOrder = isMetric ? 0 : 1
         },
         new MeasureDimension
         {
            Name = "millimetre(s)",
            SystemKeyword = "millimetres",
            Ratio = isMetric ? 1000M : 25.4M,
            DisplayOrder = isMetric ? 0 : 1
         }
      };

      await InsertInstallationDataAsync(measureDimensions);

      var measureWeights = new List<MeasureWeight>
      {
         new MeasureWeight
         {
            Name = "ounce(s)",
            SystemKeyword = "ounce",
            Ratio = isMetric ? 35.274M : 16M,
            DisplayOrder = isMetric ? 1 : 0
         },
         new MeasureWeight
         {
            Name = "lb(s)",
            SystemKeyword = "lb",
            Ratio = isMetric ? 2.20462M : 1M,
            DisplayOrder = isMetric ? 1 : 0
         },
         new MeasureWeight
         {
            Name = "kg(s)",
            SystemKeyword = "kg",
            Ratio = isMetric ? 1M : 0.45359237M,
            DisplayOrder = isMetric ? 0 : 1
         },
         new MeasureWeight
         {
            Name = "gram(s)",
            SystemKeyword = "grams",
            Ratio = isMetric ? 1000M : 453.59237M,
            DisplayOrder = isMetric ? 0 : 1
         }
      };

      await InsertInstallationDataAsync(measureWeights);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallLanguagesAsync(CultureInfo cultureInfo, RegionInfo regionInfo)
   {
      var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

      var localeDirectory = _fileProvider.MapPath(AppInstallationDefaults.LocalizationResourcesPath);
      var allLocaleFiles = _fileProvider.GetFiles(localeDirectory);

      // default localization
      var defaultLocaleFile = allLocaleFiles.Where(x => new Regex(AppInstallationDefaults.DefaultLocalizationFilePattern).IsMatch(Path.GetFileName(x))).First();
      var defaultLanguageCulture = new Regex(AppInstallationDefaults.DefaultLocalizationFilePattern).Match(Path.GetFileName(defaultLocaleFile)).Groups[1].Value;
      var defaultCulture = new CultureInfo(defaultLanguageCulture);

      var defaultLanguage = new Language
      {
         Name = defaultCulture.TwoLetterISOLanguageName.ToUpperInvariant(),
         LanguageCulture = defaultCulture.Name,
         UniqueSeoCode = defaultCulture.TwoLetterISOLanguageName,
         FlagImageFileName = $"{defaultCulture.Name.ToLowerInvariant()[^2..]}.png",
         Rtl = defaultCulture.TextInfo.IsRightToLeft,
         Published = true,
         DisplayOrder = 1
      };
      await InsertInstallationDataAsync(defaultLanguage);

      //Install locale resources for default culture
      using var streamReader = new StreamReader(defaultLocaleFile);
      await localizationService.ImportResourcesFromXmlAsync(defaultLanguage, streamReader);

      // additional locale resources
      var localeFiles = allLocaleFiles.Where(x => new Regex(AppInstallationDefaults.LocalizationFilePattern).IsMatch(Path.GetFileName(x)));
      try
      {
         foreach (var filePath in localeFiles)
         {
            var cultureName = new Regex(AppInstallationDefaults.LocalizationFilePattern).Match(Path.GetFileName(filePath)).Groups[1].Value;
            var localeCulture = new CultureInfo(cultureName);
            var localeRegion = new RegionInfo(cultureName);

            var language = new Language
            {
               Name = localeCulture.TwoLetterISOLanguageName.ToUpperInvariant(),
               LanguageCulture = localeCulture.Name,
               UniqueSeoCode = localeCulture.TwoLetterISOLanguageName,
               FlagImageFileName = $"{localeRegion.TwoLetterISORegionName.ToLowerInvariant()}.png",
               Rtl = localeCulture.TextInfo.IsRightToLeft,
               Published = true,
               DisplayOrder = 1
            };

            await InsertInstallationDataAsync(language);

            using var streamReader2 = new StreamReader(filePath);
            await localizationService.ImportResourcesFromXmlAsync(language, streamReader2);
         }
      }
      catch { }
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallCurrenciesAsync(CultureInfo cultureInfo, RegionInfo regionInfo)
   {
      //set some currencies with a rate against the USD
      var defaultCurrencies = new List<string>() { "USD", "AUD", "GBP", "CAD", "CNY", "EUR", "HKD", "JPY", "RUB", "SEK", "INR" };
      var currencies = new List<Currency>
      {
         new Currency
         {
            Name = "US Dollar",
            CurrencyCode = "USD",
            Rate = 1,
            DisplayLocale = "en-US",
            CustomFormatting = string.Empty,
            Published = true,
            DisplayOrder = 1,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding001
         },
         new Currency
         {
            Name = "Australian Dollar",
            CurrencyCode = "AUD",
            Rate = 1.34M,
            DisplayLocale = "en-AU",
            CustomFormatting = string.Empty,
            Published = false,
            DisplayOrder = 2,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding001
         },
         new Currency
         {
            Name = "British Pound",
            CurrencyCode = "GBP",
            Rate = 0.75M,
            DisplayLocale = "en-GB",
            CustomFormatting = string.Empty,
            Published = false,
            DisplayOrder = 3,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding001
         },
         new Currency
         {
            Name = "Canadian Dollar",
            CurrencyCode = "CAD",
            Rate = 1.32M,
            DisplayLocale = "en-CA",
            CustomFormatting = string.Empty,
            Published = false,
            DisplayOrder = 4,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding001
         },
         new Currency
         {
            Name = "Chinese Yuan Renminbi",
            CurrencyCode = "CNY",
            Rate = 6.43M,
            DisplayLocale = "zh-CN",
            CustomFormatting = string.Empty,
            Published = false,
            DisplayOrder = 5,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding001
         },
         new Currency
         {
            Name = "Euro",
            CurrencyCode = "EUR",
            Rate = 0.86M,
            DisplayLocale = string.Empty,
            CustomFormatting = $"{"\u20ac"}0.00", //euro symbol
            Published = false,
            DisplayOrder = 6,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding001
         },
         new Currency
         {
            Name = "Hong Kong Dollar",
            CurrencyCode = "HKD",
            Rate = 7.84M,
            DisplayLocale = "zh-HK",
            CustomFormatting = string.Empty,
            Published = false,
            DisplayOrder = 7,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding001
         },
         new Currency
         {
            Name = "Japanese Yen",
            CurrencyCode = "JPY",
            Rate = 110.45M,
            DisplayLocale = "ja-JP",
            CustomFormatting = string.Empty,
            Published = false,
            DisplayOrder = 8,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding001
         },
         new Currency
         {
            Name = "Russian Rouble",
            CurrencyCode = "RUB",
            Rate = 63.25M,
            DisplayLocale = "ru-RU",
            CustomFormatting = string.Empty,
            Published = false,
            DisplayOrder = 9,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding001
         },
         new Currency
         {
            Name = "Swedish Krona",
            CurrencyCode = "SEK",
            Rate = 8.80M,
            DisplayLocale = "sv-SE",
            CustomFormatting = string.Empty,
            Published = false,
            DisplayOrder = 10,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding1
         },
         new Currency
         {
            Name = "Indian Rupee",
            CurrencyCode = "INR",
            Rate = 68.03M,
            DisplayLocale = "en-IN",
            CustomFormatting = string.Empty,
            Published = false,
            DisplayOrder = 12,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            RoundingType = RoundingType.Rounding001
         }
      };

      //set additional currency
      if (cultureInfo != null && regionInfo != null)
      {
         if (!defaultCurrencies.Contains(regionInfo.ISOCurrencySymbol))
         {
            currencies.Add(new Currency
            {
               Name = regionInfo.CurrencyEnglishName,
               CurrencyCode = regionInfo.ISOCurrencySymbol,
               Rate = 1,
               DisplayLocale = cultureInfo.Name,
               CustomFormatting = string.Empty,
               Published = true,
               DisplayOrder = 0,
               CreatedOnUtc = DateTime.UtcNow,
               UpdatedOnUtc = DateTime.UtcNow,
               RoundingType = RoundingType.Rounding001
            });
         }

         foreach (var currency in currencies.Where(currency => currency.CurrencyCode == regionInfo.ISOCurrencySymbol))
         {
            currency.Published = true;
            currency.DisplayOrder = 0;
         }
      }

      await InsertInstallationDataAsync(currencies);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallCountriesAndStatesAsync()
   {
      var countries = ISO3166.GetCollection().Select(country => new Country
      {
         Name = country.Name,
         AllowsBilling = true,
         AllowsShipping = true,
         TwoLetterIsoCode = country.Alpha2,
         ThreeLetterIsoCode = country.Alpha3,
         NumericIsoCode = country.NumericCode,
         SubjectToVat = country.SubjectToVat,
         DisplayOrder = country.NumericCode == 840 ? 1 : 100,
         Published = true
      }).ToList();

      await InsertInstallationDataAsync(countries.ToArray());

      //Import states for all countries
      var directoryPath = _fileProvider.MapPath(AppInstallationDefaults.LocalizationResourcesPath);
      var pattern = "*.txt";

      //we use different scope to prevent creating wrong settings in DI, because the settings data not exists yet
      var stateProvincies = new List<StateProvince>();
      var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
      using var scope = serviceScopeFactory.CreateScope();
      {
         var importManager = EngineContext.Current.Resolve<IImportManager>(scope);
         foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
         {
            await using var stream = new FileStream(filePath, FileMode.Open);
            {
               var count = 0;
               using (var reader = new StreamReader(stream))
               {
                  while (!reader.EndOfStream)
                  {
                     var line = await reader.ReadLineAsync();
                     if (string.IsNullOrWhiteSpace(line))
                        continue;
                     var tmp = line.Split(',');

                     if (tmp.Length != 5)
                        throw new AppException("Wrong file format");

                     //parse
                     var countryTwoLetterIsoCode = tmp[0].Trim();
                     var name = tmp[1].Trim();
                     var abbreviation = tmp[2].Trim();
                     var published = bool.Parse(tmp[3].Trim());
                     var displayOrder = int.Parse(tmp[4].Trim());

                     var country = countries.FirstOrDefault(x => x.TwoLetterIsoCode == countryTwoLetterIsoCode);
                     if (country == null)
                     {
                        //country cannot be loaded. skip
                        continue;
                     }

                     stateProvincies.Add(new StateProvince()
                     {
                        CountryId = country.Id,
                        Name = name,
                        Abbreviation = abbreviation,
                        Published = published,
                        DisplayOrder = displayOrder
                     });

                     count++;
                  }
               }
            }
         }
      }

      await InsertInstallationDataAsync(stateProvincies);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallSampleUsersAsync()
   {
      var pictureService = EngineContext.Current.Resolve<IPictureService>();
      var sampleImagesPath = GetSamplesPath();

      var secondUserEmail = "ryan_lindgren@application.com";
      var secondUser = new User
      {
         UserGuid = Guid.NewGuid(),
         Email = secondUserEmail,
         Username = secondUserEmail.Split('@')[0],
         //Username = "IgorChemylab1",
         AvatarPictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "user_2.png")), MimeTypes.ImagePng, await pictureService.GetPictureSeNameAsync("Users"))).Id,
         IsActive = true,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
      };
      var defaultSecondUserAddress = await InsertInstallationDataAsync(
      new Address
      {
         FirstName = "Ryan",
         LastName = "Lindgren",
         PhoneNumber = "87654321",
         Email = secondUserEmail,
         FaxNumber = string.Empty,
         Company = "Steve Company",
         Address1 = "750 Bel Air Rd.",
         Address2 = string.Empty,
         City = "Los Angeles",
         StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California")?.Id,
         CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
         ZipPostalCode = "90077",
         CreatedOnUtc = DateTime.UtcNow
      });

      secondUser.BillingAddressId = defaultSecondUserAddress.Id;
      secondUser.ShippingAddressId = defaultSecondUserAddress.Id;

      await InsertInstallationDataAsync(secondUser);

      await InsertInstallationDataAsync(new UserAddress { UserId = secondUser.Id, AddressId = defaultSecondUserAddress.Id });
      //await InsertInstallationDataAsync(userRoles.Select(x=> new UserUserRole() { UserId = secondUser.Id, UserRoleId = x.Id }).ToList());

      //set default user name
      await InsertInstallationDataAsync(new GenericAttribute
      {
         EntityId = secondUser.Id,
         Key = AppUserDefaults.FirstNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultSecondUserAddress.FirstName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      },
      new GenericAttribute
      {
         EntityId = secondUser.Id,
         Key = AppUserDefaults.LastNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultSecondUserAddress.LastName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      });

      //set user password
      await InsertInstallationDataAsync(new UserPassword
      {
         UserId = secondUser.Id,
         Password = "Mnbvcxz1",
         PasswordFormat = PasswordFormat.Clear,
         PasswordSalt = string.Empty,
         CreatedOnUtc = DateTime.UtcNow
      });

      //third user
      var thirdUserEmail = "li_van@application.com";
      var thirdUser = new User
      {
         UserGuid = Guid.NewGuid(),
         Email = thirdUserEmail,
         Username = thirdUserEmail.Split('@')[0],
         //Username = "physicist123",
         AvatarPictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "user_3.png")), MimeTypes.ImagePng, await pictureService.GetPictureSeNameAsync("Users"))).Id,
         IsActive = true,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
      };

      var defaultThirdUserAddress = await InsertInstallationDataAsync(
      new Address
      {
         FirstName = "Li",
         LastName = "Van",
         PhoneNumber = "111222333",
         Email = thirdUserEmail,
         FaxNumber = string.Empty,
         Company = "Holmes Company",
         Address1 = "221B Baker Street",
         Address2 = string.Empty,
         City = "London",
         CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR")?.Id,
         ZipPostalCode = "NW1 6XE",
         CreatedOnUtc = DateTime.UtcNow
      });

      thirdUser.BillingAddressId = defaultThirdUserAddress.Id;
      thirdUser.ShippingAddressId = defaultThirdUserAddress.Id;

      await InsertInstallationDataAsync(thirdUser);

      await InsertInstallationDataAsync(new UserAddress { UserId = thirdUser.Id, AddressId = defaultThirdUserAddress.Id });
      //await InsertInstallationDataAsync(userRoles.Select(x => new UserUserRole() { UserId = thirdUser.Id, UserRoleId = x.Id }).ToList());

      //set default user name
      await InsertInstallationDataAsync(new GenericAttribute
      {
         EntityId = thirdUser.Id,
         Key = AppUserDefaults.FirstNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultThirdUserAddress.FirstName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      },
      new GenericAttribute
      {
         EntityId = thirdUser.Id,
         Key = AppUserDefaults.LastNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultThirdUserAddress.LastName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      });

      //set user password
      await InsertInstallationDataAsync(new UserPassword
      {
         UserId = thirdUser.Id,
         Password = "Mnbvcxz1",
         PasswordFormat = PasswordFormat.Clear,
         PasswordSalt = string.Empty,
         CreatedOnUtc = DateTime.UtcNow
      });

      //fourth user
      var fourthUserEmail = "samantha_pan@application.com";
      var fourthUser = new User
      {
         UserGuid = Guid.NewGuid(),
         Email = fourthUserEmail,
         Username = fourthUserEmail.Split('@')[0],
         //Username = "samMagistr",
         AvatarPictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "user_4.png")), MimeTypes.ImagePng, await pictureService.GetPictureSeNameAsync("Users"))).Id,
         IsActive = true,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
      };
      var defaultFourthUserAddress = await InsertInstallationDataAsync(
      new Address
      {
         FirstName = "Samantha",
         LastName = "Pan",
         PhoneNumber = "369258147",
         Email = fourthUserEmail,
         FaxNumber = string.Empty,
         Company = "Pan Company",
         Address1 = "St Katharine’s West 16",
         Address2 = string.Empty,
         City = "St Andrews",
         CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR")?.Id,
         ZipPostalCode = "KY16 9AX",
         CreatedOnUtc = DateTime.UtcNow
      });

      fourthUser.BillingAddressId = defaultFourthUserAddress.Id;
      fourthUser.ShippingAddressId = defaultFourthUserAddress.Id;

      await InsertInstallationDataAsync(fourthUser);

      await InsertInstallationDataAsync(new UserAddress { UserId = fourthUser.Id, AddressId = defaultFourthUserAddress.Id });
      //await InsertInstallationDataAsync(new UserUserRole { UserId = fourthUser.Id, UserRoleId = urRegistered.Id });

      //set default user name
      await InsertInstallationDataAsync(new GenericAttribute
      {
         EntityId = fourthUser.Id,
         Key = AppUserDefaults.FirstNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultFourthUserAddress.FirstName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      },
      new GenericAttribute
      {
         EntityId = fourthUser.Id,
         Key = AppUserDefaults.LastNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultFourthUserAddress.LastName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      });

      //set user password
      await InsertInstallationDataAsync(new UserPassword
      {
         UserId = fourthUser.Id,
         Password = "Mnbvcxz1",
         PasswordFormat = PasswordFormat.Clear,
         PasswordSalt = string.Empty,
         CreatedOnUtc = DateTime.UtcNow
      });

      //fifth user
      var fifthUserEmail = "arthur_morgan@application.com";
      var fifthUser = new User
      {
         UserGuid = Guid.NewGuid(),
         Email = fifthUserEmail,
         Username = fifthUserEmail.Split('@')[0],
         //Username = "engineer456",
         AvatarPictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "user_5.png")), MimeTypes.ImagePng, await pictureService.GetPictureSeNameAsync("Users"))).Id,
         IsActive = true,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
      };
      var defaultFifthUserAddress = await InsertInstallationDataAsync(
      new Address
      {
         FirstName = "Arthur",
         LastName = "Morgan",
         PhoneNumber = "14785236",
         Email = fifthUserEmail,
         FaxNumber = string.Empty,
         Company = "Brenda Company",
         Address1 = "1249 Tongass Avenue, Suite B",
         Address2 = string.Empty,
         City = "Ketchikan",
         StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Alaska")?.Id,
         CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
         ZipPostalCode = "99901",
         CreatedOnUtc = DateTime.UtcNow
      });

      fifthUser.BillingAddressId = defaultFifthUserAddress.Id;
      fifthUser.ShippingAddressId = defaultFifthUserAddress.Id;

      await InsertInstallationDataAsync(fifthUser);

      await InsertInstallationDataAsync(new UserAddress { UserId = fifthUser.Id, AddressId = defaultFifthUserAddress.Id });
      //await InsertInstallationDataAsync(new UserUserRole { UserId = fifthUser.Id, UserRoleId = urRegistered.Id });

      //set default user name
      await InsertInstallationDataAsync(new GenericAttribute
      {
         EntityId = fifthUser.Id,
         Key = AppUserDefaults.FirstNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultFifthUserAddress.FirstName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      },
      new GenericAttribute
      {
         EntityId = fifthUser.Id,
         Key = AppUserDefaults.LastNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultFifthUserAddress.LastName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      });

      //set user password
      await InsertInstallationDataAsync(new UserPassword
      {
         UserId = fifthUser.Id,
         Password = "Mnbvcxz1",
         PasswordFormat = PasswordFormat.Clear,
         PasswordSalt = string.Empty,
         CreatedOnUtc = DateTime.UtcNow
      });

      //sixth user
      var sixthUserEmail = "victoria_krasnova@application.com";
      var sixthUser = new User
      {
         UserGuid = Guid.NewGuid(),
         Email = sixthUserEmail,
         Username = sixthUserEmail.Split('@')[0],
         //Username = "logist93",
         AvatarPictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "user_6.png")), MimeTypes.ImagePng, await pictureService.GetPictureSeNameAsync("Users"))).Id,
         IsActive = true,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
      };
      var defaultSixthUserAddress = await InsertInstallationDataAsync(
          new Address
          {
             FirstName = "Victoria",
             LastName = "Krasnova",
             PhoneNumber = "45612378",
             Email = sixthUserEmail,
             FaxNumber = string.Empty,
             Company = "Terces Company",
             Address1 = "201 1st Avenue South",
             Address2 = string.Empty,
             City = "Saskatoon",
             StateProvinceId = (await _stateProvinceRepository.Table.FirstOrDefaultAsync(sp => sp.Name == "Saskatchewan"))?.Id,
             CountryId = (await _countryRepository.Table.FirstOrDefaultAsync(c => c.ThreeLetterIsoCode == "CAN"))?.Id,
             ZipPostalCode = "S7K 1J9",
             CreatedOnUtc = DateTime.UtcNow
          });

      sixthUser.BillingAddressId = defaultSixthUserAddress.Id;
      sixthUser.ShippingAddressId = defaultSixthUserAddress.Id;

      await InsertInstallationDataAsync(sixthUser);

      await InsertInstallationDataAsync(new UserAddress { UserId = sixthUser.Id, AddressId = defaultSixthUserAddress.Id });
      //await InsertInstallationDataAsync(new UserUserRole { UserId = sixthUser.Id, UserRoleId = urRegistered.Id });

      //set default user name
      await InsertInstallationDataAsync(new GenericAttribute
      {
         EntityId = sixthUser.Id,
         Key = AppUserDefaults.FirstNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultSixthUserAddress.FirstName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      },
      new GenericAttribute
      {
         EntityId = sixthUser.Id,
         Key = AppUserDefaults.LastNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultSixthUserAddress.LastName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      });

      //set user password
      await InsertInstallationDataAsync(new UserPassword
      {
         UserId = sixthUser.Id,
         Password = "Mnbvcxz1",
         PasswordFormat = PasswordFormat.Clear,
         PasswordSalt = string.Empty,
         CreatedOnUtc = DateTime.UtcNow
      });


      //seventh user
      var seventhUserEmail = "steve_holmes@application.com";
      var seventhUser = new User
      {
         UserGuid = Guid.NewGuid(),
         Email = seventhUserEmail,
         Username = seventhUserEmail.Split('@')[0],
         //Username = "coder99",
         AvatarPictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "user_7.png")), MimeTypes.ImagePng, await pictureService.GetPictureSeNameAsync("Users"))).Id,
         IsActive = true,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
      };
      var defaultSeventhUserAddress = await InsertInstallationDataAsync(
          new Address
          {
             FirstName = "Steve",
             LastName = "Holmes",
             PhoneNumber = "45612378",
             Email = seventhUserEmail,
             FaxNumber = string.Empty,
             Company = "Terces Company",
             Address1 = "201 1st Avenue South",
             Address2 = string.Empty,
             City = "Saskatoon",
             StateProvinceId = (await _stateProvinceRepository.Table.FirstOrDefaultAsync(sp => sp.Name == "Saskatchewan"))?.Id,
             CountryId = (await _countryRepository.Table.FirstOrDefaultAsync(c => c.ThreeLetterIsoCode == "CAN"))?.Id,
             ZipPostalCode = "S7K 1J9",
             CreatedOnUtc = DateTime.UtcNow
          });

      seventhUser.BillingAddressId = defaultSeventhUserAddress.Id;
      seventhUser.ShippingAddressId = defaultSeventhUserAddress.Id;

      await InsertInstallationDataAsync(seventhUser);

      await InsertInstallationDataAsync(new UserAddress { UserId = seventhUser.Id, AddressId = defaultSeventhUserAddress.Id });
      //await InsertInstallationDataAsync(new UserUserRole { UserId = sixthUser.Id, UserRoleId = urRegistered.Id });

      //set default user name
      await InsertInstallationDataAsync(new GenericAttribute
      {
         EntityId = seventhUser.Id,
         Key = AppUserDefaults.FirstNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultSeventhUserAddress.FirstName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      },
      new GenericAttribute
      {
         EntityId = seventhUser.Id,
         Key = AppUserDefaults.LastNameAttribute,
         KeyGroup = nameof(User),
         Value = defaultSeventhUserAddress.LastName,
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      });

      //set user password
      await InsertInstallationDataAsync(new UserPassword
      {
         UserId = seventhUser.Id,
         Password = "Mnbvcxz1",
         PasswordFormat = PasswordFormat.Clear,
         PasswordSalt = string.Empty,
         CreatedOnUtc = DateTime.UtcNow
      });


      // add roles
      var users = new List<User>() { secondUser, thirdUser, fourthUser, fifthUser, sixthUser, seventhUser };
      var roles = _userRoleRepository.Table
         .Where(x => x.SystemName == UserDefaults.RegisteredRoleName || x.SystemName == UserDefaults.OwnersRoleName || x.SystemName == UserDefaults.OperatorsRoleName);

      var mappings =
         (from u in users
          from r in roles
          select new UserUserRole() { UserId = u.Id, UserRoleId = r.Id })
          .ToList();

      await InsertInstallationDataAsync(mappings);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallUsersAsync(string defaultUserEmail, string defaultUserPassword)
   {
      var urAdministrators = new UserRole
      {
         Name = "Administrators",
         Active = true,
         IsSystemRole = true,
         SystemName = UserDefaults.AdministratorsRoleName
      };
      var urForumModerators = new UserRole
      {
         Name = "Forum Moderators",
         Active = true,
         IsSystemRole = true,
         SystemName = UserDefaults.ForumModeratorsRoleName
      };
      var urRegistered = new UserRole
      {
         Name = "Registered",
         Active = true,
         IsSystemRole = true,
         SystemName = UserDefaults.RegisteredRoleName
      };
      var urGuests = new UserRole
      {
         Name = "Guests",
         Active = true,
         IsSystemRole = true,
         SystemName = UserDefaults.GuestsRoleName
      };
      var urOwners = new UserRole
      {
         Name = "Owners",
         Active = true,
         IsSystemRole = true,
         SystemName = UserDefaults.OwnersRoleName
      };
      var urOperators = new UserRole
      {
         Name = "Operators",
         Active = true,
         IsSystemRole = true,
         SystemName = UserDefaults.OperatorsRoleName
      };
      var urDevices = new UserRole
      {
         Name = "Devices",
         Active = true,
         IsSystemRole = true,
         SystemName = UserDefaults.DevicesRoleName
      };
      var urDemo = new UserRole
      {
         Name = "Demo",
         Active = true,
         IsSystemRole = true,
         SystemName = UserDefaults.DemoRoleName
      };

      var userRoles = new List<UserRole>
      {
         urAdministrators,
         urForumModerators,
         urRegistered,
         urGuests,
         urOwners,
         urOperators,
         urDevices,
         urDemo
      };

      await InsertInstallationDataAsync(userRoles);

      var pictureService = EngineContext.Current.Resolve<IPictureService>();
      var sampleImagesPath = GetSamplesPath();

      //admin user
      var adminUser = new User
      {
         UserGuid = Guid.NewGuid(),
         Email = defaultUserEmail,
         Username = defaultUserEmail,
         IsActive = true,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
         AvatarPictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "photo_admin.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Users"))).Id,
      };

      var defaultAdminUserAddress = await InsertInstallationDataAsync(new Address
      {
         FirstName = "John",
         LastName = "Smith",
         PhoneNumber = "12345678",
         Email = defaultUserEmail,
         FaxNumber = string.Empty,
         Company = "App Solutions Ltd",
         Address1 = "21 West 52nd Street",
         Address2 = string.Empty,
         City = "New York",
         StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York")?.Id,
         CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
         ZipPostalCode = "10021",
         CreatedOnUtc = DateTime.UtcNow
      });

      adminUser.BillingAddressId = defaultAdminUserAddress.Id;
      adminUser.ShippingAddressId = defaultAdminUserAddress.Id;

      await InsertInstallationDataAsync(adminUser);

      await InsertInstallationDataAsync(new UserAddress { UserId = adminUser.Id, AddressId = defaultAdminUserAddress.Id });

      await InsertInstallationDataAsync(
          new UserUserRole { UserId = adminUser.Id, UserRoleId = urAdministrators.Id },
          new UserUserRole { UserId = adminUser.Id, UserRoleId = urForumModerators.Id },
          new UserUserRole { UserId = adminUser.Id, UserRoleId = urOwners.Id },
          new UserUserRole { UserId = adminUser.Id, UserRoleId = urOperators.Id },
          new UserUserRole { UserId = adminUser.Id, UserRoleId = urRegistered.Id });

      //set default user name
      await InsertInstallationDataAsync(
      new GenericAttribute
      {
         EntityId = adminUser.Id,
         Key = AppUserDefaults.FirstNameAttribute,
         KeyGroup = nameof(User),
         Value = "John",
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      },
      new GenericAttribute
      {
         EntityId = adminUser.Id,
         Key = AppUserDefaults.LastNameAttribute,
         KeyGroup = nameof(User),
         Value = "Smith",
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      });

      //set hashed admin password
      var userRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
      await userRegistrationService.ChangePasswordAsync(new ChangePasswordRequest(defaultUserEmail, false,
           PasswordFormat.Hashed, defaultUserPassword, null, AppUserServicesDefaults.DefaultHashedPasswordFormat));

      //search engine (crawler) built-in user
      var searchEngineUser = new User
      {
         Email = "builtin@search_engine_record.com",
         UserGuid = Guid.NewGuid(),
         AdminComment = "Built-in system guest record used for requests from search engines.",
         IsActive = true,
         IsSystemAccount = true,
         SystemName = AppUserDefaults.SearchEngineUserName,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
      };

      await InsertInstallationDataAsync(searchEngineUser);

      await InsertInstallationDataAsync(new UserUserRole { UserRoleId = urGuests.Id, UserId = searchEngineUser.Id });

      //built-in user for background tasks
      var backgroundTaskUser = new User
      {
         Email = "builtin@background-task-record.com",
         UserGuid = Guid.NewGuid(),
         AdminComment = "Built-in system record used for background tasks.",
         IsActive = true,
         IsSystemAccount = true,
         SystemName = AppUserDefaults.BackgroundTaskUserName,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
      };

      await InsertInstallationDataAsync(backgroundTaskUser);

      await InsertInstallationDataAsync(new UserUserRole { UserId = backgroundTaskUser.Id, UserRoleId = urGuests.Id });

      //demo user
      var demoUser = new User
      {
         UserGuid = Guid.NewGuid(),
         Email = "demo@yourplatform.com",
         Username = "demo",
         IsActive = false,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
      };

      var defaultDemoUserAddress = await InsertInstallationDataAsync(new Address
      {
         FirstName = "Demouser",
         LastName = "Demouser",
         PhoneNumber = "12345678",
         Email = "demo@yourplatform.com",
         FaxNumber = string.Empty,
         Company = "App Solutions Ltd",
         Address1 = "23 West 52nd Street",
         Address2 = string.Empty,
         City = "New York",
         StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York")?.Id,
         CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
         ZipPostalCode = "10021",
         CreatedOnUtc = DateTime.UtcNow
      });

      demoUser.BillingAddressId = defaultDemoUserAddress.Id;
      demoUser.ShippingAddressId = defaultDemoUserAddress.Id;

      await InsertInstallationDataAsync(demoUser);

      await InsertInstallationDataAsync(new UserAddress { UserId = demoUser.Id, AddressId = defaultDemoUserAddress.Id });

      await InsertInstallationDataAsync(
          new UserUserRole { UserId = demoUser.Id, UserRoleId = urAdministrators.Id },
          new UserUserRole { UserId = demoUser.Id, UserRoleId = urForumModerators.Id },
          new UserUserRole { UserId = demoUser.Id, UserRoleId = urOwners.Id },
          new UserUserRole { UserId = demoUser.Id, UserRoleId = urOperators.Id },
          new UserUserRole { UserId = demoUser.Id, UserRoleId = urRegistered.Id },
          new UserUserRole { UserId = demoUser.Id, UserRoleId = urDemo.Id });

      //set default user name
      await InsertInstallationDataAsync(
      new GenericAttribute
      {
         EntityId = demoUser.Id,
         Key = AppUserDefaults.FirstNameAttribute,
         KeyGroup = nameof(User),
         Value = "Demouser",
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      },
      new GenericAttribute
      {
         EntityId = demoUser.Id,
         Key = AppUserDefaults.LastNameAttribute,
         KeyGroup = nameof(User),
         Value = "Demouser",
         CreatedOrUpdatedDateUTC = DateTime.UtcNow
      });

      //set hashed demo password
      await userRegistrationService.ChangePasswordAsync(new ChangePasswordRequest("demo@yourplatform.com", false,
           PasswordFormat.Hashed, "demo", null, AppUserServicesDefaults.DefaultHashedPasswordFormat));

   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallActivityLogAsync(string defaultUserEmail)
   {
      //default user/user
      var defaultUser = _userRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail) ?? throw new Exception("Cannot load default user");

      await InsertInstallationDataAsync(new ActivityLog
      {
         ActivityLogTypeId = _activityLogTypeRepository.Table.FirstOrDefault(alt => alt.SystemKeyword == "AddNewUserRole")?.Id
            ?? throw new Exception("Cannot load LogType: AddNewUserRole"),

         Comment = "Added a new user role (operator)",
         CreatedOnUtc = DateTime.UtcNow,
         SubjectId = defaultUser.Id,
         SubjectName = typeof(User).Name,
         IpAddress = "127.0.0.1"
      });
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallSearchTermsAsync()
   {

      await InsertInstallationDataAsync(new SearchTerm
      {
         Count = 34,
         Keyword = "computer",
      });

      await InsertInstallationDataAsync(new SearchTerm
      {
         Count = 30,
         Keyword = "sensor",
      });

      await InsertInstallationDataAsync(new SearchTerm
      {
         Count = 27,
         Keyword = "actuator",
      });

      await InsertInstallationDataAsync(new SearchTerm
      {
         Count = 26,
         Keyword = "wire",
      });

      await InsertInstallationDataAsync(new SearchTerm
      {
         Count = 19,
         Keyword = "device",
      });

      await InsertInstallationDataAsync(new SearchTerm
      {
         Count = 10,
         Keyword = "rs232",
      });
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallEmailAccountsAsync()
   {
      var emailAccounts = new List<EmailAccount>
      {
         new EmailAccount
         {
            Email = "hub@yourplatform.com",
            DisplayName = "tinyPlatform",
            Host = "smtp.yourplatform.com",
            Port = 25,
            Username = "123",
            Password = "123",
            EnableSsl = false,
            UseDefaultCredentials = false
         }
      };

      await InsertInstallationDataAsync(emailAccounts);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallMessageTemplatesAsync()
   {
      var eaGeneral = _emailAccountRepository.Table.FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");

      var messageTemplates = new List<MessageTemplate>
      {
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.BlogCommentNotification,
            Subject = "%App.Name%. New blog comment.",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"A new blog comment has been created for blog post \"%BlogComment.BlogPostTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.UserEmailValidationMessage,
            Subject = "%App.Name%. Email validation",
            Body =
               $"<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"To activate your account <a href=\"%User.AccountActivationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />" +
               $"{Environment.NewLine}%App.Name%{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.UserEmailRevalidationMessage,
            Subject = "%App.Name%. Email validation",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"Hello %User.FullName%!{Environment.NewLine}<br />{Environment.NewLine}To validate your new email address" +
               $" <a href=\"%User.EmailRevalidationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />" +
               $"{Environment.NewLine}%App.Name%{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.PrivateMessageNotification,
            Subject = "%App.Name%. You have received a new private message",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"You have received a new private message.{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.UserPasswordRecoveryMessage,
            Subject = "%App.Name%. Password recovery",
            Body =
               $"<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"To change your password <a href=\"%User.PasswordRecoveryURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />" +
               $"{Environment.NewLine}%App.Name%{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.UserWelcomeMessage,
            Subject = "Welcome to %App.Name%",
            Body =
               $"We welcome you to <a href=\"%App.URL%\"> %App.Name%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"You can now take part in the various services we have to offer you. Some of these services include:{Environment.NewLine}<br />{Environment.NewLine}<br />" +
               $"{Environment.NewLine}{Environment.NewLine}<br />{Environment.NewLine}For help with any of our online services, please email the platform-owner:" +
               $" <a href=\"mailto:%App.Email%\">%App.Email%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"Note: This email address was provided on our registration page. If you own the email and did not register on our site, " +
               $"please send an email to <a href=\"mailto:%App.Email%\">%App.Email%</a>.{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.NewForumPostMessage,
            Subject = "%App.Name%. New Post Notification.",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"A new post has been created in the topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"" +
               $"</a> forum.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Click <a href=\"%Forums.TopicURL%\">here</a> for more info." +
               $"{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Post author: %Forums.PostAuthor%{Environment.NewLine}<br />{Environment.NewLine}" +
               $"Post body: %Forums.PostBody%{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.NewForumTopicMessage,
            Subject = "%App.Name%. New Topic Notification.",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"A new topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> has been created at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum." +
               $"{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{Environment.NewLine}" +
               $"</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.UserRegisteredNotification,
            Subject = "%App.Name%. New user registration",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"A new user registered with your platform. Below are the user's details:{Environment.NewLine}<br />{Environment.NewLine}" +
               $"Full name: %User.FullName%{Environment.NewLine}<br />{Environment.NewLine}Email: %User.Email%{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.DeviceRegisteredNotification,
            Subject = "%App.Name%. New device registration",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"A new device \"%Device.SystemName%\" was registered by your platform. Below are the user's details:{Environment.NewLine}<br />{Environment.NewLine}" +
               $"Full name: %User.FullName%{Environment.NewLine}<br />{Environment.NewLine}Email: %User.Email%{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.NewsCommentNotification,
            Subject = "%App.Name%. New news comment.",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"A new news comment has been created for news \"%NewsComment.NewsTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage,
            Subject = "%App.Name%. Subscription activation message.",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a>" +
               $"{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}If you received this email by mistake, simply delete it.{Environment.NewLine}</p>" +
               $"{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage,
            Subject = "%App.Name%. Subscription deactivation message.",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%NewsLetterSubscription.DeactivationUrl%\">Click here to unsubscribe from our newsletter.</a>{Environment.NewLine}" +
               $"</p>{Environment.NewLine}<p>{Environment.NewLine}If you received this email by mistake, simply delete it.{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.NewVatSubmittedPlatformOwnerNotification,
            Subject = "%App.Name%. New VAT number is submitted.",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\">%App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"%User.FullName% (%User.Email%) has just submitted a new VAT number. Details are below:{Environment.NewLine}<br />{Environment.NewLine}VAT number:" +
               $" %User.VatNumber%{Environment.NewLine}<br />{Environment.NewLine}VAT number status: %User.VatNumberStatus%{Environment.NewLine}<br />" +
               $"{Environment.NewLine}Received name: %VatValidationResult.Name%{Environment.NewLine}<br />{Environment.NewLine}Received address: " +
               $"%VatValidationResult.Address%{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.EmailAFriendMessage,
            Subject = "%App.Name%. Referred Item",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\"> %App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"%EmailAFriend.Email% was shopping on %App.Name% and wanted to share the following item with you.{Environment.NewLine}<br />{Environment.NewLine}<br />" +
               $"{Environment.NewLine}<b><a target=\"_blank\" href=\"%Product.ProductURLForUser%\">%Product.Name%</a></b>{Environment.NewLine}<br />" +
               $"{Environment.NewLine}%Product.ShortDescription%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"For more info click <a target=\"_blank\" href=\"%Product.ProductURLForUser%\">here</a>{Environment.NewLine}<br />{Environment.NewLine}<br />" +
               $"{Environment.NewLine}<br />{Environment.NewLine}%EmailAFriend.PersonalMessage%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"%App.Name%{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.WishlistToFriendMessage,
            Subject = "%App.Name%. Wishlist",
            Body =
               $"<p>{Environment.NewLine}<a href=\"%App.URL%\"> %App.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}" +
               $"%Wishlist.Email% was shopping on %App.Name% and wanted to share a wishlist with you.{Environment.NewLine}<br />{Environment.NewLine}<br />" +
               $"{Environment.NewLine}<br />{Environment.NewLine}For more info click <a target=\"_blank\" href=\"%Wishlist.URLForUser%\">here</a>{Environment.NewLine}" +
               $"<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Wishlist.PersonalMessage%{Environment.NewLine}<br />" +
               $"{Environment.NewLine}<br />{Environment.NewLine}%App.Name%{Environment.NewLine}</p>{Environment.NewLine}",

            IsActive = true,
            EmailAccountId = eaGeneral.Id
         },
         new MessageTemplate
         {
            Name = MessageTemplateSystemNames.ContactUsMessage,
            Subject = "%App.Name%. Contact us",
            Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
            IsActive = true,
            EmailAccountId = eaGeneral.Id
         }
      };

      await InsertInstallationDataAsync(messageTemplates);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallTopicsAsync()
   {
      var defaultTopicTemplate = _topicTemplateRepository.Table.FirstOrDefault(tt => tt.Name == "Default template")
         ?? throw new Exception("Topic template cannot be loaded");

      var topics = new List<Topic>
      {
         new Topic
         {
            SystemName = "AboutUs",
            IncludeInSitemap = false,
            IsPasswordProtected = false,
            IncludeInFooterColumn1 = true,
            DisplayOrder = 20,
            Published = true,
            Title = "About us",
            Body =
               "<p>Put your &quot;About Us&quot; information here. You can edit this in the admin site.</p>",
            TopicTemplateId = defaultTopicTemplate.Id
         },
         new Topic
         {
            SystemName = "ConditionsOfUse",
            IncludeInSitemap = false,
            IsPasswordProtected = false,
            IncludeInFooterColumn1 = true,
            DisplayOrder = 15,
            Published = true,
            Title = "Conditions of Use",
            Body = "<p>Put your conditions of use information here. You can edit this in the admin site.</p>",
            TopicTemplateId = defaultTopicTemplate.Id
         },
         new Topic
         {
            SystemName = "ContactUs",
            IncludeInSitemap = false,
            IsPasswordProtected = false,
            DisplayOrder = 1,
            Published = true,
            Title = string.Empty,
            Body = "<p>Put your contact information here. You can edit this in the admin site.</p>",
            TopicTemplateId = defaultTopicTemplate.Id
         },
         new Topic
         {
            SystemName = "ForumWelcomeMessage",
            IncludeInSitemap = false,
            IsPasswordProtected = false,
            DisplayOrder = 1,
            Published = true,
            Title = "Forums",
            Body = "<p>Put your welcome message here. You can edit this in the admin site.</p>",
            TopicTemplateId = defaultTopicTemplate.Id
         },
         new Topic
         {
            SystemName = "HomepageText",
            IncludeInSitemap = false,
            IsPasswordProtected = false,
            DisplayOrder = 1,
            Published = true,
            Title = "Welcome to tinyPlatform",
            Body =
            "<p>It is a <a href=\"https://github.com/aybelanov/tinyPlatform?tab=GPL-3.0-1-ov-file\" target=\"_blank\">free</a> and <a href=\"https://github.com/aybelanov/tinyPlatform\" target=\"_blank\">open source</a> simple Web IoT platform for data acquisition, remote control and online monitoring, where you can register devices, creates widgets and monitors and share them to other users.</p>" +
            "<p>You are on the main public page. <a href=\"/login?returnUrl=%2F\">Login</a> as a demo user (username: demo, password: demo) and then learn <a href=\"/dashboard\">the telemetry dashboard</a> and <a href=\"/admin\">the hub admin panel</a>.</p>" +
            "<p>If you have questions, see the <a href=\"http://docs.tinyplat.com/\">Documentation</a>, or open a disscussion on <a href=\"https://github.com/aybelanov/tinyPlatform/discussions\">GitHub</a></p>" +
            "<p>You can edit this content in the admin panel.</p>",
            TopicTemplateId = defaultTopicTemplate.Id
         },
         new Topic
         {
            SystemName = "LoginRegistrationInfo",
            IncludeInSitemap = false,
            IsPasswordProtected = false,
            DisplayOrder = 1,
            Published = true,
            Title = "About login / registration",
            Body = "<p>For demo login, enter the username: demo and password: demo. You can edit this in the admin site.</p>",
            TopicTemplateId = defaultTopicTemplate.Id
         },
         new Topic
         {
            SystemName = "PrivacyInfo",
            IncludeInSitemap = false,
            IsPasswordProtected = false,
            IncludeInFooterColumn1 = true,
            DisplayOrder = 10,
            Published = true,
            Title = "Privacy notice",
            Body = "<p>Put your privacy policy information here. You can edit this in the admin site.</p>",
            TopicTemplateId = defaultTopicTemplate.Id
         },
         new Topic
         {
            SystemName = "PageNotFound",
            IncludeInSitemap = false,
            IsPasswordProtected = false,
            DisplayOrder = 1,
            Published = true,
            Title = string.Empty,
            Body =
               "<p><strong>The page you requested was not found, and we have a fine guess why.</strong></p><ul><li>If you typed the URL directly," +
               " please make sure the spelling is correct.</li><li>The page no longer exists. In this case, we profusely apologize for the inconvenience " +
               "and for any damage this may cause.</li></ul>",

            TopicTemplateId = defaultTopicTemplate.Id
         }
      };

      await InsertInstallationDataAsync(topics);

      var defaultCulture = new CultureInfo(HubCommonDefaults.DefaultLanguageCulture);
      var allLanguages = await EngineContext.Current.Resolve<ILanguageService>().GetAllLanguagesAsync();
      var languageEn = allLanguages.FirstOrDefault(x => x.LanguageCulture == defaultCulture.Name) ?? throw new AppException("Default language does not installed");
      var languageRu = allLanguages.FirstOrDefault(x => x.LanguageCulture == "ru-RU") ?? throw new AppException("Russian language does not installed");

      //search engine names
      foreach (var topic in topics)
      {
         await InsertInstallationDataAsync(new UrlRecord
         {
            EntityId = topic.Id,
            EntityName = nameof(Topic),
            LanguageId = languageEn.Id,
            IsActive = true,
            Slug = await ValidateSeNameAsync(topic, !string.IsNullOrEmpty(topic.Title) ? topic.Title : topic.SystemName)
         });
      }

      var homePageTopic = topics.First(x => x.SystemName == "HomepageText");
      var aboutLoginTopic = topics.First(x => x.SystemName == "LoginRegistrationInfo");
      await InsertInstallationDataAsync(new List<LocalizedProperty>()
      {
         new()
         {
            EntityId = homePageTopic.Id,
            LocaleKeyGroup = nameof(Topic),
            LocaleKey = nameof(Topic.Title),
            LanguageId = languageEn.Id,
            LocaleValue = "Welcome to tinyPlatform"
         },
         new()
         {
            EntityId = homePageTopic.Id,
            LocaleKeyGroup = nameof(Topic),
            LocaleKey = nameof(Topic.Title),
            LanguageId = languageRu.Id,
            LocaleValue = "Добро пожаловать на tinyPlatfom"
         },
         new()
         {
            EntityId = homePageTopic.Id,
            LocaleKeyGroup = nameof(Topic),
            LocaleKey = nameof(Topic.Body),
            LanguageId = languageEn.Id,
            LocaleValue = "<p>This is a <a href=\"https://github.com/aybelanov/tinyPlatform?tab=GPL-3.0-1-ov-file\" target=\"_blank\">free</a> and " +
            "<a href=\"https://github.com/aybelanov/tinyPlatform\" target=\"_blank\">open source</a> simple Web IoT platform " +
            "for data acquisition, remote control and online monitoring, where you can register devices, creates widgets and monitors and share them to other users.</p>" +
            "<p>You are on the main public page. <a href=\"/login?returnUrl=%2F\">Login</a> as a demo user (username: demo, password: demo) " +
            "and then learn <a href=\"/dashboard\">Telemetry dashboard</a> and <a href=\"/admin\">Admin panel</a>.</p>" +
            "<p>If you have questions, see the <a href=\"http://docs.tinyplat.com/\">Documentation</a>," +
            " or open a disscussion on <a href=\"https://github.com/aybelanov/tinyPlatform/discussions\">GitHub</a></p>" +
            "<p>You can edit this content in the admin panel.</p>"
         },
         new()
         {
            EntityId = homePageTopic.Id,
            LocaleKeyGroup = nameof(Topic),
            LocaleKey = nameof(Topic.Body),
            LanguageId = languageRu.Id,
            LocaleValue = "<p>Это <a href=\"https://github.com/aybelanov/tinyPlatform?tab=GPL-3.0-1-ov-file\" target=\"_blank\">бесплатная</a> простая Web IoT платформа" +
            " с <a href=\"https://github.com/aybelanov/tinyPlatform\" target=\"_blank\">открытым исходным кодом</a> для сбора данных, удалённого управления и мониторинга," +
            " где вы можете регистрировать устройства, создавать виджеты и мониторы и делиться ими с другими пользователями.</p>" +
            "<p>Вы на главной странице сайта платформы. <a href=\"/login?returnUrl=%2F\">Войдите</a> как демо пользователь (логин: demo, пароль: demo) " +
            "и затем изучите <a href=\"/dashboard\">Панель телеметрии</a> и <a href=\"/admin\">Панель администрирования</a>.</p>" +
            "<p>Если у вас есть вопросы, смотрите <a href=\"http://docs.tinyplat.com/\">Документацию</a>, " +
            "или откройте дискуссию на <a href=\"https://github.com/aybelanov/tinyPlatform/discussions\">GitHub</a></p>" +
            "<p>Вы можете отредактировать этот контент в админ панели.</p>"
         },
         new()
         {
            EntityId = aboutLoginTopic.Id,
            LocaleKeyGroup = nameof(Topic),
            LocaleKey = nameof(Topic.Title),
            LanguageId = languageEn.Id,
            LocaleValue = "About login / registration"
         },
         new()
         {
            EntityId = aboutLoginTopic.Id,
            LocaleKeyGroup = nameof(Topic),
            LocaleKey = nameof(Topic.Title),
            LanguageId = languageRu.Id,
            LocaleValue = "О регистрации"
         },
         new()
         {
            EntityId = aboutLoginTopic.Id,
            LocaleKeyGroup = nameof(Topic),
            LocaleKey = nameof(Topic.Body),
            LanguageId = languageEn.Id,
            LocaleValue = "For demo login, enter the username: demo and password: demo. You can edit this in the admin site."
         },
         new()
         {
            EntityId = aboutLoginTopic.Id,
            LocaleKeyGroup = nameof(Topic),
            LocaleKey = nameof(Topic.Body),
            LanguageId = languageRu.Id,
            LocaleValue = "Для демо-входа, введите логин: demo, пароль: demo. Вы можете отредактировать этот контент в админ панели"
         },
      });


   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallTopicTemplatesAsync()
   {
      var topicTemplates = new List<TopicTemplate>
      {
         new TopicTemplate
         {
            Name = "Default template",
            ViewPath = "TopicDetails",
            DisplayOrder = 1
         }
      };

      await InsertInstallationDataAsync(topicTemplates);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallSettingsAsync(RegionInfo regionInfo)
   {
      var isMetric = regionInfo?.IsMetric ?? false;
      var country = regionInfo?.TwoLetterISORegionName ?? string.Empty;
      var isGermany = country == "DE";
      var isEurope = ISO3166.FromCountryCode(country)?.SubjectToVat ?? false;

      var settingService = EngineContext.Current.Resolve<ISettingService>();
      await settingService.SaveSettingAsync(new PdfSettings
      {
         LogoPictureId = 0,
         LetterPageSizeEnabled = false,
         RenderOrderNotes = true,
         FontFileName = "FreeSerif.ttf",
         InvoiceFooterTextColumn1 = null,
         InvoiceFooterTextColumn2 = null
      });

      await settingService.SaveSettingAsync(new SitemapSettings
      {
         SitemapEnabled = true,
         SitemapPageSize = 200,
         SitemapIncludeBlogPosts = true,
         SitemapIncludeNews = false,
         SitemapIncludeTopics = true
      });

      await settingService.SaveSettingAsync(new SitemapXmlSettings
      {
         SitemapXmlEnabled = true,
         SitemapXmlIncludeBlogPosts = true,
         SitemapXmlIncludeCategories = true,
         SitemapXmlIncludeNews = true,
         SitemapXmlIncludeCustomUrls = true,
         SitemapXmlIncludeTopics = true
      });

      await settingService.SaveSettingAsync(new CommonSettings
      {
         UseSystemEmailForContactUsForm = true,
         IgnoreAcl = true,
         DisplayJavaScriptDisabledWarning = false,
         Log404Errors = true,
         BreadcrumbDelimiter = "/",
         BbcodeEditorOpenLinksInNewWindow = false,
         PopupForTermsOfServiceLinks = true,
         JqueryMigrateScriptLoggingActive = false,
         UseResponseCompression = true,
         FaviconAndAppIconsHeadCode =

            "<link rel=\"apple-touch-icon\" sizes=\"180x180\" href=\"/icons/icons_0/apple-touch-icon-180x180.png\">" +
            "<link rel=\"icon\" type=\"image/png\" sizes=\"32x32\" href=\"/icons/icons_0/favicon-32x32.png\">" +
            "<link rel=\"icon\" type=\"image/png\" sizes=\"192x192\" href=\"/icons/icons_0/android-chrome-192x192.png\">" +
            "<link rel=\"icon\" type=\"image/png\" sizes=\"16x16\" href=\"/icons/icons_0/favicon-16x16.png\">" +
            "<link rel=\"manifest\" href=\"/icons/icons_0/site.webmanifest\">" +
            "<link rel=\"mask-icon\" href=\"/icons/icons_0/safari-pinned-tab.svg\">" +
            "<link rel=\"shortcut icon\" href=\"/icons/icons_0/favicon_white.svg\">" +
            "<meta name=\"msapplication-TileColor\" content=\"#2d89ef\">" +
            "<meta name=\"msapplication-TileImage\" content=\"/icons/icons_0/mstile-144x144.png\">" +
            "<meta name=\"msapplication-config\" content=\"/icons/icons_0/browserconfig.xml\">" +
            "<meta name=\"theme-color\" content=\"#ffffff\">",

         EnableHtmlMinification = true,
         RestartTimeout = HubCommonDefaults.RestartTimeout,
         UseAjaxLoadMenu = true
      });

      await settingService.SaveSettingAsync(new SeoSettings
      {
         PageTitleSeparator = ". ",
         PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterPlatformname,
         HomepageTitle = "Home page title",
         HomepageDescription = "Home page description",
         DefaultTitle = "Your platform",
         DefaultMetaKeywords = string.Empty,
         DefaultMetaDescription = string.Empty,
         GenerateMetaDescription = true,
         ConvertNonWesternChars = false,
         AllowUnicodeCharsInUrls = true,
         CanonicalUrlsEnabled = false,
         QueryStringInCanonicalUrlsEnabled = false,
         WwwRequirement = WwwRequirement.NoMatter,
         TwitterMetaTags = true,
         OpenGraphMetaTags = true,
         MicrodataEnabled = true,
         ReservedUrlRecordSlugs = AppSeoDefaults.ReservedUrlRecordSlugs,
         CustomHeadTags = string.Empty
      });

      await settingService.SaveSettingAsync(new AdminAreaSettings
      {
         DefaultGridPageSize = 15,
         PopupGridPageSize = 7,
         GridPageSizes = "7, 15, 20, 50, 100",
         RichEditorAdditionalSettings = null,
         RichEditorAllowJavaScript = false,
         RichEditorAllowStyleTag = false,
         UseRichEditorForUserEmails = false,
         UseRichEditorInMessageTemplates = false,
         CheckCopyrightRemovalKey = true,
         UseIsoDateFormatInJsonResult = true,
         ShowDocumentationReferenceLinks = true
      });

      await settingService.SaveSettingAsync(new LocalizationSettings
      {
         DefaultAdminLanguageId = _languageRepository.Table.Single(l => l.LanguageCulture == HubCommonDefaults.DefaultLanguageCulture).Id,
         UseImagesForLanguageSelection = false,
         SeoFriendlyUrlsForLanguagesEnabled = false,
         AutomaticallyDetectLanguage = false,
         LoadAllLocaleRecordsOnStartup = false,
         LoadAllLocalizedPropertiesOnStartup = false,
         LoadAllUrlRecordsOnStartup = false,
         IgnoreRtlPropertyForAdminArea = false
      });

      await settingService.SaveSettingAsync(new UserSettings
      {
         UsernamesEnabled = true,
         UsernameMinLenght = 5,
         CheckUsernameAvailabilityEnabled = true,
         AllowUsersToChangeUsernames = false,
         DefaultPasswordFormat = PasswordFormat.Hashed,
         HashedPasswordFormat = AppUserServicesDefaults.DefaultHashedPasswordFormat,
         PasswordMinLength = 6,
         PasswordRequireDigit = true,
         PasswordRequireLowercase = true,
         PasswordRequireNonAlphanumeric = true,
         PasswordRequireUppercase = true,
         UnduplicatedPasswordsNumber = 4,
         PasswordRecoveryLinkDaysValid = 7,
         PasswordLifetime = 90,
         FailedPasswordAllowedAttempts = 0,
         FailedPasswordLockoutMinutes = 30,
         UserRegistrationType = UserRegistrationType.AdminApproval,
         AllowUsersToUploadAvatars = true,
         AvatarMaximumSizeBytes = 20000,
         DefaultAvatarEnabled = true,
         ShowUsersLocation = true,
         ShowUsersJoinDate = true,
         AllowViewingProfiles = false,
         NotifyNewUserRegistration = true,
         UserNameFormat = UserNameFormat.ShowFirstName,
         FirstNameEnabled = true,
         FirstNameRequired = true,
         LastNameEnabled = true,
         LastNameRequired = true,
         GenderEnabled = false,
         DateOfBirthEnabled = false,
         DateOfBirthRequired = false,
         DateOfBirthMinimumAge = null,
         CompanyEnabled = false,
         StreetAddressEnabled = false,
         StreetAddress2Enabled = false,
         ZipPostalCodeEnabled = false,
         CityEnabled = false,
         CountyEnabled = false,
         CountyRequired = false,
         CountryEnabled = false,
         CountryRequired = false,
         StateProvinceEnabled = false,
         StateProvinceRequired = false,
         PhoneEnabled = false,
         FaxEnabled = false,
         AcceptPrivacyPolicyEnabled = true,
         NewsletterEnabled = false,
         NewsletterTickedByDefault = true,
         HideNewsletterBlock = false,
         NewsletterBlockAllowToUnsubscribe = false,
         BeenRecentlyMinutes = 20,
         PlatformLastVisitedPage = true,
         StoreIpAddresses = true,
         LastActivityMinutes = 15,
         SuffixDeletedUsers = false,
         EnteringEmailTwice = false,
         DeleteGuestTaskOlderThanMinutes = 1440,
         PhoneNumberValidationEnabled = false,
         PhoneNumberValidationUseRegex = false,
         PhoneNumberValidationRule = "^[0-9]{1,14}?$"
      });

      await settingService.SaveSettingAsync(new DeviceSettings
      {
         CheckSystemNameAvailabilityEnabled = true,
         SystemNameMinLength = 7,
         SystemNameMaxLength = 20,
         DeviceRegistrationType = DeviceRegistrationType.Standard,
         BlockDevicesIfOwnerNotActive = true,
         DefaultPasswordFormat = PasswordFormat.Hashed,
         NotifyNewDeviceRegistration = true,
         FailedPasswordAllowedAttempts = 3,
         FailedPasswordLockoutMinutes = 5,
         HashedPasswordFormat = AppUserServicesDefaults.DefaultHashedPasswordFormat,
         PasswordMinLength = 7,
         PasswordRequireDigit = true,
         PasswordRequireLowercase = true,
         PasswordRequireNonAlphanumeric = true,
         PasswordRequireUppercase = true,
         StoreIpAddresses = true,
         UnduplicatedPasswordNumber = 0,
         MaxSensorDatasInDb = 10_000_000,
         VideoFileExpiration = 21_600,
         CleanSensorDataInterval = 3600,
         BeenRecentlyMinutes = 15
      });

      await settingService.SaveSettingAsync(new MultiFactorAuthenticationSettings
      {
         ForceMultifactorAuthentication = false
      });

      await settingService.SaveSettingAsync(new AddressSettings
      {
         CompanyEnabled = true,
         StreetAddressEnabled = true,
         StreetAddressRequired = true,
         StreetAddress2Enabled = true,
         ZipPostalCodeEnabled = true,
         ZipPostalCodeRequired = true,
         CityEnabled = true,
         CityRequired = true,
         CountyEnabled = false,
         CountyRequired = false,
         CountryEnabled = true,
         StateProvinceEnabled = true,
         PhoneEnabled = true,
         PhoneRequired = true,
         FaxEnabled = true
      });

      await settingService.SaveSettingAsync(new MediaSettings
      {
         AvatarPictureSize = 120,
         DefaultThumbPictureSize = 415,
         DefaultDetailsPictureSize = 550,
         DefaultThumbPictureSizeOnDetailsPage = 100,
         AssociatedPictureSize = 220,
         AutoCompleteSearchThumbPictureSize = 20,
         ImageSquarePictureSize = 32,
         MaximumImageSize = 1980,
         DefaultPictureZoomEnabled = false,
         DefaultImageQuality = 80,
         MultipleThumbDirectories = false,
         ImportImagesUsingHash = true,
         AzureCacheControlHeader = string.Empty,
         UseAbsoluteImagePath = true
      });

      await settingService.SaveSettingAsync(new ExternalAuthenticationSettings
      {
         RequireEmailValidation = false,
         LogErrors = false,
         AllowUsersToRemoveAssociations = true
      });

      var primaryCurrency = "USD";
      await settingService.SaveSettingAsync(new CurrencySettings
      {
         DisplayCurrencyLabel = false,
         PrimaryPlatformCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == primaryCurrency).Id,
         PrimaryExchangeRateCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == primaryCurrency).Id,
         ActiveExchangeRateProviderSystemName = "CurrencyExchange.ECB",
         AutoUpdateEnabled = false,
         DisplayCurrencySelector = false
      });

      var baseDimension = isMetric ? "meters" : "inches";
      var baseWeight = isMetric ? "kg" : "lb";

      await settingService.SaveSettingAsync(new MeasureSettings
      {
         BaseDimensionId = _measureDimensionRepository.Table.Single(m => m.SystemKeyword == baseDimension).Id,
         BaseWeightId = _measureWeightRepository.Table.Single(m => m.SystemKeyword == baseWeight).Id
      });

      await settingService.SaveSettingAsync(new MessageTemplatesSettings
      {
         CaseInvariantReplacement = false,
         Color1 = "#b9babe",
         Color2 = "#ebecee",
         Color3 = "#dde2e6"
      });

      await settingService.SaveSettingAsync(new SecuritySettings
      {
         EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
         AdminAreaAllowedIpAddresses = null,
         HoneypotEnabled = false,
         HoneypotInputName = "hpinput",
         AllowNonAsciiCharactersInHeaders = true,
      });

      await settingService.SaveSettingAsync(new DateTimeSettings
      {
         DefaultPlatformTimeZoneId = string.Empty,
         AllowUsersToSetTimeZone = false
      });

      await settingService.SaveSettingAsync(new BlogSettings
      {
         Enabled = true,
         PostsPageSize = 10,
         AllowNotRegisteredUsersToLeaveComments = true,
         NotifyAboutNewBlogComments = false,
         NumberOfTags = 15,
         ShowHeaderRssUrl = false,
         BlogCommentsMustBeApproved = false,
      });
      await settingService.SaveSettingAsync(new NewsSettings
      {
         Enabled = true,
         AllowNotRegisteredUsersToLeaveComments = true,
         NotifyAboutNewNewsComments = false,
         ShowNewsOnMainPage = true,
         MainPageNewsCount = 3,
         NewsArchivePageSize = 10,
         ShowHeaderRssUrl = false,
         NewsCommentsMustBeApproved = false,
      });

      await settingService.SaveSettingAsync(new ForumSettings
      {
         ForumsEnabled = true,
         RelativeDateTimeFormattingEnabled = true,
         AllowUsersToDeletePosts = false,
         AllowUsersToEditPosts = false,
         AllowUsersToManageSubscriptions = false,
         AllowGuestsToCreatePosts = false,
         AllowGuestsToCreateTopics = false,
         AllowPostVoting = true,
         MaxVotesPerDay = 30,
         TopicSubjectMaxLength = 450,
         PostMaxLength = 4000,
         StrippedTopicMaxLength = 45,
         TopicsPageSize = 10,
         PostsPageSize = 10,
         SearchResultsPageSize = 10,
         ActiveDiscussionsPageSize = 50,
         LatestUserPostsPageSize = 10,
         ShowUsersPostCount = true,
         ForumEditor = EditorType.BBCodeEditor,
         SignaturesEnabled = true,
         AllowPrivateMessages = true,
         ShowAlertForPM = true,
         PrivateMessagesPageSize = 10,
         ForumSubscriptionsPageSize = 10,
         NotifyAboutPrivateMessages = false,
         PMSubjectMaxLength = 450,
         PMTextMaxLength = 4000,
         HomepageActiveDiscussionsTopicCount = 5,
         ActiveDiscussionsFeedEnabled = false,
         ActiveDiscussionsFeedCount = 25,
         ForumFeedsEnabled = false,
         ForumFeedCount = 10,
         ForumSearchTermMinimumLength = 3
      });


      var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
      if (eaGeneral == null)
         throw new Exception("Default email account cannot be loaded");
      await settingService.SaveSettingAsync(new EmailAccountSettings
      {
         DefaultEmailAccountId = eaGeneral.Id
      });

      await settingService.SaveSettingAsync(new WidgetSettings
      {
         ActiveWidgetSystemNames = new List<string> { "Widgets.NivoSlider" }
      });

      await settingService.SaveSettingAsync(new DisplayDefaultMenuItemSettings
      {
         DisplayHomepageMenuItem = true,
         DisplaySearchMenuItem = true,
         DisplayUserInfoMenuItem = true,
         DisplayBlogMenuItem = true,
         DisplayForumsMenuItem = true,
         DisplayContactUsMenuItem = true
      });

      await settingService.SaveSettingAsync(new DisplayDefaultFooterItemSettings
      {
         DisplaySitemapFooterItem = true,
         DisplayContactUsFooterItem = true,
         DisplayUserInfoFooterItem = true,
         DisplayUserAddressesFooterItem = true,
      });

      await settingService.SaveSettingAsync(new CaptchaSettings
      {
         ReCaptchaApiUrl = "https://www.google.com/recaptcha/",
         ReCaptchaDefaultLanguage = string.Empty,
         ReCaptchaPrivateKey = string.Empty,
         ReCaptchaPublicKey = string.Empty,
         ReCaptchaRequestTimeout = 20,
         ReCaptchaTheme = string.Empty,
         AutomaticallyChooseLanguage = true,
         Enabled = false,
         CaptchaType = CaptchaType.CheckBoxReCaptchaV2,
         ReCaptchaV3ScoreThreshold = 0.5M,
         ShowOnBlogCommentPage = false,
         ShowOnContactUsPage = false,
         ShowOnEmailToFriendPage = false,
         ShowOnEmailWishlistToFriendPage = false,
         ShowOnForgotPasswordPage = false,
         ShowOnForum = false,
         ShowOnLoginPage = false,
         ShowOnNewsCommentPage = false,
         ShowOnRegistrationPage = false,
      });

      await settingService.SaveSettingAsync(new MessagesSettings
      {
         UsePopupNotifications = false
      });

      await settingService.SaveSettingAsync(new ProxySettings
      {
         Enabled = false,
         Address = string.Empty,
         Port = string.Empty,
         Username = string.Empty,
         Password = string.Empty,
         BypassOnLocal = true,
         PreAuthenticate = true
      });

      await settingService.SaveSettingAsync(new CookieSettings
      {
         CompareProductsCookieExpires = 24 * 10,
         RecentlyViewedProductsCookieExpires = 24 * 10,
         UserCookieExpires = 24 * 365
      });
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallActivityLogTypesAsync()
   {
      var activityLogTypes = new List<ActivityLogType>
      {
         //admin area activities
         new ActivityLogType
         {
            SystemKeyword = "AddNewAddressAttribute",
            Enabled = true,
            Name = "Add a new address attribute"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewAddressAttributeValue",
            Enabled = true,
            Name = "Add a new address attribute value"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewAffiliate",
            Enabled = true,
            Name = "Add a new affiliate"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewBlogPost",
            Enabled = true,
            Name = "Add a new blog post"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewCampaign",
            Enabled = true,
            Name = "Add a new campaign"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewCountry",
            Enabled = true,
            Name = "Add a new country"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewCurrency",
            Enabled = true,
            Name = "Add a new currency"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewUser",
            Enabled = true,
            Name = "Add a new user"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewUserAttribute",
            Enabled = true,
            Name = "Add a new user attribute"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewUserAttributeValue",
            Enabled = true,
            Name = "Add a new user attribute value"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewUserRole",
            Enabled = true,
            Name = "Add a new user role"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewEmailAccount",
            Enabled = true,
            Name = "Add a new email account"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewLanguage",
            Enabled = true,
            Name = "Add a new language"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewMeasureDimension",
            Enabled = true,
            Name = "Add a new measure dimension"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewMeasureWeight",
            Enabled = true,
            Name = "Add a new measure weight"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewNews",
            Enabled = true,
            Name = "Add a new news"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewSetting",
            Enabled = true,
            Name = "Add a new setting"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewStateProvince",
            Enabled = true,
            Name = "Add a new state or province"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewTopic",
            Enabled = true,
            Name = "Add a new topic"
         },
         new ActivityLogType
         {
            SystemKeyword = "AddNewWidget",
            Enabled = true,
            Name = "Add a new widget"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteActivityLog",
            Enabled = true,
            Name = "Delete activity log"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteAddressAttribute",
            Enabled = true,
            Name = "Delete an address attribute"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteAddressAttributeValue",
            Enabled = true,
            Name = "Delete an address attribute value"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteAffiliate",
            Enabled = true,
            Name = "Delete an affiliate"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteBlogPost",
            Enabled = true,
            Name = "Delete a blog post"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteBlogPostComment",
            Enabled = true,
            Name = "Delete a blog post comment"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteCampaign",
            Enabled = true,
            Name = "Delete a campaign"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteCountry",
            Enabled = true,
            Name = "Delete a country"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteCurrency",
            Enabled = true,
            Name = "Delete a currency"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteUser",
            Enabled = true,
            Name = "Delete a user"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteUserAttribute",
            Enabled = true,
            Name = "Delete a user attribute"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteUserAttributeValue",
            Enabled = true,
            Name = "Delete a user attribute value"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteUserRole",
            Enabled = true,
            Name = "Delete a user role"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteEmailAccount",
            Enabled = true,
            Name = "Delete an email account"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteLanguage",
            Enabled = true,
            Name = "Delete a language"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteManufacturer",
            Enabled = true,
            Name = "Delete a manufacturer"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteMeasureDimension",
            Enabled = true,
            Name = "Delete a measure dimension"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteMeasureWeight",
            Enabled = true,
            Name = "Delete a measure weight"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteMessageTemplate",
            Enabled = true,
            Name = "Delete a message template"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteNews",
            Enabled = true,
            Name = "Delete a news"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteNewsComment",
            Enabled = true,
            Name = "Delete a news comment"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeletePlugin",
            Enabled = true,
            Name = "Delete a plugin"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteSetting",
            Enabled = true,
            Name = "Delete a setting"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteStateProvince",
            Enabled = true,
            Name = "Delete a state or province"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteSystemLog",
            Enabled = true,
            Name = "Delete system log"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteTopic",
            Enabled = true,
            Name = "Delete a topic"
         },
         new ActivityLogType
         {
            SystemKeyword = "DeleteWidget",
            Enabled = true,
            Name = "Delete a widget"
         },
         new ActivityLogType
         {
            SystemKeyword = "DashBoard.Connected",
            Enabled = true,
            Name = "Dashboard connected"
         },
         new ActivityLogType
         {
            SystemKeyword = "DashBoard.Disconnected",
            Enabled = true,
            Name = "Dashboard disconnected"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditActivityLogTypes",
            Enabled = true,
            Name = "Edit activity log types"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditAddressAttribute",
            Enabled = true,
            Name = "Edit an address attribute"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditAddressAttributeValue",
            Enabled = true,
            Name = "Edit an address attribute value"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditAffiliate",
            Enabled = true,
            Name = "Edit an affiliate"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditBlogPost",
            Enabled = true,
            Name = "Edit a blog post"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditCampaign",
            Enabled = true,
            Name = "Edit a campaign"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditCategory",
            Enabled = true,
            Name = "Edit category"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditCheckoutAttribute",
            Enabled = true,
            Name = "Edit a checkout attribute"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditCountry",
            Enabled = true,
            Name = "Edit a country"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditCurrency",
            Enabled = true,
            Name = "Edit a currency"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditUser",
            Enabled = true,
            Name = "Edit a user"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditUserAttribute",
            Enabled = true,
            Name = "Edit a user attribute"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditUserAttributeValue",
            Enabled = true,
            Name = "Edit a user attribute value"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditUserRole",
            Enabled = true,
            Name = "Edit a user role"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditEmailAccount",
            Enabled = true,
            Name = "Edit an email account"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditLanguage",
            Enabled = true,
            Name = "Edit a language"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditMeasureDimension",
            Enabled = true,
            Name = "Edit a measure dimension"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditMeasureWeight",
            Enabled = true,
            Name = "Edit a measure weight"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditMessageTemplate",
            Enabled = true,
            Name = "Edit a message template"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditNews",
            Enabled = true,
            Name = "Edit a news"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditOrder",
            Enabled = true,
            Name = "Edit an order"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditPlugin",
            Enabled = true,
            Name = "Edit a plugin"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditSettings",
            Enabled = true,
            Name = "Edit setting(s)"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditStateProvince",
            Enabled = true,
            Name = "Edit a state or province"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditServiceInfo",
            Enabled = true,
            Name = "Edit service info"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditTask",
            Enabled = true,
            Name = "Edit a task"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditTopic",
            Enabled = true,
            Name = "Edit a topic"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditWidget",
            Enabled = true,
            Name = "Edit a widget"
         },
         new ActivityLogType
         {
            SystemKeyword = "Impersonation.Started",
            Enabled = true,
            Name = "User impersonation session. Started"
         },
         new ActivityLogType
         {
            SystemKeyword = "Impersonation.Finished",
            Enabled = true,
            Name = "User impersonation session. Finished"
         },
         new ActivityLogType
         {
            SystemKeyword = "ImportStates",
            Enabled = true,
            Name = "States were imported"
         },
         new ActivityLogType
         {
            SystemKeyword = "InstallNewPlugin",
            Enabled = true,
            Name = "Install a new plugin"
         },
         new ActivityLogType
         {
            SystemKeyword = "UninstallPlugin",
            Enabled = true,
            Name = "Uninstall a plugin"
         },
         //public platform activities
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.SendPM",
            Enabled = false,
            Name = "Public platform. Send PM"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.ContactUs",
            Enabled = false,
            Name = "Public platform. Use contact us form"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.AddToWishlist",
            Enabled = false,
            Name = "Public platform. Add to wishlist"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.Login",
            Enabled = false,
            Name = "Public platform. Login"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.Logout",
            Enabled = false,
            Name = "Public platform. Logout"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.AddNewsComment",
            Enabled = false,
            Name = "Public platform. Add news comment"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.AddBlogComment",
            Enabled = false,
            Name = "Public platform. Add blog comment"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.AddForumTopic",
            Enabled = false,
            Name = "Public platform. Add forum topic"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.EditForumTopic",
            Enabled = false,
            Name = "Public platform. Edit forum topic"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.DeleteForumTopic",
            Enabled = false,
            Name = "Public platform. Delete forum topic"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.AddForumPost",
            Enabled = false,
            Name = "Public platform. Add forum post"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.EditForumPost",
            Enabled = false,
            Name = "Public platform. Edit forum post"
         },
         new ActivityLogType
         {
            SystemKeyword = "PublicPlatform.DeleteForumPost",
            Enabled = false,
            Name = "Public platform. Delete forum post"
         },
         new ActivityLogType
         {
            SystemKeyword = "UploadNewPlugin",
            Enabled = true,
            Name = "Upload a plugin"
         },
         new ActivityLogType
         {
            SystemKeyword = "UploadNewTheme",
            Enabled = true,
            Name = "Upload a theme"
         },
         new ActivityLogType
         {
            SystemKeyword = "UploadIcons",
            Enabled = true,
            Name = "Upload a favicon and app icons"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditNewsComment",
            Enabled = true,
            Name = "Edit a news comment"
         },
         new ActivityLogType
         {
            SystemKeyword = "EditBlogComment",
            Enabled = true,
            Name = "Edited a bog comment."
         },
         new ActivityLogType
         {
            SystemKeyword = "Client.Login",
            Enabled = true,
            Name = "Client login"
         },
         new ActivityLogType
         {
            SystemKeyword = "Client.Logout",
            Enabled = true,
            Name = "Client logout"
         },
         new ActivityLogType
         {
            SystemKeyword = "Client.Connect",
            Enabled = true,
            Name = "Client connect"
         },
         new ActivityLogType
         {
            SystemKeyword = "Client.Disconnect",
            Enabled = true,
            Name = "Client connect"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.Login",
            Enabled = true,
            Name = "Device login"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.Logout",
            Enabled = true,
            Name = "Device logout"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.Connect",
            Enabled = true,
            Name = "Device connect"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.Disconnect",
            Enabled = true,
            Name = "Device disconnect"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.Register",
            Enabled = true,
            Name = "Device register"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.Deleted",
            Enabled = true,
            Name = "Device deleted"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.Updated",
            Enabled = true,
            Name = "Device updated"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.PasswordChanged",
            Enabled = true,
            Name = "Device password changed"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.OwnerChanged",
            Enabled = true,
            Name = "Device owner changed"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.SystemNameChanged",
            Enabled = true,
            Name = "Device system name changed"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.DeleteMap",
            Enabled = true,
            Name = "Device delete user map"
         },
         new ActivityLogType
         {
            SystemKeyword = "Device.AddMap",
            Enabled = true,
            Name = "Device add user map"
         }
      };

      await InsertInstallationDataAsync(activityLogTypes);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallScheduleTasksAsync()
   {
      var lastEnabledUtc = DateTime.UtcNow;
      var tasks = new List<ScheduleTask>
      {
         new() {
            Name = "Send emails",
            Seconds = 60,
            Type = typeof(Hub.Services.Messages.QueuedMessagesSendTask).AssemblyQualifiedName, //"Hub.Services.Messages.QueuedMessagesSendTask, Hub.Services",
            Enabled = false,
            LastEnabledUtc = lastEnabledUtc,
            StopOnError = false
         },
         new() {
            Name = "Keep alive",
            Seconds = 300,
            Type = typeof(Hub.Services.Common.KeepAliveTask).AssemblyQualifiedName,
            Enabled = true,
            LastEnabledUtc = lastEnabledUtc,
            StopOnError = false
         },
         new() {
            Name = "Delete guests",
            Seconds = 600,
            Type = typeof(Hub.Services.Users.DeleteGuestsTask).AssemblyQualifiedName,
            Enabled = true,
            LastEnabledUtc = lastEnabledUtc,
            StopOnError = false
         },
         new() {
            Name = "Clear cache",
            Seconds = 600,
            Type = typeof(Hub.Services.Caching.ClearCacheTask).AssemblyQualifiedName,
            Enabled = false,
            StopOnError = false
         },
         new() {
            Name = "Clear log",
            //60 minutes
            Seconds = 3600,
            Type = typeof(Hub.Services.Logging.ClearLogTask).AssemblyQualifiedName,
            Enabled = false,
            StopOnError = false
         },
         new() {
            Name = "Clear sensor data",
            Seconds = 3600,
            Type = typeof(Hub.Services.Common.ClearSensorDatasTask).AssemblyQualifiedName,
            Enabled = true,
            StopOnError = false
         }
      };

      await InsertInstallationDataAsync(tasks);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallForumsAsync()
   {
      var forumGroup = new ForumGroup
      {
         Name = "General",
         DisplayOrder = 5,
         CreatedOnUtc = DateTime.UtcNow,
         UpdatedOnUtc = DateTime.UtcNow
      };

      await InsertInstallationDataAsync(forumGroup);

      var newProductsForum = new Forum
      {
         ForumGroupId = forumGroup.Id,
         Name = "Dashboard Forum",
         Description = "Discuss the dashboard client and industry trends",
         NumTopics = 0,
         NumPosts = 0,
         LastPostUserId = 0,
         LastPostTime = null,
         DisplayOrder = 1,
         CreatedOnUtc = DateTime.UtcNow,
         UpdatedOnUtc = DateTime.UtcNow
      };

      await InsertInstallationDataAsync(newProductsForum);

      var mobileDevicesForum = new Forum
      {
         ForumGroupId = forumGroup.Id,
         Name = "Devices Forum",
         Description = "Discuss the devices and industry trends",
         NumTopics = 0,
         NumPosts = 0,
         LastPostUserId = 0,
         LastPostTime = null,
         DisplayOrder = 10,
         CreatedOnUtc = DateTime.UtcNow,
         UpdatedOnUtc = DateTime.UtcNow
      };

      await InsertInstallationDataAsync(mobileDevicesForum);

      var packagingShippingForum = new Forum
      {
         ForumGroupId = forumGroup.Id,
         Name = "Hub Forum",
         Description = "Discuss the hub and industry trends",
         NumTopics = 0,
         NumPosts = 0,
         LastPostTime = null,
         DisplayOrder = 20,
         CreatedOnUtc = DateTime.UtcNow,
         UpdatedOnUtc = DateTime.UtcNow
      };

      await InsertInstallationDataAsync(packagingShippingForum);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallBlogPostsAsync(string defaultUserEmail)
   {
      var defaultLanguage = _languageRepository.Table.FirstOrDefault() ?? throw new Exception("Default language could not be loaded");
      var blogService = EngineContext.Current.Resolve<IBlogService>();

      var blogPosts = new List<BlogPost>
            {
                new BlogPost
                {
                    AllowComments = true,
                    LanguageId = defaultLanguage.Id,
                    Title = "Post #1",

                   BodyOverview = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                   "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
                   " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
                   "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>" +
                    "<p>Integer lacinia, mi nec rhoncus tristique, nisl est tempus metus, ornare porttitor nunc odio nec metus." +
                    " Nulla id libero dui. Duis porttitor elit quis lacinia consectetur. Suspendisse ut ante dui." +
                    " Nam at vestibulum nunc. Vestibulum quis metus scelerisque, tincidunt justo a, rhoncus augue." +
                    " Suspendisse ornare bibendum pellentesque. Etiam fermentum tellus eget consequat ornare." +
                    " Vivamus sagittis augue orci, non lacinia ante malesuada convallis. Curabitur condimentum tempus ligula, in mollis diam sagittis non." +
                    " Etiam egestas velit quis justo consequat tincidunt.</p>",

                   Body = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                   "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
                   " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
                   "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>" +
                    "<p>Integer lacinia, mi nec rhoncus tristique, nisl est tempus metus, ornare porttitor nunc odio nec metus." +
                    " Nulla id libero dui. Duis porttitor elit quis lacinia consectetur. Suspendisse ut ante dui." +
                    " Nam at vestibulum nunc. Vestibulum quis metus scelerisque, tincidunt justo a, rhoncus augue." +
                    " Suspendisse ornare bibendum pellentesque. Etiam fermentum tellus eget consequat ornare." +
                    " Vivamus sagittis augue orci, non lacinia ante malesuada convallis. Curabitur condimentum tempus ligula, in mollis diam sagittis non." +
                    " Etiam egestas velit quis justo consequat tincidunt.</p>" +
                    "<p>Mauris euismod dui id tempus blandit. Etiam interdum sagittis arcu eu malesuada. Donec venenatis aliquet diam, vitae porta ligula vulputate ac." +
                    " Donec molestie nibh massa, nec ultrices enim semper non. In auctor ullamcorper nibh, sed vestibulum dolor euismod id. Etiam pulvinar finibus felis," +
                    " eget pharetra leo pharetra id. Donec tristique odio non orci blandit cursus. Ut varius tellus eu euismod eleifend." +
                    " Quisque pretium pharetra ex, at auctor purus gravida at. Cras lobortis eget lectus sed elementum. Etiam sit amet ante vel enim dapibus posuere." +
                    " Nullam rutrum cursus dolor, at ultrices massa eleifend eget. Sed eu dui eget lorem tempor commodo a vel quam." +
                    " Integer tincidunt risus vitae mattis pretium. Maecenas nulla velit, cursus eget dolor sed, lobortis tristique ipsum.</p>" +
                    "<p>Nunc mauris purus, venenatis et lectus iaculis, imperdiet bibendum elit. Aliquam venenatis, enim sit amet auctor vestibulum, " +
                    "nibh tortor elementum lectus, sed tempus urna nisl at dolor. Sed blandit dictum ultricies. Ut congue pulvinar odio, id sodales risus dictum vel." +
                    " Phasellus ac sapien cursus, aliquet augue ac, bibendum lorem. Nullam volutpat mattis nisi, eu cursus felis vulputate porttitor." +
                    " Nunc a massa porttitor, suscipit orci et, sollicitudin nulla. Cras sed pretium lectus. Maecenas mattis in sapien eu vulputate." +
                    " Curabitur pretium congue velit. Maecenas sit amet justo condimentum, finibus lacus vitae, ultrices tortor." +
                    " Praesent suscipit, elit vel ultrices tincidunt, enim lorem scelerisque lacus, sed faucibus risus erat ut diam." +
                    " Maecenas vel gravida neque. Aliquam erat volutpat.</p>",

                   Tags = "iot, e-commerce, blog, money",
                    CreatedOnUtc = DateTime.UtcNow
                },
                new BlogPost
                {
                    AllowComments = true,
                    LanguageId = defaultLanguage.Id,
                    Title = "Post #2",

                   BodyOverview = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                   "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
                   " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
                   "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>" +
                    "<p>Integer lacinia, mi nec rhoncus tristique, nisl est tempus metus, ornare porttitor nunc odio nec metus." +
                    " Nulla id libero dui. Duis porttitor elit quis lacinia consectetur. Suspendisse ut ante dui." +
                    " Nam at vestibulum nunc. Vestibulum quis metus scelerisque, tincidunt justo a, rhoncus augue." +
                    " Suspendisse ornare bibendum pellentesque. Etiam fermentum tellus eget consequat ornare." +
                    " Vivamus sagittis augue orci, non lacinia ante malesuada convallis. Curabitur condimentum tempus ligula, in mollis diam sagittis non." +
                    " Etiam egestas velit quis justo consequat tincidunt.</p>",

                   Body = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                   "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
                   " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
                   "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>" +
                    "<p>Integer lacinia, mi nec rhoncus tristique, nisl est tempus metus, ornare porttitor nunc odio nec metus." +
                    " Nulla id libero dui. Duis porttitor elit quis lacinia consectetur. Suspendisse ut ante dui." +
                    " Nam at vestibulum nunc. Vestibulum quis metus scelerisque, tincidunt justo a, rhoncus augue." +
                    " Suspendisse ornare bibendum pellentesque. Etiam fermentum tellus eget consequat ornare." +
                    " Vivamus sagittis augue orci, non lacinia ante malesuada convallis. Curabitur condimentum tempus ligula, in mollis diam sagittis non." +
                    " Etiam egestas velit quis justo consequat tincidunt.</p>" +
                    "<p>Mauris euismod dui id tempus blandit. Etiam interdum sagittis arcu eu malesuada. Donec venenatis aliquet diam, vitae porta ligula vulputate ac." +
                    " Donec molestie nibh massa, nec ultrices enim semper non. In auctor ullamcorper nibh, sed vestibulum dolor euismod id. Etiam pulvinar finibus felis," +
                    " eget pharetra leo pharetra id. Donec tristique odio non orci blandit cursus. Ut varius tellus eu euismod eleifend." +
                    " Quisque pretium pharetra ex, at auctor purus gravida at. Cras lobortis eget lectus sed elementum. Etiam sit amet ante vel enim dapibus posuere." +
                    " Nullam rutrum cursus dolor, at ultrices massa eleifend eget. Sed eu dui eget lorem tempor commodo a vel quam." +
                    " Integer tincidunt risus vitae mattis pretium. Maecenas nulla velit, cursus eget dolor sed, lobortis tristique ipsum.</p>" +
                    "<p>Nunc mauris purus, venenatis et lectus iaculis, imperdiet bibendum elit. Aliquam venenatis, enim sit amet auctor vestibulum, " +
                    "nibh tortor elementum lectus, sed tempus urna nisl at dolor. Sed blandit dictum ultricies. Ut congue pulvinar odio, id sodales risus dictum vel." +
                    " Phasellus ac sapien cursus, aliquet augue ac, bibendum lorem. Nullam volutpat mattis nisi, eu cursus felis vulputate porttitor." +
                    " Nunc a massa porttitor, suscipit orci et, sollicitudin nulla. Cras sed pretium lectus. Maecenas mattis in sapien eu vulputate." +
                    " Curabitur pretium congue velit. Maecenas sit amet justo condimentum, finibus lacus vitae, ultrices tortor." +
                    " Praesent suscipit, elit vel ultrices tincidunt, enim lorem scelerisque lacus, sed faucibus risus erat ut diam." +
                    " Maecenas vel gravida neque. Aliquam erat volutpat.</p>",

                   Tags = "iot, e-commerce, blog, money",
                    CreatedOnUtc = DateTime.UtcNow
                },
                new BlogPost
                {
                    AllowComments = true,
                    LanguageId = defaultLanguage.Id,
                    Title = "Post #3",

                   BodyOverview = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                   "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
                   " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
                   "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>" +
                    "<p>Integer lacinia, mi nec rhoncus tristique, nisl est tempus metus, ornare porttitor nunc odio nec metus." +
                    " Nulla id libero dui. Duis porttitor elit quis lacinia consectetur. Suspendisse ut ante dui." +
                    " Nam at vestibulum nunc. Vestibulum quis metus scelerisque, tincidunt justo a, rhoncus augue." +
                    " Suspendisse ornare bibendum pellentesque. Etiam fermentum tellus eget consequat ornare." +
                    " Vivamus sagittis augue orci, non lacinia ante malesuada convallis. Curabitur condimentum tempus ligula, in mollis diam sagittis non." +
                    " Etiam egestas velit quis justo consequat tincidunt.</p>",

                   Body = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                   "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
                   " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
                   "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>" +
                    "<p>Integer lacinia, mi nec rhoncus tristique, nisl est tempus metus, ornare porttitor nunc odio nec metus." +
                    " Nulla id libero dui. Duis porttitor elit quis lacinia consectetur. Suspendisse ut ante dui." +
                    " Nam at vestibulum nunc. Vestibulum quis metus scelerisque, tincidunt justo a, rhoncus augue." +
                    " Suspendisse ornare bibendum pellentesque. Etiam fermentum tellus eget consequat ornare." +
                    " Vivamus sagittis augue orci, non lacinia ante malesuada convallis. Curabitur condimentum tempus ligula, in mollis diam sagittis non." +
                    " Etiam egestas velit quis justo consequat tincidunt.</p>" +
                    "<p>Mauris euismod dui id tempus blandit. Etiam interdum sagittis arcu eu malesuada. Donec venenatis aliquet diam, vitae porta ligula vulputate ac." +
                    " Donec molestie nibh massa, nec ultrices enim semper non. In auctor ullamcorper nibh, sed vestibulum dolor euismod id. Etiam pulvinar finibus felis," +
                    " eget pharetra leo pharetra id. Donec tristique odio non orci blandit cursus. Ut varius tellus eu euismod eleifend." +
                    " Quisque pretium pharetra ex, at auctor purus gravida at. Cras lobortis eget lectus sed elementum. Etiam sit amet ante vel enim dapibus posuere." +
                    " Nullam rutrum cursus dolor, at ultrices massa eleifend eget. Sed eu dui eget lorem tempor commodo a vel quam." +
                    " Integer tincidunt risus vitae mattis pretium. Maecenas nulla velit, cursus eget dolor sed, lobortis tristique ipsum.</p>" +
                    "<p>Nunc mauris purus, venenatis et lectus iaculis, imperdiet bibendum elit. Aliquam venenatis, enim sit amet auctor vestibulum, " +
                    "nibh tortor elementum lectus, sed tempus urna nisl at dolor. Sed blandit dictum ultricies. Ut congue pulvinar odio, id sodales risus dictum vel." +
                    " Phasellus ac sapien cursus, aliquet augue ac, bibendum lorem. Nullam volutpat mattis nisi, eu cursus felis vulputate porttitor." +
                    " Nunc a massa porttitor, suscipit orci et, sollicitudin nulla. Cras sed pretium lectus. Maecenas mattis in sapien eu vulputate." +
                    " Curabitur pretium congue velit. Maecenas sit amet justo condimentum, finibus lacus vitae, ultrices tortor." +
                    " Praesent suscipit, elit vel ultrices tincidunt, enim lorem scelerisque lacus, sed faucibus risus erat ut diam." +
                    " Maecenas vel gravida neque. Aliquam erat volutpat.</p>",

                   Tags = "iot, e-commerce, blog, money",
                    CreatedOnUtc = DateTime.UtcNow
                },
            };

      await InsertInstallationDataAsync(blogPosts);

      //search engine names
      foreach (var blogPost in blogPosts)
         await InsertInstallationDataAsync(new UrlRecord
         {
            EntityId = blogPost.Id,
            EntityName = nameof(BlogPost),
            LanguageId = blogPost.LanguageId,
            IsActive = true,
            Slug = await ValidateSeNameAsync(blogPost, blogPost.Title)
         });

      //comments
      var defaultUser = _userRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
      if (defaultUser == null)
         throw new Exception("Cannot load default user");

      foreach (var blogPost in blogPosts)
         await blogService.InsertBlogCommentAsync(new BlogComment
         {
            BlogPostId = blogPost.Id,
            UserId = defaultUser.Id,
            CommentText = "This is a sample comment for this blog post",
            IsApproved = true,
            CreatedOnUtc = DateTime.UtcNow
         });

      await UpdateInstallationDataAsync(blogPosts);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallNewsAsync(string defaultUserEmail)
   {
      var defaultLanguage = _languageRepository.Table.FirstOrDefault();

      if (defaultLanguage == null)
         throw new Exception("Default language could not be loaded");

      var newsService = EngineContext.Current.Resolve<INewsService>();

      var news = new List<NewsItem>
      {
         new NewsItem
         {
            AllowComments = true,
            LanguageId = defaultLanguage.Id,
            Title = "News title 1",

            Short = "<p>Short news #1 description.</p>" +
            "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
            "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
            " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
            "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>",

            Full = "<p>Full news #1 content.</p>" +
            "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
            "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
            " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
            "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>" +
            "<p>Integer lacinia, mi nec rhoncus tristique, nisl est tempus metus, ornare porttitor nunc odio nec metus." +
            " Nulla id libero dui. Duis porttitor elit quis lacinia consectetur. Suspendisse ut ante dui." +
            " Nam at vestibulum nunc. Vestibulum quis metus scelerisque, tincidunt justo a, rhoncus augue." +
            " Suspendisse ornare bibendum pellentesque. Etiam fermentum tellus eget consequat ornare." +
            " Vivamus sagittis augue orci, non lacinia ante malesuada convallis. Curabitur condimentum tempus ligula, in mollis diam sagittis non." +
            " Etiam egestas velit quis justo consequat tincidunt.</p>" +
            "<p>Mauris euismod dui id tempus blandit. Etiam interdum sagittis arcu eu malesuada. Donec venenatis aliquet diam, vitae porta ligula vulputate ac." +
            " Donec molestie nibh massa, nec ultrices enim semper non. In auctor ullamcorper nibh, sed vestibulum dolor euismod id. Etiam pulvinar finibus felis," +
            " eget pharetra leo pharetra id. Donec tristique odio non orci blandit cursus. Ut varius tellus eu euismod eleifend." +
            " Quisque pretium pharetra ex, at auctor purus gravida at. Cras lobortis eget lectus sed elementum. Etiam sit amet ante vel enim dapibus posuere." +
            " Nullam rutrum cursus dolor, at ultrices massa eleifend eget. Sed eu dui eget lorem tempor commodo a vel quam." +
            " Integer tincidunt risus vitae mattis pretium. Maecenas nulla velit, cursus eget dolor sed, lobortis tristique ipsum.</p>" +
            "<p>Nunc mauris purus, venenatis et lectus iaculis, imperdiet bibendum elit. Aliquam venenatis, enim sit amet auctor vestibulum, " +
            "nibh tortor elementum lectus, sed tempus urna nisl at dolor. Sed blandit dictum ultricies. Ut congue pulvinar odio, id sodales risus dictum vel." +
            " Phasellus ac sapien cursus, aliquet augue ac, bibendum lorem. Nullam volutpat mattis nisi, eu cursus felis vulputate porttitor." +
            " Nunc a massa porttitor, suscipit orci et, sollicitudin nulla. Cras sed pretium lectus. Maecenas mattis in sapien eu vulputate." +
            " Curabitur pretium congue velit. Maecenas sit amet justo condimentum, finibus lacus vitae, ultrices tortor." +
            " Praesent suscipit, elit vel ultrices tincidunt, enim lorem scelerisque lacus, sed faucibus risus erat ut diam." +
            " Maecenas vel gravida neque. Aliquam erat volutpat.</p>",

            Published = true,
            CreatedOnUtc = DateTime.UtcNow
         },
         new NewsItem
         {
            AllowComments = true,
            LanguageId = defaultLanguage.Id,
            Title = "News title 2",

            Short = "<p>Short news #2 description.</p>" +
            "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
            "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
            " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
            "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>",

            Full = "<p>Full news #2 content.</p>" +
            "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
            "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
            " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
            "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>" +
            "<p>Integer lacinia, mi nec rhoncus tristique, nisl est tempus metus, ornare porttitor nunc odio nec metus." +
            " Nulla id libero dui. Duis porttitor elit quis lacinia consectetur. Suspendisse ut ante dui." +
            " Nam at vestibulum nunc. Vestibulum quis metus scelerisque, tincidunt justo a, rhoncus augue." +
            " Suspendisse ornare bibendum pellentesque. Etiam fermentum tellus eget consequat ornare." +
            " Vivamus sagittis augue orci, non lacinia ante malesuada convallis. Curabitur condimentum tempus ligula, in mollis diam sagittis non." +
            " Etiam egestas velit quis justo consequat tincidunt.</p>" +
            "<p>Mauris euismod dui id tempus blandit. Etiam interdum sagittis arcu eu malesuada. Donec venenatis aliquet diam, vitae porta ligula vulputate ac." +
            " Donec molestie nibh massa, nec ultrices enim semper non. In auctor ullamcorper nibh, sed vestibulum dolor euismod id. Etiam pulvinar finibus felis," +
            " eget pharetra leo pharetra id. Donec tristique odio non orci blandit cursus. Ut varius tellus eu euismod eleifend." +
            " Quisque pretium pharetra ex, at auctor purus gravida at. Cras lobortis eget lectus sed elementum. Etiam sit amet ante vel enim dapibus posuere." +
            " Nullam rutrum cursus dolor, at ultrices massa eleifend eget. Sed eu dui eget lorem tempor commodo a vel quam." +
            " Integer tincidunt risus vitae mattis pretium. Maecenas nulla velit, cursus eget dolor sed, lobortis tristique ipsum.</p>" +
            "<p>Nunc mauris purus, venenatis et lectus iaculis, imperdiet bibendum elit. Aliquam venenatis, enim sit amet auctor vestibulum, " +
            "nibh tortor elementum lectus, sed tempus urna nisl at dolor. Sed blandit dictum ultricies. Ut congue pulvinar odio, id sodales risus dictum vel." +
            " Phasellus ac sapien cursus, aliquet augue ac, bibendum lorem. Nullam volutpat mattis nisi, eu cursus felis vulputate porttitor." +
            " Nunc a massa porttitor, suscipit orci et, sollicitudin nulla. Cras sed pretium lectus. Maecenas mattis in sapien eu vulputate." +
            " Curabitur pretium congue velit. Maecenas sit amet justo condimentum, finibus lacus vitae, ultrices tortor." +
            " Praesent suscipit, elit vel ultrices tincidunt, enim lorem scelerisque lacus, sed faucibus risus erat ut diam." +
            " Maecenas vel gravida neque. Aliquam erat volutpat.</p>",

            Published = true,
            CreatedOnUtc = DateTime.UtcNow.AddSeconds(1)
         },
         new NewsItem
         {
            AllowComments = true,
            LanguageId = defaultLanguage.Id,
            Title = "News title 3",

            Short = "<p>Short news #3 description.</p>" +
            "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
            "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
            " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
            "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>",

            Full = "<p>Full news #3 content.</p>" +
            "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
            "Vivamus blandit, libero at euismod accumsan, diam sapien iaculis erat, vitae posuere ligula diam quis augue. Proin ut eros id arcu lobortis blandit.'" +
            " Duis velit eros, imperdiet rhoncus orci et, maximus efficitur nisl. Vestibulum quis laoreet felis, at volutpat neque. " +
            "Suspendisse ut ligula et leo tempor suscipit. Aliquam a dignissim tortor. Vivamus volutpat commodo suscipit.</p>" +
            "<p>Integer lacinia, mi nec rhoncus tristique, nisl est tempus metus, ornare porttitor nunc odio nec metus." +
            " Nulla id libero dui. Duis porttitor elit quis lacinia consectetur. Suspendisse ut ante dui." +
            " Nam at vestibulum nunc. Vestibulum quis metus scelerisque, tincidunt justo a, rhoncus augue." +
            " Suspendisse ornare bibendum pellentesque. Etiam fermentum tellus eget consequat ornare." +
            " Vivamus sagittis augue orci, non lacinia ante malesuada convallis. Curabitur condimentum tempus ligula, in mollis diam sagittis non." +
            " Etiam egestas velit quis justo consequat tincidunt.</p>" +
            "<p>Mauris euismod dui id tempus blandit. Etiam interdum sagittis arcu eu malesuada. Donec venenatis aliquet diam, vitae porta ligula vulputate ac." +
            " Donec molestie nibh massa, nec ultrices enim semper non. In auctor ullamcorper nibh, sed vestibulum dolor euismod id. Etiam pulvinar finibus felis," +
            " eget pharetra leo pharetra id. Donec tristique odio non orci blandit cursus. Ut varius tellus eu euismod eleifend." +
            " Quisque pretium pharetra ex, at auctor purus gravida at. Cras lobortis eget lectus sed elementum. Etiam sit amet ante vel enim dapibus posuere." +
            " Nullam rutrum cursus dolor, at ultrices massa eleifend eget. Sed eu dui eget lorem tempor commodo a vel quam." +
            " Integer tincidunt risus vitae mattis pretium. Maecenas nulla velit, cursus eget dolor sed, lobortis tristique ipsum.</p>" +
            "<p>Nunc mauris purus, venenatis et lectus iaculis, imperdiet bibendum elit. Aliquam venenatis, enim sit amet auctor vestibulum, " +
            "nibh tortor elementum lectus, sed tempus urna nisl at dolor. Sed blandit dictum ultricies. Ut congue pulvinar odio, id sodales risus dictum vel." +
            " Phasellus ac sapien cursus, aliquet augue ac, bibendum lorem. Nullam volutpat mattis nisi, eu cursus felis vulputate porttitor." +
            " Nunc a massa porttitor, suscipit orci et, sollicitudin nulla. Cras sed pretium lectus. Maecenas mattis in sapien eu vulputate." +
            " Curabitur pretium congue velit. Maecenas sit amet justo condimentum, finibus lacus vitae, ultrices tortor." +
            " Praesent suscipit, elit vel ultrices tincidunt, enim lorem scelerisque lacus, sed faucibus risus erat ut diam." +
            " Maecenas vel gravida neque. Aliquam erat volutpat.</p>",

            Published = true,
            CreatedOnUtc = DateTime.UtcNow.AddSeconds(2)
         }
      };

      await InsertInstallationDataAsync(news);

      //search engine names
      foreach (var newsItem in news)
         await InsertInstallationDataAsync(new UrlRecord
         {
            EntityId = newsItem.Id,
            EntityName = nameof(NewsItem),
            LanguageId = newsItem.LanguageId,
            IsActive = true,
            Slug = await ValidateSeNameAsync(newsItem, newsItem.Title)
         });

      //comments
      var defaultUser = _userRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
      if (defaultUser == null)
         throw new Exception("Cannot load default user");

      foreach (var newsItem in news)
         await newsService.InsertNewsCommentAsync(new NewsComment
         {
            NewsItemId = newsItem.Id,
            UserId = defaultUser.Id,
            CommentTitle = "Sample comment title",
            CommentText = "This is a sample comment...",
            IsApproved = true,
            CreatedOnUtc = DateTime.UtcNow
         });

      await UpdateInstallationDataAsync(news);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task InstallPollsAsync()
   {
      var defaultLanguage = _languageRepository.Table.FirstOrDefault();

      if (defaultLanguage == null)
         throw new Exception("Default language could not be loaded");

      var poll1 = new Poll
      {
         LanguageId = defaultLanguage.Id,
         Name = "Do you like .NET?",
         SystemKeyword = string.Empty,
         Published = true,
         ShowOnHomepage = true,
         DisplayOrder = 1
      };

      await InsertInstallationDataAsync(poll1);

      var answers = new List<PollAnswer>
      {
         new PollAnswer
         {
            Name = "Excellent",
            DisplayOrder = 1,
            PollId = poll1.Id
         },
         new PollAnswer
         {
            Name = "Good",
            DisplayOrder = 2,
            PollId = poll1.Id
         },
         new PollAnswer
         {
            Name = "Poor",
            DisplayOrder = 3,
            PollId = poll1.Id
         },
         new PollAnswer
         {
            Name = "Very bad",
            DisplayOrder = 4,
            PollId = poll1.Id
         }
      };

      await InsertInstallationDataAsync(answers);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   protected async Task InstallSampleTelemetry(string defaultUserEmail)
   {
      var users = (from u in _userRepository.Table
                   join ur in _userUserRoleRepository.Table on u.Id equals ur.UserId
                   join r in _userRoleRepository.Table on ur.UserRoleId equals r.Id
                   where r.SystemName == UserDefaults.RegisteredRoleName
                   select u).Distinct();

      //var user = users.FirstOrDefault(x => x.Email == defaultUserEmail) ?? throw new Exception("Default user does not exist.");
      var user = users.FirstOrDefault(x => x.Email == "demo@yourplatform.com") ?? throw new Exception("Default user does not exist.");

      // add devices
      var deviceSystemNames = new[]
      {
         "Controller#1",
         "Controller#2",
         "Controller#3",
      };

      var deviceLocations = new double[][]
      {
         [51.62611778293639, 39.23257921797199],
         [51.14809639786075, 37.940344270655],
         [51.73688045750395, 36.25332177246154]
      };

      var deviceList = new List<Device>();

      for (int i = 0; i < deviceSystemNames.Length; i++)
      {
         var device = new Device()
         {
            Configuration = "{}",
            Enabled = true,
            Guid = Guid.NewGuid(),
            SystemName = deviceSystemNames[i],
            DisplayOrder = -15,
            CountDataRows = 1_000_000,
            DataSendingDelay = 1_500,
            ClearDataDelay = 3600,
            DataPacketSize = 1024,
            DataflowReconnectDelay = 10_000,
            IsDeleted = false,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            LastActivityOnUtc = DateTime.UtcNow,
            OwnerId = user.Id,
            IsActive = true,
            IsMobile = (i + 1) % 3 == 0,
            Lat = deviceLocations[i][0],
            Lon = deviceLocations[i][1],
            ShowOnMain = true,
            AdminComment = $"Controller #{i + 1} admin comment.",

            Name = $"Controller #{i + 1} | Контроллер #{i + 1}",
            Description = $"Controller #{i + 1} description | Контроллер #{i + 1} описание"
         };

         deviceList.Add(device);
      }

      await InsertInstallationDataAsync(deviceList);

      //set hashed device password
      using var serviceScope = EngineContext.Current.Resolve<IServiceScopeFactory>().CreateScope();
      var deviceRegistrationService = serviceScope.ServiceProvider.GetRequiredService<IDeviceRegistrationService>();
      foreach (var systemName in deviceSystemNames)
      {
         var result = await deviceRegistrationService.ChangePasswordAsync(new ChangePasswordRequest(systemName, false, PasswordFormat.Hashed, "Secret123!", null, AppUserServicesDefaults.DefaultHashedPasswordFormat));

         if (!result.Success)
         {
            throw new Exception("Sample device registartion errors: " + string.Join('\n', result.Errors));
         }
      }

      // share some device to other users
      var thirdUser = _userRepository.Table.AsNoTracking().Where(x => x.Email == "li_van@application.com").Single();
      await InsertInstallationDataAsync(deviceList.Select(x => new UserDevice() { DeviceId = x.Id, UserId = thirdUser.Id }).ToList());

      // add sensors
      var sensorList = new List<Sensor>();
      foreach (var device in deviceList)
      {
         sensorList.AddRange(new List<Sensor>()
         {
            new Sensor()
            {
               SystemName = "SpeedSensor#1",
               SensorType = SensorType.Scalar,
               PriorityType = PriorityType.Top,
               AdminComment = "Emulator sensor #1 admin comment",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = "{\"AverageValue\": 60,\"DeviationValue\": 30,\"AverageDelay\": 2,\"DeviationDelay\": 50}",

               Name = "Speed sensor #1 | Датчик скорости #1",
               Description = "Speed sensor #1 description | Датчик скорости #1 описание",
               MeasureUnit = "km/h | км/ч"
            },
            new Sensor()
            {
               SystemName = "TemperatureSensor#2",
               SensorType = SensorType.Scalar,
               PriorityType = PriorityType.Top,
               AdminComment = "Temperature sensor #2 admin comment",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = "{\"AverageValue\": 95,\"DeviationValue\": 30,\"AverageDelay\": 2,\"DeviationDelay\": 50}",

               Name = "Temperature sensor #2 | Датчик температуры #2",
               Description = "Temperature sensor #2 description | Датчик температуры #2 описание",
               MeasureUnit = "°C|°C"
            },
            new Sensor()
            {
               SystemName = "PressureSensor#3",
               SensorType = SensorType.Scalar,
               PriorityType = PriorityType.Low,
               AdminComment = "Pressure sensor #3 admin comment",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = "{\"AverageValue\": 1000,\"DeviationValue\": 30,\"AverageDelay\": 2,\"DeviationDelay\": 50}",

               Name = "Pressure sensor #3 | Датчик давления #3",
               Description = "Pressure sensor #3 description | Датчик давления #3 описание",
               MeasureUnit = "Pa|Па"
            },
            new Sensor()
            {
               SystemName = "VoltageSensor#4",
               SensorType = SensorType.Scalar,
               PriorityType = PriorityType.Nomatter,
               AdminComment = "Emulator sensor #4 admin comment",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               DeviceId = device.Id,
               Configuration = device.Id % 2 == 0
                  ? "{\"AverageValue\": 24,\"DeviationValue\": 30,\"AverageDelay\": 2,\"DeviationDelay\": 50}"
                  : "{\"AverageValue\": 24,\"DeviationValue\": 30,\"AverageDelay\": 2,\"DeviationDelay\": 50}",

               Name = "Voltage sensor #4 | Датчик напряжения #4",
               Description = "Voltage sensor #4 description | Датчик напряжения #4 описание",
               MeasureUnit = "V|В"
            },
            new Sensor()
            {
               SystemName = "ImpulseSensor#5",
               SensorType = SensorType.Scalar,
               PriorityType = PriorityType.Common,
               AdminComment = "Impulse sensor #5 admin comment",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = JsonSerializer.Serialize(new
               {
                     AverageValue = 500,
                     DeviationValue = 10,
                     AverageDelay = 7,
                     DeviationDelay = 50
               }),

               Name = "Impulse sensor #5 | Датчик импульсов #5",
               Description = "Impulse sensor #5 description | Датчик импульсов #5 описание",
               MeasureUnit = "imp|имп"
            },
            new Sensor()
            {
               SystemName = "AccelerationSensor#6",
               SensorType = SensorType.Scalar,
               PriorityType = PriorityType.Common,
               AdminComment = "Acceleration sensor #6 admin comment",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = JsonSerializer.Serialize(new
               {
                     AverageValue = 10,
                     DeviationValue = 30,
                     AverageDelay = 10,
                     DeviationDelay = 50
               }),

               Name = "Acceleration sensor #6 | Датчик ускорения #6",
               Description = "Acceleration sensor #6 description | Датчик ускорения #6 описание",
               MeasureUnit = "m/s2|м/c2"
            },
            new Sensor()
            {
               SystemName = "PowerSensor#7",
               SensorType = SensorType.Scalar,
               PriorityType = PriorityType.Common,
               AdminComment = "Power sensor #7 admin comment",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = JsonSerializer.Serialize(new
               {
                     AverageValue = 150,
                     DeviationValue = 20,
                     AverageDelay = 12,
                     DeviationDelay = 50
               }),

               Name = "Power sensor #7 | Датчик мощности #7",
               Description = "Power sensor #7 description | Датчик мощнотси #7 описание",
               MeasureUnit = "W|Вт"
            },
            new Sensor()
            {
               SystemName = "FrequencySensor#8",
               SensorType = SensorType.Scalar,
               PriorityType = PriorityType.Common,
               AdminComment = "Frequency sensor #8 admin comment",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = JsonSerializer.Serialize(new
               {
                     AverageValue = 50,
                     DeviationValue = 10,
                     AverageDelay = 10,
                     DeviationDelay = 30
               }),

               Name = "Frequency sensor #8 | Датчик частоты #8",
               Description = "Frequency sensor #8 description | Датчик частоты #8 описание",
               MeasureUnit = "Hz|Гц"
            },
            new Sensor()
            {
               SystemName = "WeightSensor#9",
               SensorType = SensorType.Scalar,
               PriorityType = PriorityType.Common,
               AdminComment = "Weight sensor #9 admin comment",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = JsonSerializer.Serialize(new
               {
                     AverageValue = 20000,
                     DeviationValue = 10,
                     AverageDelay = 15,
                     DeviationDelay = 30
               }),

               Name = "Weight sensor #9 | Датчик веса #9",
               Description = "Weight sensor #9 description | Датчик веса #8 описание",
               MeasureUnit = "kg|кг"
            },
            new Sensor()
            {
               SystemName = "ResistanceSensor#10",
               SensorType = SensorType.Scalar,
               PriorityType = PriorityType.Common,
               AdminComment = "Resistance sensor #10 admin comment",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = JsonSerializer.Serialize(new
               {
                     AverageValue = 15,
                     DeviationValue = 50,
                     AverageDelay = 10,
                     DeviationDelay = 50
               }),

               Name = "Resistance sensor #10 | Датчик сопротивления #10",
               Description = "Resistance sensor #10 description | Датчик сопротивления #10 описание",
               MeasureUnit = "Ω|Ом"
            },
            new Sensor()
            {
               SystemName = "GNSSTracker#1",
               SensorType = SensorType.Spatial,
               PriorityType = PriorityType.Common,
               AdminComment = "GNSS tracker #1 example",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = "{}",

               Name = "GNSS tracker #1 | ГНСС трекер #1",
               Description = "GNSS tracker #1 description | ГНСС трекер #1 описание",
            },
            new Sensor()
            {
               SystemName = "IPCam#1",
               SensorType = SensorType.MediaStream,
               PriorityType = PriorityType.Common,
               AdminComment = "IP Camera #1 stream example",
               Enabled = true,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = JsonSerializer.Serialize(new
               {
                     SourceUrl = "rtsp://device:Mnbvcxz1@192.168.0.155:554",
                     VideoSize= "640x360",
               }),

               Name = "IP Camera #1 | IP камера #1",
               Description = "IP Camera #1 description | IP камера #1 описание",
            },
            new Sensor()
            {
               SystemName = "IPCam#2",
               SensorType = SensorType.MediaStream,
               PriorityType = PriorityType.Common,
               AdminComment = "IP Camera #2 stream example",
               Enabled = false,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = JsonSerializer.Serialize(new
               {
                     SourceUrl = "rtsp://device:Mnbvcxz1@192.168.0.155:554",
                     VideoSize= "720x420",
               }),

               Name = "IP Camera #2 | IP камера #2",
               Description = "IP Camera #2 description | IP камера #2 описание",
            },
            new Sensor()
            {
               SystemName = "IPCam#3",
               SensorType = SensorType.MediaStream,
               PriorityType = PriorityType.Common,
               AdminComment = "IP Camera #3 stream example",
               Enabled = false,
               DisplayOrder = 0,
               SubjectToAcl = true,
               IsDeleted = false,
               CreatedOnUtc = DateTime.UtcNow,
               DeviceId = device.Id,
               Configuration = JsonSerializer.Serialize(new
               {
                     SourceUrl = "rtsp://device:Mnbvcxz1@192.168.0.155:554",
                     VideoSize= "1280x540",
               }),

               Name = "IP Camera #3 | IP камера #3",
               Description = "IP Camera #3 description | IP камера #3 описание",
            }
         });

         // initial device configuration version
         await InsertInstallationDataAsync(new GenericAttribute()
         {
            EntityId = device.Id,
            KeyGroup = nameof(Device),
            Key = ClientDefaults.DeviceConfigurationVersion,
            Value = DateTime.UtcNow.Ticks.ToString(),
            CreatedOrUpdatedDateUTC = DateTime.UtcNow
         });
      }

      await InsertInstallationDataAsync(sensorList);

      // localization
      var locales = new List<LocalizedProperty>();

      foreach (var language in _languageRepository.Table)
      {
         var isEng = language.UniqueSeoCode.Equals("en", StringComparison.InvariantCultureIgnoreCase);
         var isRu = language.UniqueSeoCode.Equals("ru", StringComparison.InvariantCultureIgnoreCase);
         if (isEng || isRu)
         {
            foreach (var device in deviceList)
            {
               var name = isEng ? device.Name?.Split('|')[0].Trim() : device.Name?.Split('|')[1].Trim();
               if (!string.IsNullOrEmpty(name))
               {
                  var deviceName = new LocalizedProperty()
                  {
                     LanguageId = language.Id,
                     EntityId = device.Id,
                     LocaleKeyGroup = nameof(Device),
                     LocaleKey = nameof(Device.Name),
                     LocaleValue = name
                  };

                  locales.Add(deviceName);
               }

               var description = isEng ? device.Description?.Split('|')[0].Trim() : device.Description?.Split('|')[1].Trim();
               if (!string.IsNullOrEmpty(description))
               {
                  var deviceDescription = new LocalizedProperty()
                  {
                     LanguageId = language.Id,
                     EntityId = device.Id,
                     LocaleKeyGroup = nameof(Device),
                     LocaleKey = nameof(Device.Description),
                     LocaleValue = description
                  };

                  locales.Add(deviceDescription);
               }
            }

            foreach (var sensor in sensorList)
            {
               var name = isEng ? sensor.Name?.Split('|')[0].Trim() : sensor.Name?.Split('|')[1].Trim();
               if (!string.IsNullOrEmpty(name))
               {
                  var sensorName = new LocalizedProperty()
                  {
                     LanguageId = language.Id,
                     EntityId = sensor.Id,
                     LocaleKeyGroup = nameof(Sensor),
                     LocaleKey = nameof(Sensor.Name),
                     LocaleValue = name
                  };

                  locales.Add(sensorName);
               }

               var description = isEng ? sensor.Description?.Split('|')[0].Trim() : sensor.Description?.Split('|')[1].Trim();
               if (!string.IsNullOrEmpty(description))
               {
                  var sensorDescription = new LocalizedProperty()
                  {
                     LanguageId = language.Id,
                     EntityId = sensor.Id,
                     LocaleKeyGroup = nameof(Sensor),
                     LocaleKey = nameof(Sensor.Description),
                     LocaleValue = description
                  };

                  locales.Add(sensorDescription);
               }

               var mu = isEng ? sensor.MeasureUnit?.Split('|')[0].Trim() : sensor.MeasureUnit?.Split('|')[1].Trim();
               if (!string.IsNullOrEmpty(mu))
               {
                  var sensorMeasureDevice = new LocalizedProperty()
                  {
                     LanguageId = language.Id,
                     EntityId = sensor.Id,
                     LocaleKeyGroup = nameof(Sensor),
                     LocaleKey = nameof(Sensor.MeasureUnit),
                     LocaleValue = mu
                  };

                  locales.Add(sensorMeasureDevice);
               }
            }
         }
      }

      if (_appSettings.Get<InstallationConfig>().InstallSampleData.InstallJunk)
         await InstallJunkSampleDataAsync(users.ToArray(), locales);

      await InsertInstallationDataAsync(locales);
   }


   /// <returns>A task that represents the asynchronous operation</returns>
   private async Task InstallJunkSampleDataAsync(User[] users, List<LocalizedProperty> locales)
   {
      #region users

      var junkUsersCount = _appSettings.Get<InstallationConfig>().InstallSampleData.SampleUsersCount;

      // users
      var junkUsers = new List<User>();
      for (var i = 0; i < junkUsersCount; i++)
      {
         junkUsers.Add(new User
         {
            UserGuid = Guid.NewGuid(),
            Email = $"testuser{i + 1}@application.com",
            Username = $"testuser{i + 1}",
            IsActive = true,
            CreatedOnUtc = DateTime.UtcNow,
            LastActivityUtc = DateTime.UtcNow,
         });
      }
      await InsertInstallationDataAsync(junkUsers);

      // address
      var junkUserAddresses = new List<Address>();
      var stateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California")?.Id;
      var countryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id;
      for (var i = 0; i < junkUsersCount; i++)
      {
         junkUserAddresses.Add(new Address
         {
            FirstName = $"Test{i + 1}",
            LastName = $"User{i + 1}",
            PhoneNumber = (87654321 + i).ToString(),
            Email = $"testuser{i + 1}@application.com",
            FaxNumber = string.Empty,
            Company = $"Test Company{i + 1}",
            Address1 = $"{750 + i} Bel Air Rd.",
            Address2 = string.Empty,
            City = "Los Angeles",
            StateProvinceId = stateProvinceId,
            CountryId = countryId,
            ZipPostalCode = "90077",
            CreatedOnUtc = DateTime.UtcNow
         });
      }
      await InsertInstallationDataAsync(junkUserAddresses);

      // address mapping
      var junkUserAddressMapping = new List<UserAddress>();
      for (var i = 0; i < junkUsersCount; i++) junkUserAddressMapping.Add(new UserAddress() { AddressId = junkUserAddresses[i].Id, UserId = junkUsers[i].Id });
      await InsertInstallationDataAsync(junkUserAddressMapping);

      // attrs
      var junkUserAttributes = new List<GenericAttribute>();
      for (int i = 0; i < junkUsersCount; i++)
      {
         //set default user name
         junkUserAttributes.AddRange(new List<GenericAttribute>()
         {
            new GenericAttribute
            {
               EntityId = junkUsers[i].Id,
               Key = AppUserDefaults.FirstNameAttribute,
               KeyGroup = nameof(User),
               Value = junkUserAddresses[i].FirstName,
               CreatedOrUpdatedDateUTC = DateTime.UtcNow
            },
            new GenericAttribute
            {
               EntityId = junkUsers[i].Id,
               Key = AppUserDefaults.LastNameAttribute,
               KeyGroup = nameof(User),
               Value = junkUserAddresses[i].LastName,
               CreatedOrUpdatedDateUTC = DateTime.UtcNow
            }
         });
      }
      await InsertInstallationDataAsync(junkUserAttributes);

      //set user password
      var junkUserPasswords = new List<UserPassword>();
      for (int i = 0; i < junkUsersCount; i++)
      {
         junkUserPasswords.Add(new UserPassword
         {
            UserId = junkUsers[i].Id,
            Password = "123456",
            PasswordFormat = PasswordFormat.Clear,
            PasswordSalt = string.Empty,
            CreatedOnUtc = DateTime.UtcNow
         });
      }
      await InsertInstallationDataAsync(junkUserPasswords);

      // add roles
      var roles = _userRoleRepository.Table
         .Where(x => x.SystemName == UserDefaults.RegisteredRoleName || x.SystemName == UserDefaults.OwnersRoleName || x.SystemName == UserDefaults.OperatorsRoleName);

      var userRoleMappings =
         (from u in junkUsers
          from r in roles
          select new UserUserRole() { UserId = u.Id, UserRoleId = r.Id })
          .ToList();

      await InsertInstallationDataAsync(userRoleMappings);

      #endregion

      #region sample monitors

      var monitorsCount = _appSettings.Get<InstallationConfig>().InstallSampleData.SampleMonitorsCount;

      // monitors
      var monitors = new List<Monitor>();

      for (int i = 0; i < monitorsCount; i++)
      {
         var index = Random.Shared.Next(0, users.Length);
         var monitor = new Monitor()
         {
            IsDeleted = i == 3,
            DisplayOrder = Random.Shared.Next(1, 16),
            AdminComment = "Some admin comment",
            CreatedOnUtc = DateTime.UtcNow,
            OwnerId = users[index].Id,
            ShowInMenu = Random.Shared.NextDouble() >= 0.5,
            UpdatedOnUtc = DateTime.UtcNow,
         };

         monitors.Add(monitor);
      }

      await InsertInstallationDataAsync(monitors);

      // mapping to customer
      var userMonitorMaps = new List<UserMonitor>();

      foreach (var monitor in monitors)
      {
         int index = 0;
         do { index = Random.Shared.Next(0, users.Length); }
         while (monitor.OwnerId == users[index].Id);

         var mapMonitorToUser = new UserMonitor()
         {
            UserId = users[index].Id,
            MonitorId = monitor.Id,
            ShowInMenu = Random.Shared.NextDouble() >= 0.5,
            DisplayOrder = Random.Shared.Next(1, 16),
         };

         userMonitorMaps.Add(mapMonitorToUser);
      }

      await InsertInstallationDataAsync(userMonitorMaps.DistinctBy(x => new { x.UserId, x.MonitorId }).ToList());

      foreach (var monitor in monitors)
      {
         foreach (var language in _languageRepository.Table)
         {
            var isEng = language.UniqueSeoCode.Equals("en", StringComparison.InvariantCultureIgnoreCase);
            var isRu = language.UniqueSeoCode.Equals("ru", StringComparison.InvariantCultureIgnoreCase);
            if (isEng || isRu)
            {
               var localeMenuitem = new LocalizedProperty()
               {
                  LanguageId = language.Id,
                  EntityId = monitor.Id,
                  LocaleKeyGroup = nameof(Monitor),
                  LocaleKey = nameof(Monitor.MenuItem),
                  LocaleValue = isEng ? $"Monitor #{monitor.Id}" : $"Монитор #{monitor.Id}"
               };

               locales.Add(localeMenuitem);

               var localeName = new LocalizedProperty()
               {
                  LanguageId = language.Id,
                  EntityId = monitor.Id,
                  LocaleKeyGroup = nameof(Monitor),
                  LocaleKey = nameof(Monitor.Name),
                  LocaleValue = isEng ? $"Monitor#{monitor.Id}" : $"Монитор#{monitor.Id}"
               };

               locales.Add(localeName);

               var localeDescription = new LocalizedProperty()
               {
                  LanguageId = language.Id,
                  EntityId = monitor.Id,
                  LocaleKeyGroup = nameof(Monitor),
                  LocaleKey = nameof(Monitor.Description),
                  LocaleValue = isEng ? $"Monitor #{monitor.Id} short description" : $"Монитор #{monitor.Id} краткое описание"
               };

               locales.Add(localeDescription);
            }
         }
      }

      #endregion

      #region sample devices

      #region locations
      var deviceLocations = new double[][]
      {
         [51.63831359799488, 39.25300496893483],
         [51.92862134801934, 39.21408065161984],
         [52.56411925590882, 39.63430615249749],
         [52.49580702885951, 39.83322364445875],
         [51.60265199786437, 39.24665923012],
         [51.24228362651933, 37.78983887249942],
         [51.26200399800287, 37.66188731888211],
         [55.644534317199685, 37.81721264972737],
         [54.692267718708436, 20.441483495135063],
         [43.103712367802274, 131.91151137304703 ],
         [55.06483430959536, 82.98062874494003 ],
         [59.923310452666264, 30.256607455883806],
         [25.892878139519993, 51.54186694179891],
         [70.451871014163, 68.28816824316088],
         [71.2659065624681, 72.06837561725204],
         [69.70972710307346, 170.30686294404538],
         [52.53862842332021, 158.20203094973206],
         [46.63041761245641, 142.9112972379921],
         [62.538168260956226, 114.00756408816345],
         [22.371032161736416, 114.09553887340921],
         [31.246564862420193, 121.74469243713756],
         [1.228823261676004, 103.76767241959821],
         [-22.25611515612784, 166.44242585192004],
         [19.02072130202616, 72.89941652627132],
         [-34.006120084893624, 18.500415234377744],
         [-23.759306889222355, -46.58045824601649],
         [32.946508136272996, 3.2384377054600573],
         [40.71206394635603, -74.13608047220428],
         [37.82452313536839, -122.29591030898014],
         [-18.198065571008375, 49.359018508042304],
         [-49.349252509221635, 70.26555974770972],
         [-22.311608503954066, -68.89388456357842]
      };
      #endregion

      var devicesCount = _appSettings.Get<InstallationConfig>().InstallSampleData.SampleDevicesCount;

      var devices = new List<Device>();

      // units
      for (int i = 0; i < devicesCount; i++)
      {
         var index = Random.Shared.Next(1, users.Length);
         var unit = new Device()
         {
            IsDeleted = i == 3,
            DisplayOrder = Random.Shared.Next(1, 16),
            Enabled = Random.Shared.NextDouble() >= 0.5,
            Guid = Guid.NewGuid(),
            IsActive = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            LastActivityOnUtc = i < 10 ? DateTime.UtcNow : null,
            OwnerId = users[index].Id,
            SystemName = $"SampleController#{i + 1}",
            Configuration = "{}",
         };

         if (i < deviceLocations.Length)
         {
            unit.IsMobile = false;
            unit.Lat = deviceLocations[i][0];
            unit.Lon = deviceLocations[i][1];
            unit.ShowOnMain = true;
         }

         devices.Add(unit);
      }

      await InsertInstallationDataAsync(devices);

      // mapping shared users
      var deviceToUserMaps = new List<UserDevice>();
      foreach (var device in devices)
      {
         int index;
         do { index = Random.Shared.Next(0, users.Length); }
         while (device.OwnerId == users[index].Id);

         var mapDeviceToUser = new UserDevice()
         {
            UserId = users[index].Id,
            DeviceId = device.Id
         };

         deviceToUserMaps.Add(mapDeviceToUser);
      }
      await InsertInstallationDataAsync(deviceToUserMaps);

      foreach (var device in devices)
      {
         foreach (var language in _languageRepository.Table)
         {
            var isEng = language.UniqueSeoCode.Equals("en", StringComparison.InvariantCultureIgnoreCase);
            var isRu = language.UniqueSeoCode.Equals("ru", StringComparison.InvariantCultureIgnoreCase);
            if (isEng || isRu)
            {
               var localeName = new LocalizedProperty()
               {
                  LanguageId = language.Id,
                  EntityId = device.Id,
                  LocaleKeyGroup = nameof(Device),
                  LocaleKey = nameof(Device.Name),
                  LocaleValue = isEng ? $"Device#{device.Id}" : $"Устройство#{device.Id}"
               };

               locales.Add(localeName);

               var localeDescription = new LocalizedProperty()
               {
                  LanguageId = language.Id,
                  EntityId = device.Id,
                  LocaleKeyGroup = nameof(Device),
                  LocaleKey = nameof(Device.Description),
                  LocaleValue = isEng ? $"Device #{device.Id} short description" : $"Устройство #{device.Id} краткое описание"
               };

               locales.Add(localeDescription);
            }
         }
      }

      #endregion

      #region sensors

      var sensorsCount = _appSettings.Get<InstallationConfig>().InstallSampleData.SampleSensorsCount;

      // sensors
      var sensors = new List<Sensor>();

      foreach (var device in devices)
      {
         for (int i = 0; i < sensorsCount; i++)
         {
            var sensor = new Sensor()
            {
               DeviceId = device.Id,
               IsDeleted = i == 5,
               DisplayOrder = Random.Shared.Next(0, 16),
               Enabled = Random.Shared.Next(-10, 10) > 0,
               SensorType = SensorType.Scalar,
               PriorityType = (PriorityType)((int)Math.Round(Random.Shared.Next(10, 50) / 10D, 0)),
               CreatedOnUtc = DateTime.UtcNow,
               UpdatedOnUtc = DateTime.UtcNow,
               Configuration = "{}",
               AdminComment = "Some admin comment",
            };

            sensor.SystemName = $"Sensor#{i}";

            sensors.Add(sensor);
         }
      }

      await InsertInstallationDataAsync(sensors);

      foreach (var sensor in sensors)
      {
         foreach (var language in _languageRepository.Table)
         {
            var isEng = language.UniqueSeoCode.Equals("en", StringComparison.InvariantCultureIgnoreCase);
            var isRu = language.UniqueSeoCode.Equals("ru", StringComparison.InvariantCultureIgnoreCase);
            if (isEng || isRu)
            {
               var localeName = new LocalizedProperty()
               {
                  LanguageId = language.Id,
                  EntityId = sensor.Id,
                  LocaleKeyGroup = nameof(Sensor),
                  LocaleKey = nameof(Sensor.Name),
                  LocaleValue = isEng ? $"Sensor#{sensor.Id}" : $"Сенсор#{sensor.Id}"
               };

               locales.Add(localeName);

               var localeDescription = new LocalizedProperty()
               {
                  LanguageId = language.Id,
                  EntityId = sensor.Id,
                  LocaleKeyGroup = nameof(Sensor),
                  LocaleKey = nameof(Sensor.Description),
                  LocaleValue = isEng ? $"Sensor #{sensor.Id} short description" : $"Сенсор #{sensor.Id} краткое описание"
               };

               locales.Add(localeDescription);

               var localeMeasureUnit = new LocalizedProperty()
               {
                  LanguageId = language.Id,
                  EntityId = sensor.Id,
                  LocaleKeyGroup = nameof(Sensor),
                  LocaleKey = nameof(Sensor.MeasureUnit),
                  LocaleValue = isEng ? $"mu" : $"еи"
               };

               locales.Add(localeMeasureUnit);
            }
         }
      }

      #endregion

      #region widgets

      var widgetsCount = _appSettings.Get<InstallationConfig>().InstallSampleData.SampleWidgetsCount;
      var widgetTypeEnumLenght = Enum.GetNames<PriorityType>().Length;
      var widgets = new List<Widget>();

      for (int i = 0; i < widgetsCount; i++)
      {
         var index = Random.Shared.Next(0, users.Length);
         var widget = new Widget()
         {
            AdminComment = "Some admin comment",
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            IsDeleted = i == 5,
            DisplayOrder = Random.Shared.Next(0, 16),
            Enabled = true,
            UserId = users[index].Id,
            WidgetType = (WidgetType)Random.Shared.Next(1, widgetTypeEnumLenght),
            SubjectToAcl = false,
            Adjustment = JsonSerializer.Serialize(new
            {
               HistoryPointsCount = 120,
               MaxCriticalValue = 120,
               MaxValue = 100,
               MinValue = -100,
               MinCriticalValue = -120,
               ShowHistory = true,
               SmothHistoryChart = true,
               ShowHistoryTrends = true,
               ShowCriticalValueNotification = true,
               ShowHistoryAnnotations = true,
               UseValueConstraint = true,
               ShowAsAreachart = true,
               ShowNotificationForAll = false,
            }),
         };

         widgets.Add(widget);
      }

      await InsertInstallationDataAsync(widgets);

      // locales
      foreach (var widget in widgets)
      {
         foreach (var language in _languageRepository.Table)
         {
            var isEng = language.UniqueSeoCode.Equals("en", StringComparison.InvariantCultureIgnoreCase);
            var isRu = language.UniqueSeoCode.Equals("ru", StringComparison.InvariantCultureIgnoreCase);
            if (isEng || isRu)
            {
               var localeName = new LocalizedProperty()
               {
                  LanguageId = language.Id,
                  EntityId = widget.Id,
                  LocaleKeyGroup = nameof(Widget),
                  LocaleKey = nameof(Widget.Name),
                  LocaleValue = isEng ? $"{widget.WidgetType}-widget#{widget.Id}" : $"{widget.WidgetType}-виджет#{widget.Id}"
               };

               locales.Add(localeName);

               var localeDescription = new LocalizedProperty()
               {
                  LanguageId = language.Id,
                  EntityId = widget.Id,
                  LocaleKeyGroup = nameof(Widget),
                  LocaleKey = nameof(Widget.Description),
                  LocaleValue = isEng ? $"{widget.WidgetType} widget #{widget.Id} description" : $"{widget.WidgetType}-виджет #{widget.Id} описание"
               };

               locales.Add(localeDescription);
            }
         }
      }

      #endregion
   }

   #endregion

   #region Methods

   /// <summary>
   /// Install required data
   /// </summary>
   /// <param name="defaultUserEmail">Default user email</param>
   /// <param name="defaultUserPassword">Default user password</param>
   /// <param name="regionInfo">RegionInfo</param>
   /// <param name="cultureInfo">CultureInfo</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InstallRequiredDataAsync(string defaultUserEmail, string defaultUserPassword, RegionInfo regionInfo, CultureInfo cultureInfo)
   {
      await InstallAppInformationAsync();
      await InstallMeasuresAsync(regionInfo);
      await InstallLanguagesAsync(cultureInfo, regionInfo);
      await InstallCurrenciesAsync(cultureInfo, regionInfo);
      await InstallCountriesAndStatesAsync();
      await InstallEmailAccountsAsync();
      await InstallMessageTemplatesAsync();
      await InstallTopicTemplatesAsync();
      await InstallSettingsAsync(regionInfo);
      await InstallUsersAsync(defaultUserEmail, defaultUserPassword);
      await InstallTopicsAsync();
      await InstallActivityLogTypesAsync();
      await InstallScheduleTasksAsync();
   }

   /// <summary>
   /// Install sample data
   /// </summary>
   /// <param name="defaultUserEmail">Default user email</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InstallSampleDataAsync(string defaultUserEmail)
   {
      await InstallSampleUsersAsync();
      await InstallForumsAsync();
      await InstallBlogPostsAsync(defaultUserEmail);
      await InstallNewsAsync(defaultUserEmail);
      await InstallPollsAsync();
      await InstallActivityLogAsync(defaultUserEmail);
      await InstallSearchTermsAsync();
      await InstallSampleTelemetry(defaultUserEmail);

      var settingService = EngineContext.Current.Resolve<ISettingService>();

      await settingService.SaveSettingAsync(new DisplayDefaultFooterItemSettings
      {
         DisplayBlogFooterItem = true,
         DisplayForumsFooterItem = true,
         DisplayNewsFooterItem = true,
         DisplayContactUsFooterItem = true,
         DisplaySearchFooterItem = false,
         DisplaySitemapFooterItem = true,
         DisplayUserAddressesFooterItem = true,
         DisplayUserInfoFooterItem = true,
         DisplayWishlistFooterItem = false,
      });
   }

   #endregion
}