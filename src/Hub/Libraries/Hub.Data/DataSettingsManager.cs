using System.Collections.Generic;
using Hub.Core.Configuration;
using Hub.Core.Infrastructure;
using Hub.Data.Configuration;

namespace Hub.Data
{
   /// <summary>
   /// Represents the data settings manager
   /// </summary>
   public partial class DataSettingsManager
   {
      #region Fields

      /// <summary>
      /// Gets a cached value indicating whether the database is installed. We need this value invariable during installation process
      /// </summary>
      private static bool? _databaseIsInstalled;

      #endregion

      #region Methods

      /// <summary>
      /// Load data settings
      /// </summary>
      /// <param name="reload">Force loading settings from disk</param>
      /// <returns>Data settings</returns>
      public static DataConfig LoadSettings(bool reload = false)
      {
         if (!reload && Singleton<DataConfig>.Instance is not null)
            return Singleton<DataConfig>.Instance;
         
         Singleton<DataConfig>.Instance = Singleton<AppSettings>.Instance.Get<DataConfig>();

         return Singleton<DataConfig>.Instance;
      }

      /// <summary>
      /// Save data settings
      /// </summary>
      /// <param name="dataSettings">Data settings</param>
      /// <param name="fileProvider">File provider</param>
      public static void SaveSettings(DataConfig dataSettings, IAppFileProvider fileProvider)
      {
         AppSettingsHelper.SaveAppSettings(new List<IConfig> { dataSettings }, fileProvider);
         LoadSettings(reload: true);
      }

      /// <summary>
      /// Gets a value indicating whether database is already installed
      /// </summary>
      public static bool IsDatabaseInstalled()
      {
         _databaseIsInstalled ??= !string.IsNullOrEmpty(LoadSettings()?.ConnectionString);

         return _databaseIsInstalled.Value;
      }

      /// <summary>
      /// Gets the command execution timeout.
      /// </summary>
      /// <value>
      /// Number of seconds. Negative timeout value means that a default timeout will be used. 0 timeout value corresponds to infinite timeout.
      /// </value>
      public static int GetSqlCommandTimeout()
      {
         return LoadSettings()?.SQLCommandTimeout ?? -1;
      }

      #endregion
   }
}