using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Configuration;
using Hub.Core.Domain.Configuration;
using Hub.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Hub.Services.Configuration;

/// <summary>
/// Setting manager
/// </summary>
public partial class SettingService : ISettingService
{
   #region Fields

   private readonly IRepository<Setting> _settingRepository;
   private readonly IStaticCacheManager _staticCacheManager;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="settingRepository"></param>
   /// <param name="staticCacheManager"></param>
   public SettingService(IRepository<Setting> settingRepository,
       IStaticCacheManager staticCacheManager)
   {
      _settingRepository = settingRepository;
      _staticCacheManager = staticCacheManager;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Gets all settings
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the settings
   /// </returns>
   protected virtual async Task<IDictionary<string, IList<Setting>>> GetAllSettingsDictionaryAsync()
   {
      return await _staticCacheManager.GetAsync(AppSettingsDefaults.SettingsAllAsDictionaryCacheKey, async () =>
      {
         var settings = await GetAllSettingsAsync();

         var dictionary = new Dictionary<string, IList<Setting>>();
         foreach (var s in settings)
         {
            var resourceName = s.Name.ToLowerInvariant();
            var settingForCaching = new Setting
            {
               Id = s.Id,
               Name = s.Name,
               Value = s.Value,
            };
            if (!dictionary.ContainsKey(resourceName))
               //first setting
               dictionary.Add(resourceName, new List<Setting>
                  {
                         settingForCaching
                  });
            else
               //already added
               dictionary[resourceName].Add(settingForCaching);
         }

         return dictionary;
      });
   }

   /// <summary>
   /// GetTable setting value
   /// </summary>
   /// <param name="type">Type</param>
   /// <param name="key">Key</param>
   /// <param name="value">Value</param>
   /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task SetSettingAsync(Type type, string key, object value, bool clearCache = true)
   {
      if (key == null)
         throw new ArgumentNullException(nameof(key));
      key = key.Trim().ToLowerInvariant();
      var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

      var allSettings = await GetAllSettingsDictionaryAsync();
      var settingForCaching = allSettings.ContainsKey(key) ?
          allSettings[key].FirstOrDefault() : null;
      if (settingForCaching != null)
      {
         //update
         var setting = await GetSettingByIdAsync(settingForCaching.Id);
         setting.Value = valueStr;
         await UpdateSettingAsync(setting, clearCache);
      }
      else
      {
         //insert
         var setting = new Setting
         {
            Name = key,
            Value = valueStr,
         };
         await InsertSettingAsync(setting, clearCache);
      }
   }

   #endregion

   #region Methods

   /// <summary>
   /// Adds a setting
   /// </summary>
   /// <param name="setting">Setting</param>
   /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertSettingAsync(Setting setting, bool clearCache = true)
   {
      await _settingRepository.InsertAsync(setting);

      //cache
      if (clearCache)
         await ClearCacheAsync();
   }

   /// <summary>
   /// Updates a setting
   /// </summary>
   /// <param name="setting">Setting</param>
   /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateSettingAsync(Setting setting, bool clearCache = true)
   {
      if (setting == null)
         throw new ArgumentNullException(nameof(setting));

      await _settingRepository.UpdateAsync(setting);

      //cache
      if (clearCache)
         await ClearCacheAsync();
   }

   /// <summary>
   /// Deletes a setting
   /// </summary>
   /// <param name="setting">Setting</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteSettingAsync(Setting setting)
   {
      await _settingRepository.DeleteAsync(setting);

      //cache
      await ClearCacheAsync();
   }

   /// <summary>
   /// Deletes settings
   /// </summary>
   /// <param name="settings">Settings</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteSettingsAsync(IList<Setting> settings)
   {
      await _settingRepository.DeleteAsync(settings);

      //cache
      await ClearCacheAsync();
   }

   /// <summary>
   /// Gets a setting by identifier
   /// </summary>
   /// <param name="settingId">Setting identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the setting
   /// </returns>
   public virtual async Task<Setting> GetSettingByIdAsync(long settingId)
   {
      return await _settingRepository.GetByIdAsync(settingId, cache => default);
   }

   /// <summary>
   /// Get setting by key
   /// </summary>
   /// <param name="key">Key</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the setting
   /// </returns>
   public virtual async Task<Setting> GetSettingAsync(string key)
   {
      if (string.IsNullOrEmpty(key))
         return null;

      var settings = await GetAllSettingsDictionaryAsync();
      key = key.Trim().ToLowerInvariant();
      if (!settings.ContainsKey(key))
         return null;

      var settingsByKey = settings[key];
      var setting = settingsByKey.FirstOrDefault();

      return setting != null ? await GetSettingByIdAsync(setting.Id) : null;
   }

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
   public virtual async Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default)
   {
      if (string.IsNullOrEmpty(key))
         return defaultValue;

      var settings = await GetAllSettingsDictionaryAsync();
      key = key.Trim().ToLowerInvariant();
      if (!settings.ContainsKey(key))
         return defaultValue;

      var settingsByKey = settings[key];
      var setting = settingsByKey.FirstOrDefault();

      return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
   }

   /// <summary>
   /// GetTable setting value
   /// </summary>
   /// <typeparam name="T">Type</typeparam>
   /// <param name="key">Key</param>
   /// <param name="value">Value</param>
   /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SetSettingAsync<T>(string key, T value, bool clearCache = true)
   {
      await SetSettingAsync(typeof(T), key, value, clearCache);
   }

