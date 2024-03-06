using Hub.Core.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hub.Services.Media.RoxyFileman;

/// <summary>
/// Represents errors that occur when working with uploaded files
/// </summary>
public class RoxyFilemanException : Exception
{
   #region Fields

   private static Dictionary<string, string> _languageResources;

   #endregion

   #region Ctor

   /// <summary>
   /// Default Ctor
   /// </summary>
   private RoxyFilemanException() : base()
   {
   }

   /// <summary>
   /// Initializes a new instance of the RoxyFilemanException
   /// </summary>
   /// <param name="key">Roxy language resource key</param>
   public RoxyFilemanException(string key) : base(GetLocalizedMessage(key))
   {
   }

   /// <summary>
   /// Initializes a new instance of the RoxyFilemanException
   /// </summary>
   /// <param name="key">Roxy language resource key</param>
   /// <param name="innerException">The exception that is the cause of the current exception</param>
   public RoxyFilemanException(string key, Exception innerException) : base(GetLocalizedMessage(key), innerException)
   {
   }

   #endregion

   #region Utils

   /// <summary>
   /// Get the language resource value
   /// </summary>
   /// <param name="key">Language resource key</param>
   /// <returns>
   /// The language resource value
   /// </returns>
   protected static string GetLocalizedMessage(string key)
   {
      var fileProvider = EngineContext.Current.Resolve<IAppFileProvider>();

      var roxyConfig = Singleton<RoxyFilemanConfig>.Instance;
      var languageFile = fileProvider.GetAbsolutePath($"{AppRoxyFilemanDefaults.LanguageDirectory}/{roxyConfig.LANG}.json");

      if (!fileProvider.FileExists(languageFile))
         languageFile = fileProvider.GetAbsolutePath($"{AppRoxyFilemanDefaults.LanguageDirectory}/en.json");

      if (_languageResources is null)
      {
         var json = fileProvider.ReadAllTextAsync(languageFile, Encoding.UTF8).Result;
         _languageResources = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
      }

      if (_languageResources is null)
         return key;

      if (_languageResources.TryGetValue(key, out var value))
         return value;

      return key;
   }

   #endregion
}