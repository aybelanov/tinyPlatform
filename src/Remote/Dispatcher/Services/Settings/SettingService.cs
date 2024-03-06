using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Data;
using Devices.Dispatcher.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Devices.Dispatcher.Services.Settings;

/// <summary>
/// Setting manager
/// </summary>
public partial class SettingService : ISettingService
{
   #region Fields

   private readonly AppDbContext _dbContext;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public SettingService(AppDbContext dbContext)
   {
      _dbContext = dbContext;
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
   }

   /// <summary>
   /// Set setting value
   /// </summary>
   /// <param name="type">Type</param>
   /// <param name="key">Key</param>
   /// <param name="value">Value</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected virtual async Task SetSettingAsync(Type type, string key, object value)
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
         await UpdateSettingAsync(setting);
      }
      else
      {
         //insert
         var setting = new Setting
         {
            Name = key,
            Value = valueStr,
         };
         await InsertSettingAsync(setting);
      }
   }


   /// <summary>
   /// Converts a value to a destination type.
   /// </summary>
   /// <param name="value">The value to convert.</param>
   /// <param name="destinationType">The type to convert the value to.</param>
   /// <returns>The converted value.</returns>
   private object To(object value, Type destinationType)
   {
      return To(value, destinationType, CultureInfo.InvariantCulture);
   }

   /// <summary>
   /// Converts a value to a destination type.
   /// </summary>
   /// <param name="value">The value to convert.</param>
   /// <param name="destinationType">The type to convert the value to.</param>
   /// <param name="culture">Culture</param>
   /// <returns>The converted value.</returns>
   private object To(object value, Type destinationType, CultureInfo culture)
   {
      if (value == null)
         return null;

      var sourceType = value.GetType();

      var destinationConverter = TypeDescriptor.GetConverter(destinationType);
      if (destinationConverter.CanConvertFrom(value.GetType()))
         return destinationConverter.ConvertFrom(null, culture, value);

      var sourceConverter = TypeDescriptor.GetConverter(sourceType);
      if (sourceConverter.CanConvertTo(destinationType))
         return sourceConverter.ConvertTo(null, culture, value, destinationType);

      if (destinationType.IsEnum && value is int)
         return Enum.ToObject(destinationType, (int)value);

      if (!destinationType.IsInstanceOfType(value))
         return Convert.ChangeType(value, destinationType, culture);

      return value;
   }


   /// <summary>
   /// Converts a value to a destination type.
   /// </summary>
   /// <param name="value">The value to convert.</param>
   /// <typeparam name="T">The type to convert the value to.</typeparam>
   /// <returns>The converted value.</returns>
   private T To<T>(object value)
   {
      //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
      return (T)To(value, typeof(T));
   }

   #endregion

   #region Methods

   /// <summary>
   /// Adds a setting
   /// </summary>
   /// <param name="setting">Setting</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertSettingAsync(Setting setting)
   {
      await _dbContext.Settings.AddAsync(setting);
      await _dbContext.SaveChangesAsync();
   }

   /// <summary>
   /// Updates a setting
   /// </summary>
   /// <param name="setting">Setting</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateSettingAsync(Setting setting)
   {
      if (setting == null)
         throw new ArgumentNullException(nameof(setting));

      _dbContext.Settings.Update(setting);
      await _dbContext.SaveChangesAsync();
   }

   /// <summary>
   /// Deletes a setting
   /// </summary>
   /// <param name="setting">Setting</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteSettingAsync(Setting setting)
   {
      _dbContext.Settings.Remove(setting);
      await _dbContext.SaveChangesAsync();

   }

   /// <summary>
   /// Deletes settings
   /// </summary>
   /// <param name="settings">Settings</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteSettingsAsync(IList<Setting> settings)
   {
      _dbContext.Settings.RemoveRange(settings);
      await _dbContext.SaveChangesAsync();
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
      return await _dbContext.Settings.FirstOrDefaultAsync(x => x.Id == settingId);
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

      return setting != null ? To<T>(setting.Value) : defaultValue;
   }

   /// <summary>
   /// Set setting value
   /// </summary>
   /// <typeparam name="T">Type</typeparam>
   /// <param name="key">Key</param>
   /// <param name="value">Value</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SetSettingAsync<T>(string key, T value)
   {
      await SetSettingAsync(typeof(T), key, value);
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
      var settings = await _dbContext.Settings.OrderBy(x => x.Name).ToListAsync();
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

      foreach (var prop in type.GetProperties())
      {
         // get properties we can read and write to
         if (!prop.CanRead || !prop.CanWrite)
            continue;

         var key = type.Name + "." + prop.Name;
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
            await SetSettingAsync(prop.PropertyType, key, value);
         else
            await SetSettingAsync(key, string.Empty);
      }
   }

   /// <summary>
   /// Save settings object
   /// </summary>
   /// <typeparam name="T">Entity type</typeparam>
   /// <typeparam name="TPropType">Property type</typeparam>
   /// <param name="settings">Settings</param>
   /// <param name="keySelector">Key selector</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SaveSettingAsync<T, TPropType>(T settings,
       Expression<Func<T, TPropType>> keySelector) where T : ISettings, new()
   {
      if (keySelector.Body is not MemberExpression member)
         throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

      var propInfo = member.Member as PropertyInfo;
      if (propInfo == null)
         throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

      var key = GetSettingKey(settings, keySelector);
      var value = (TPropType)propInfo.GetValue(settings, null);
      if (value != null)
         await SetSettingAsync(key, value);
      else
         await SetSettingAsync(key, string.Empty);
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


   /// <summary>
   /// Get device scope configuration (the device, the watchers, the sensors)
   /// </summary>
   /// <returns></returns>
   public virtual async Task<long> GetConfigurationVersion()
   {
      var currentConfigurationModifiedTicks = (await LoadSettingAsync<DeviceSettings>()).ModifiedTicks;
      return currentConfigurationModifiedTicks;
   }

   #endregion
}