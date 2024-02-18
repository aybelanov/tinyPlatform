using ClosedXML.Excel;
using Hub.Core;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.Messages;
using Hub.Core.Http;
using Hub.Core.Infrastructure;
using Hub.Services.Directory;
using Hub.Services.ExportImport.Help;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Media;
using Hub.Services.Messages;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hub.Services.ExportImport;

/// <summary>
/// Import manager
/// </summary>
public partial class ImportManager : IImportManager
{
   #region Fields

   private readonly ICountryService _countryService;
   private readonly IUserActivityService _userActivityService;
   private readonly IHttpClientFactory _httpClientFactory;
   private readonly ILocalizationService _localizationService;
   private readonly ILogger _logger;
   private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
   private readonly IAppFileProvider _fileProvider;
   private readonly IPictureService _pictureService;
   private readonly IStateProvinceService _stateProvinceService;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public ImportManager(ICountryService countryService,
       IUserActivityService userActivityService,
       IHttpClientFactory httpClientFactory,
       ILocalizationService localizationService,
       ILogger logger,
       INewsLetterSubscriptionService newsLetterSubscriptionService,
       IAppFileProvider fileProvider,
       IPictureService pictureService,
       IStateProvinceService stateProvinceService)
   {
      _countryService = countryService;
      _userActivityService = userActivityService;
      _httpClientFactory = httpClientFactory;
      _fileProvider = fileProvider;
      _localizationService = localizationService;
      _logger = logger;
      _newsLetterSubscriptionService = newsLetterSubscriptionService;
      _pictureService = pictureService;
      _stateProvinceService = stateProvinceService;
   }

   #endregion

   #region Utilities

   /// <returns>A task that represents the asynchronous operation</returns>
   private static Task SetOutLineForSpecificationAttributeRowAsync(object cellValue, IXLWorksheet worksheet, int endRow)
   {
      var attributeType = (cellValue ?? string.Empty).ToString();

      if (attributeType.Equals("AttributeType", StringComparison.InvariantCultureIgnoreCase))
      {
         worksheet.Row(endRow).OutlineLevel = 1;
      }

      return Task.CompletedTask;
   }

   /// <returns>Column index</returns>
   protected virtual int GetColumnIndex(string[] properties, string columnName)
   {
      if (properties == null)
         throw new ArgumentNullException(nameof(properties));

      if (columnName == null)
         throw new ArgumentNullException(nameof(columnName));

      for (var i = 0; i < properties.Length; i++)
         if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
            return i + 1; //excel indexes start from 1
      return 0;
   }

   /// <returns>Mime</returns>
   protected virtual string GetMimeTypeFromFilePath(string filePath)
   {
      new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);

