using System.Threading.Tasks;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Users;

namespace Hub.Core;

/// <summary>
/// Represents work context
/// </summary>
public interface IWorkContext
{
   /// <summary>
   /// Gets the current user
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task<User> GetCurrentUserAsync();

   /// <summary>
   /// Gets the current user's signalr connection identifier
   /// that saved in the request headers
   /// </summary>
   /// <returns>SignalR connection identifier</returns>
   Task<string> GetCurrentConncetionIdAsync();

   /// <summary>
   /// Gets the current device
   /// </summary>
   /// <returns>Device entity</returns>
   Task<Device> GetCurrentDeviceAsync();

   /// <summary>
   /// Sets the current user
   /// </summary>
   /// <param name="user">Current user</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task SetCurrentUserAsync(User user = null);

   /// <summary>
   /// Sets the current device to the current context
   /// </summary>
   /// <returns></returns>
   Task SetCurrentDeviceAsync(Device device = null);

   /// <summary>
   /// Gets the original user (in case the current one is impersonated)
   /// </summary>
   User OriginalUserIfImpersonated { get; }

   /// <summary>
   /// Gets current user working language
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task<Language> GetWorkingLanguageAsync();

   /// <summary>
   /// Sets current user working language
   /// </summary>
   /// <param name="language">Language</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task SetWorkingLanguageAsync(Language language);

   /// <summary>
   /// Gets or sets current user working currency
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task<Currency> GetWorkingCurrencyAsync();

   /// <summary>
   /// Sets current user working currency
   /// </summary>
   /// <param name="currency">Currency</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task SetWorkingCurrencyAsync(Currency currency);
}
