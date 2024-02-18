using Clients.Dash.Infrastructure;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.Dash.Services.Configuration;

/// <summary>
/// Represents a setting service
/// </summary>
public class SettingsService
{
   #region fields

   private readonly IJSRuntime _js;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="js"></param>
   public SettingsService(IJSRuntime js)
   {
      _js = js;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Gets a setting by key
   /// </summary>
   /// <typeparam name="T">Instance type</typeparam>
   /// <param name="key">Setting key</param>
   /// <returns></returns>
   public async Task<T> GetSettingByKeyAsync<T>(string key)
   {
      try
      {
         var value = await _js.InvokeAsync<string>("appjs.getSettings", key);
         var result = CommonHelper.To<T>(value);
         return result;
      }
      catch { }

      return default(T);
   }

   /// <summary>
   /// Gets a setting by key
   /// </summary>
   /// <typeparam name="T">Instance type</typeparam>
   /// <param name="key">Setting key</param>
   /// <returns></returns>
   public T GetSettingByKey<T>(string key)
   {
      try
      {
         var value = ((IJSInProcessRuntime)_js).Invoke<string>("appjs.getSettings", key);
         var result = CommonHelper.To<T>(value);
         return result;   
      }
      catch { }

      return default;
   }

   /// <summary>
   /// Sets or updates the setting
   /// </summary>
   /// <param name="key">Setting key</param>
   /// <param name="value">Setting value</param>
   /// <returns></returns>
   public async Task SetOrUpdateSettingAsync(string key, object value)
   {
      await _js.InvokeVoidAsync("appjs.saveSettings", key, value);
   }

   /// <summary>
   /// Sets or updates the setting
   /// </summary>
   /// <param name="key">Setting key</param>
   /// <param name="value">Setting value</param>
   /// <returns></returns>
   public void SetOrUpdateSetting(string key, object value)
   {
      ((IJSInProcessRuntime)_js).InvokeVoid("appjs.saveSettings", key, value);
   }

   /// <summary>
   /// Removes a setting value by the key
   /// </summary>
   /// <param name="key">Setting key</param>
   public async Task RemoveSettingAsync(string key)
   {
      await _js.InvokeVoidAsync("appjs.removeSettings", key);
   }

   /// <summary>
   /// Removes a setting value by the key
   /// </summary>
   /// <param name="key">Setting key</param>
   public void RemoveSetting(string key)
   {
      ((IJSInProcessRuntime)_js).InvokeVoid("appjs.removeSettings", key);
   }

   #endregion
}
