using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hub.Core.Configuration;
using Hub.Core.Domain.Configuration;

namespace Hub.Services.Configuration
{
   /// <summary>
   /// Setting service interface
   /// </summary>
   public partial interface ISettingService
   {
      /// <summary>
      /// Gets a setting by identifier
      /// </summary>
      /// <param name="settingId">Setting identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the setting
      /// </returns>
      Task<Setting> GetSettingByIdAsync(long settingId);

      /// <summary>
      /// Deletes a setting
      /// </summary>
      /// <param name="setting">Setting</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteSettingAsync(Setting setting);

      /// <summary>
      /// Deletes settings
      /// </summary>
      /// <param name="settings">Settings</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteSettingsAsync(IList<Setting> settings);

      /// <summary>
      /// Get setting by key
      /// </summary>
      /// <param name="key">Key</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the setting
      /// </returns>
      Task<Setting> GetSettingAsync(string key);

      /// <summary>
      /// Get setting value by key
      /// </summary>
      /// <typeparam name="T">Type</typeparam>
      /// <param name="key">Key</param>
      /// <param name="defaultValue">Default value</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the setting value
      /// </returns>
      Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default);

      /// <summary>
      /// GetTable setting value
      /// </summary>
      /// <typeparam name="T">Type</typeparam>
      /// <param name="key">Key</param>
      /// <param name="value">Value</param>
      /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task SetSettingAsync<T>(string key, T value, bool clearCache = true);

      /// <summary>
      /// Gets all settings
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the settings
      /// </returns>
      Task<IList<Setting>> GetAllSettingsAsync();

      /// <summary>
      /// Determines whether a setting exists
      /// </summary>
      /// <typeparam name="T">Entity type</typeparam>
      /// <typeparam name="TPropType">Property type</typeparam>
      /// <param name="settings">Settings</param>
      /// <param name="keySelector">Key selector</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue -setting exists; false - does not exist
      /// </returns>
      Task<bool> SettingExistsAsync<T, TPropType>(T settings,
          Expression<Func<T, TPropType>> keySelector)
          where T : ISettings, new();

      /// <summary>
      /// Load settings
      /// </summary>
      /// <typeparam name="T">Type</typeparam>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task<T> LoadSettingAsync<T>() where T : ISettings, new();

      /// <summary>
      /// Load settings
      /// </summary>
      /// <param name="type">Type</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task<ISettings> LoadSettingAsync(Type type);

      /// <summary>
      /// Save settings object
      /// </summary>
      /// <typeparam name="T">Type</typeparam>
      /// <param name="settings">Setting instance</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task SaveSettingAsync<T>(T settings) where T : ISettings, new();

      /// <summary>
      /// Save settings object
      /// </summary>
      /// <typeparam name="T">Entity type</typeparam>
      /// <typeparam name="TPropType">Property type</typeparam>
      /// <param name="settings">Settings</param>
      /// <param name="keySelector">Key selector</param>
      /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task SaveSettingAsync<T, TPropType>(T settings,
          Expression<Func<T, TPropType>> keySelector, bool clearCache = true) where T : ISettings, new();

      /// <summary>
      /// </summary>
      /// <typeparam name="T">Type</typeparam>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteSettingAsync<T>() where T : ISettings, new();

      /// <summary>
      /// Delete settings object
      /// </summary>
      /// <typeparam name="T">Entity type</typeparam>
      /// <typeparam name="TPropType">Property type</typeparam>
      /// <param name="settings">Settings</param>
      /// <param name="keySelector">Key selector</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteSettingAsync<T, TPropType>(T settings,
          Expression<Func<T, TPropType>> keySelector) where T : ISettings, new();

      /// <summary>
      /// Clear cache
      /// </summary>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task ClearCacheAsync();

      /// <summary>
      /// Get setting key (stored into database)
      /// </summary>
      /// <typeparam name="TSettings">Type of settings</typeparam>
      /// <typeparam name="T">Property type</typeparam>
      /// <param name="settings">Settings</param>
      /// <param name="keySelector">Key selector</param>
      /// <returns>Key</returns>
      string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector)
          where TSettings : ISettings, new();
   }
}