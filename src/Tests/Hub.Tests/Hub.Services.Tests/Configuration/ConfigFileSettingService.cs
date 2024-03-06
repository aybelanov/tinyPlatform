using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Configuration;
using Hub.Data;
using Hub.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Tests.Configuration
{
   public class ConfigFileSettingService(IRepository<Setting> settingRepository, IStaticCacheManager staticCacheManager) : SettingService(settingRepository, staticCacheManager)
   {
      public override Task<Setting> GetSettingByIdAsync(long settingId)
      {
         throw new InvalidOperationException("Get setting by id is not supported");
      }

      public override async Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default)
      {
         if (string.IsNullOrEmpty(key))
            return defaultValue;

         var settings = await GetAllSettingsAsync();
         key = key.Trim().ToLowerInvariant();

         var setting = settings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

         if (setting != null)
            return Shared.Common.Helpers.CommonHelper.To<T>(setting.Value);

         return defaultValue;
      }

      public override Task DeleteSettingAsync(Setting setting)
      {
         throw new InvalidOperationException("Deleting settings is not supported");
      }

      public override Task SetSettingAsync<T>(string key, T value, bool clearCache = true)
      {
         throw new NotImplementedException();
      }

      public override Task<IList<Setting>> GetAllSettingsAsync()
      {
         var settings = new List<Setting>();
         var appSettings = new Dictionary<string, string>
            {
                { "Setting1", "SomeValue"},
                { "Setting2", "25"},
                { "Setting3", "12/25/2010"},
                { "TestSettings.ServerName", "Ruby"},
                { "TestSettings.Ip", "192.168.0.1"},
                { "TestSettings.PortNumber", "21"},
                { "TestSettings.Username", "admin"},
                { "TestSettings.Password", "password"}
            };
         foreach (var setting in appSettings)
            settings.Add(new Setting
            {
               Name = setting.Key.ToLowerInvariant(),
               Value = setting.Value
            });

         return Task.FromResult<IList<Setting>>(settings);
      }

      public override Task ClearCacheAsync()
      {
         return Task.CompletedTask;
      }
   }
}