   /// <summary>
   /// Gets all settings
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the settings
   /// </returns>
   public virtual async Task<IList<Setting>> GetAllSettingsAsync()
   {
      var settings = await _settingRepository.GetAllAsync(query =>
      {
         return from s in query
                orderby s.Name
                select s;
      }, cache => default);

      return settings;
   }

   /// <summary>
   /// Determines whether a setting exists
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <typeparam name="TPropType">Property type</typeparam>
   /// <param name="settings">Entity</param>
   /// <param name="keySelector">Key selector</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue -setting exists; false - does not exist
   /// </returns>
   public virtual async Task<bool> SettingExistsAsync<T, TPropType>(T settings,
       Expression<Func<T, TPropType>> keySelector)
       where T : ISettings, new()
   {
      var key = GetSettingKey(settings, keySelector);

      var setting = await GetSettingByKeyAsync<string>(key);
      return setting != null;
   }

   /// <summary>
   /// Load settings
   /// </summary>
   /// <typeparam name="T">Type</typeparam>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task<T> LoadSettingAsync<T>() where T : ISettings, new()
   {
      return (T)await LoadSettingAsync(typeof(T));
   }

   /// <summary>
   /// Load settings
   /// </summary>
   /// <param name="type">Type</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task<ISettings> LoadSettingAsync(Type type)
   {
      var settings = Activator.CreateInstance(type);

      if (!DataSettingsManager.IsDatabaseInstalled())
         return settings as ISettings;

      foreach (var prop in type.GetProperties())
      {
         // get properties we can read and write to
         if (!prop.CanRead || !prop.CanWrite)
            continue;

         var key = type.Name + "." + prop.Name;
         //load by store
         var setting = await GetSettingByKeyAsync<string>(key);
         if (setting == null)
            continue;

         if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
            continue;

         if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
            continue;

         var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

         //set property
         prop.SetValue(settings, value, null);
      }

      return settings as ISettings;
   }

   /// <summary>
   /// Save settings object
   /// </summary>
   /// <typeparam name="T">Type</typeparam>
   /// <param name="settings">Setting instance</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SaveSettingAsync<T>(T settings) where T : ISettings, new()
   {
      /* We do not clear cache after each setting update.
       * This behavior can increase performance because cached settings will not be cleared 
       * and loaded from database after each update */
      foreach (var prop in typeof(T).GetProperties())
      {
         // get properties we can read and write to
         if (!prop.CanRead || !prop.CanWrite)
            continue;

         if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
            continue;

         var key = typeof(T).Name + "." + prop.Name;
         var value = prop.GetValue(settings, null);
         if (value != null)
            await SetSettingAsync(prop.PropertyType, key, value, false);
         else
            await SetSettingAsync(key, string.Empty, false);
      }

      //and now clear cache
      await ClearCacheAsync();
   }

   /// <summary>
   /// Save settings object
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <typeparam name="TPropType">Property type</typeparam>
   /// <param name="settings">Settings</param>
   /// <param name="keySelector">Key selector</param>
   /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SaveSettingAsync<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool clearCache = true)
      where T : ISettings, new()
   {
      if (keySelector.Body is not MemberExpression member)
         throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

      var propInfo = member.Member as PropertyInfo;
      if (propInfo == null)
         throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

      var key = GetSettingKey(settings, keySelector);
      var value = (TPropType)propInfo.GetValue(settings, null);
      if (value != null)
         await SetSettingAsync(key, value, clearCache);
      else
         await SetSettingAsync(key, string.Empty, clearCache);
   }

   /// <summary>
   /// Delete all settings
   /// </summary>
   /// <typeparam name="T">Type</typeparam>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteSettingAsync<T>() where T : ISettings, new()
   {
      var settingsToDelete = new List<Setting>();
      var allSettings = await GetAllSettingsAsync();
      foreach (var prop in typeof(T).GetProperties())
      {
         var key = typeof(T).Name + "." + prop.Name;
         settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
      }

      await DeleteSettingsAsync(settingsToDelete);
   }

   /// <summary>
   /// Delete settings object
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <typeparam name="TPropType">Property type</typeparam>
   /// <param name="settings">Settings</param>
   /// <param name="keySelector">Key selector</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteSettingAsync<T, TPropType>(T settings,
       Expression<Func<T, TPropType>> keySelector) where T : ISettings, new()
   {
      var key = GetSettingKey(settings, keySelector);
      key = key.Trim().ToLowerInvariant();

      var allSettings = await GetAllSettingsDictionaryAsync();
      var settingForCaching = allSettings.ContainsKey(key) ?
          allSettings[key].FirstOrDefault() : null;
      if (settingForCaching == null)
         return;

      //update
      var setting = await GetSettingByIdAsync(settingForCaching.Id);
      await DeleteSettingAsync(setting);
   }

   /// <summary>
   /// Clear cache
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task ClearCacheAsync()
   {
      await _staticCacheManager.RemoveByPrefixAsync(AppEntityCacheDefaults<Setting>.Prefix);
   }

   /// <summary>
   /// Get setting key (stored into database)
   /// </summary>
   /// <typeparam name="TSettings">Type of settings</typeparam>
   /// <typeparam name="T">Property type</typeparam>
   /// <param name="settings">Settings</param>
   /// <param name="keySelector">Key selector</param>
   /// <returns>Key</returns>
   public virtual string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector)
       where TSettings : ISettings, new()
   {
      if (keySelector.Body is not MemberExpression member)
         throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

      if (member.Member is not PropertyInfo propInfo)
         throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

      var key = $"{typeof(TSettings).Name}.{propInfo.Name}";

      return key;
   }
   #endregion
}