      //set to jpeg in case mime type cannot be found
      return mimeType ?? MimeTypes.ImageJpeg;
   }

   /// <summary>
   /// Creates or loads the image
   /// </summary>
   /// <param name="picturePath">The path to the image file</param>
   /// <param name="name">The name of the object</param>
   /// <param name="picId">Image identifier, may be null</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the image or null if the image has not changed
   /// </returns>
   protected virtual async Task<Picture> LoadPictureAsync(string picturePath, string name, int? picId = null)
   {
      if (string.IsNullOrEmpty(picturePath) || !_fileProvider.FileExists(picturePath))
         return null;

      var mimeType = GetMimeTypeFromFilePath(picturePath);
      var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
      var pictureAlreadyExists = false;
      if (picId != null)
      {
         //compare with existing product pictures
         var existingPicture = await _pictureService.GetPictureByIdAsync(picId.Value);
         if (existingPicture != null)
         {
            var existingBinary = await _pictureService.LoadPictureBinaryAsync(existingPicture);
            //picture binary after validation (like in database)
            var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType);
            if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                existingBinary.SequenceEqual(newPictureBinary))
            {
               pictureAlreadyExists = true;
            }
         }
      }

      if (pictureAlreadyExists)
         return null;

      var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, await _pictureService.GetPictureSeNameAsync(name));
      return newPicture;
   }

   /// <returns>A task that represents the asynchronous operation</returns>
   private async Task LogPictureInsertErrorAsync(string picturePath, Exception ex)
   {
      var extension = _fileProvider.GetFileExtension(picturePath);
      var name = _fileProvider.GetFileNameWithoutExtension(picturePath);

      var point = string.IsNullOrEmpty(extension) ? string.Empty : ".";
      var fileName = _fileProvider.FileExists(picturePath) ? $"{name}{point}{extension}" : string.Empty;

      await _logger.ErrorAsync($"Insert picture failed (file name: {fileName})", ex);
   }

   /// <returns>A task that represents the asynchronous operation</returns>
   private async Task<string> DownloadFileAsync(string urlString, IList<string> downloadedFiles)
   {
      if (string.IsNullOrEmpty(urlString))
         return string.Empty;

      if (!Uri.IsWellFormedUriString(urlString, UriKind.Absolute))
         return urlString;

      //ensure that temp directory is created
      var tempDirectory = _fileProvider.MapPath(ExportImportDefaults.UploadsTempPath);
      _fileProvider.CreateDirectory(tempDirectory);

      var fileName = _fileProvider.GetFileName(urlString);
      if (string.IsNullOrEmpty(fileName))
         return string.Empty;

      var filePath = _fileProvider.Combine(tempDirectory, fileName);
      try
      {
         var client = _httpClientFactory.CreateClient(AppHttpDefaults.DefaultHttpClient);
         var fileData = await client.GetByteArrayAsync(urlString);
         await using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            fs.Write(fileData, 0, fileData.Length);

         downloadedFiles?.Add(filePath);
         return filePath;
      }
      catch (Exception ex)
      {
         await _logger.ErrorAsync("Download image failed", ex);
      }

      return string.Empty;
   }



   #endregion

   #region Methods

   /// <summary>
   /// Get property list by excel cells
   /// </summary>
   /// <typeparam name="T">Type of object</typeparam>
   /// <param name="worksheet">Excel worksheet</param>
   /// <returns>Property list</returns>
   public static IList<PropertyByName<T>> GetPropertiesByExcelCells<T>(IXLWorksheet worksheet)
   {
      var properties = new List<PropertyByName<T>>();
      var poz = 1;
      while (true)
      {
         try
         {
            var cell = worksheet.Row(1).Cell(poz);

            if (string.IsNullOrEmpty(cell?.Value.ToString()))
               break;

            poz += 1;
            properties.Add(new PropertyByName<T>(cell.Value.ToString()));
         }
         catch
         {
            break;
         }
      }

      return properties;
   }

   /// <summary>
   /// Import newsletter subscribers from TXT file
   /// </summary>
   /// <param name="stream">Stream</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the number of imported subscribers
   /// </returns>
   public virtual async Task<int> ImportNewsletterSubscribersFromTxtAsync(Stream stream)
   {
      var count = 0;
      using (var reader = new StreamReader(stream))
         while (!reader.EndOfStream)
         {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
               continue;
            var tmp = line.Split(',');

            if (tmp.Length > 3)
               throw new AppException("Wrong file format");

            var isActive = true;

            //"email" field specified
            var email = tmp[0].Trim();

            if (!CommonHelper.IsValidEmail(email))
               continue;

            //"active" field specified
            if (tmp.Length >= 2)
               isActive = bool.Parse(tmp[1].Trim());

            //import
            var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAsync(email);
            if (subscription != null)
            {
               subscription.Email = email;
               subscription.Active = isActive;
               await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
            }
            else
            {
               subscription = new NewsLetterSubscription
               {
                  Active = isActive,
                  CreatedOnUtc = DateTime.UtcNow,
                  Email = email,
                  NewsLetterSubscriptionGuid = Guid.NewGuid()
               };
               await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
            }

            count++;
         }

      return count;
   }

   /// <summary>
   /// Import states from TXT file
   /// </summary>
   /// <param name="stream">Stream</param>
   /// <param name="writeLog">Indicates whether to add logging</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the number of imported states
   /// </returns>
   public virtual async Task<int> ImportStatesFromTxtAsync(Stream stream, bool writeLog = true)
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

            var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryTwoLetterIsoCode);
            if (country == null)
            {
               //country cannot be loaded. skip
               continue;
            }

            //import
            var states = await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id, showHidden: true);
            var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (state != null)
            {
               state.Abbreviation = abbreviation;
               state.Published = published;
               state.DisplayOrder = displayOrder;
               await _stateProvinceService.UpdateStateProvinceAsync(state);
            }
            else
            {
               state = new StateProvince
               {
                  CountryId = country.Id,
                  Name = name,
                  Abbreviation = abbreviation,
                  Published = published,
                  DisplayOrder = displayOrder
               };
               await _stateProvinceService.InsertStateProvinceAsync(state);
            }

            count++;
         }
      }

      //activity log
      if (writeLog)
      {
         await _userActivityService.InsertActivityAsync("ImportStates",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportStates"), count));
      }

      return count;
   }

   #endregion

}