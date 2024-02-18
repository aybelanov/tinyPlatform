using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Domain.Logging;
using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Core.Domain.Common;
using Hub.Data;
using Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace Hub.Services.Logging;

/// <summary>
/// Default application logger
/// </summary>
public partial class DefaultLogger : ILogger
{
   #region Fields

   private readonly CommonSettings _commonSettings;

   private readonly IRepository<Log> _logRepository;
   private readonly IWebHelper _webHelper;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DefaultLogger(CommonSettings commonSettings,
       IRepository<Log> logRepository,
       IWebHelper webHelper)
   {
      _commonSettings = commonSettings;
      _logRepository = logRepository;
      _webHelper = webHelper;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Gets a value indicating whether this message should not be logged
   /// </summary>
   /// <param name="message">Message</param>
   /// <returns>Result</returns>
   protected virtual bool IgnoreLog(string message)
   {
      if (!_commonSettings.IgnoreLogWordlist.Any())
         return false;

      if (string.IsNullOrWhiteSpace(message))
         return false;

      return _commonSettings
          .IgnoreLogWordlist
          .Any(x => message.Contains(x, StringComparison.InvariantCultureIgnoreCase));
   }

   #endregion

   #region Methods

   /// <summary>
   /// Determines whether a log level is enabled
   /// </summary>
   /// <param name="level">Log level</param>
   /// <returns>Result</returns>
   public virtual bool IsEnabled(LogLevel level)
   {
      return level switch
      {
         LogLevel.Debug => false,
         _ => true,
      };
   }

   /// <summary>
   /// Deletes a log item
   /// </summary>
   /// <param name="log">Log item</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteLogAsync(Log log)
   {
      if (log == null)
         throw new ArgumentNullException(nameof(log));

      await _logRepository.DeleteAsync(log, false);
   }

   /// <summary>
   /// Deletes a log items
   /// </summary>
   /// <param name="logs">Log items</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteLogsAsync(IList<Log> logs)
   {
      await _logRepository.DeleteAsync(logs, false);
   }

   /// <summary>
   /// Clears a log
   /// </summary>
   /// <param name="olderThan">The date that sets the restriction on deleting records. Leave null to remove all records</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task ClearLogAsync(DateTime? olderThan = null)
   {
      if (olderThan == null)
         await _logRepository.TruncateAsync();
      else
         await _logRepository.DeleteAsync(p => p.CreatedOnUtc < olderThan.Value);
   }

   /// <summary>
   /// Gets all log items
   /// </summary>
   /// <param name="fromUtc">Log item creation from; null to load all records</param>
   /// <param name="toUtc">Log item creation to; null to load all records</param>
   /// <param name="message">Message</param>
   /// <param name="logLevel">Log level; null to load all records</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the log item items
   /// </returns>
   public virtual async Task<IPagedList<Log>> GetAllLogsAsync(DateTime? fromUtc = null, DateTime? toUtc = null,
       string message = "", LogLevel? logLevel = null,
       int pageIndex = 0, int pageSize = int.MaxValue)
   {
      var logs = await _logRepository.GetAllPagedAsync(_ =>
      {
         var query = _logRepository.Table.AsNoTracking();

         if (fromUtc.HasValue)
            query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
         if (toUtc.HasValue)
            query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
         if (logLevel.HasValue)
         {
            var logLevelId = (int)logLevel.Value;
            query = query.Where(l => logLevelId == l.LogLevelId);
         }

         if (!string.IsNullOrEmpty(message))
            query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
         query = query.OrderByDescending(l => l.CreatedOnUtc);

         return query;
      }, pageIndex, pageSize);

      return logs;
   }

   /// <summary>
   /// Gets a log item
   /// </summary>
   /// <param name="logId">Log item identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the log item
   /// </returns>
   public virtual async Task<Log> GetLogByIdAsync(long logId)
   {
      return await _logRepository.GetByIdAsync(logId);
   }

   /// <summary>
   /// Get log items by identifiers
   /// </summary>
   /// <param name="logIds">Log item identifiers</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the log items
   /// </returns>
   public virtual async Task<IList<Log>> GetLogByIdsAsync(long[] logIds)
   {
      return await _logRepository.GetByIdsAsync(logIds);
   }

   /// <summary>
   /// Inserts a log item
   /// </summary>
   /// <param name="logLevel">Log level</param>
   /// <param name="shortMessage">The short message</param>
   /// <param name="fullMessage">The full message</param>
   /// <param name="subject">The subject (user, device etc.) to associate log record with</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains a log item
   /// </returns>
   public virtual async Task<Log> InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "", BaseEntity subject = null)
   {
      //check ignore word/phrase list?
      if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
         return null;

      var log = new Log
      {
         LogLevel = logLevel,
         ShortMessage = shortMessage,
         FullMessage = fullMessage,
         IpAddress = _webHelper.GetCurrentIpAddress(),
         EntityId = subject?.Id,
         EntityName = subject?.GetType().Name,
         PageUrl = _webHelper.GetThisPageUrl(true),
         ReferrerUrl = _webHelper.GetUrlReferrer(),
         CreatedOnUtc = DateTime.UtcNow
      };

      await _logRepository.InsertAsync(log, false);

      return log;
   }

   /// <summary>
   /// Information
   /// </summary>
   /// <param name="message">Message</param>
   /// <param name="exception">Exception</param>
   /// <param name="user">User</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InformationAsync(string message, Exception exception = null, User user = null)
   {
      //don't log thread abort exception
      if (exception is System.Threading.ThreadAbortException)
         return;

      if (IsEnabled(LogLevel.Information))
         await InsertLogAsync(LogLevel.Information, message, exception?.ToString() ?? string.Empty, user);
   }

   /// <summary>
   /// Warning
   /// </summary>
   /// <param name="message">Message</param>
   /// <param name="exception">Exception</param>
   /// <param name="user">User</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task WarningAsync(string message, Exception exception = null, User user = null)
   {
      //don't log thread abort exception
      if (exception is System.Threading.ThreadAbortException)
         return;

      if (IsEnabled(LogLevel.Warning))
         await InsertLogAsync(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, user);
   }

   /// <summary>
   /// Error
   /// </summary>
   /// <param name="message">Message</param>
   /// <param name="exception">Exception</param>
   /// <param name="user">User</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task ErrorAsync(string message, Exception exception = null, User user = null)
   {
      //don't log thread abort exception
      if (exception is System.Threading.ThreadAbortException)
         return;

      if (IsEnabled(LogLevel.Error))
         await InsertLogAsync(LogLevel.Error, message, exception?.ToString() ?? string.Empty, user);
   }

   #endregion
}