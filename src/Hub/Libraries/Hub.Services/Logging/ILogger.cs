using Hub.Core;
using Hub.Core.Domain.Logging;
using Hub.Core.Domain.Users;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Logging
{
   /// <summary>
   /// Logger interface
   /// </summary>
   public partial interface ILogger
   {
      /// <summary>
      /// Determines whether a log level is enabled
      /// </summary>
      /// <param name="level">Log level</param>
      /// <returns>Result</returns>
      bool IsEnabled(LogLevel level);

      /// <summary>
      /// Deletes a log item
      /// </summary>
      /// <param name="log">Log item</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteLogAsync(Log log);

      /// <summary>
      /// Deletes a log items
      /// </summary>
      /// <param name="logs">Log items</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteLogsAsync(IList<Log> logs);

      /// <summary>
      /// Clears a log
      /// </summary>
      /// <param name="olderThan">The date that sets the restriction on deleting records. Leave null to remove all records</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task ClearLogAsync(DateTime? olderThan = null);

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
      Task<IPagedList<Log>> GetAllLogsAsync(DateTime? fromUtc = null, DateTime? toUtc = null,
          string message = "", LogLevel? logLevel = null,
          int pageIndex = 0, int pageSize = int.MaxValue);

      /// <summary>
      /// Gets a log item
      /// </summary>
      /// <param name="logId">Log item identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the log item
      /// </returns>
      Task<Log> GetLogByIdAsync(long logId);

      /// <summary>
      /// Get log items by identifiers
      /// </summary>
      /// <param name="logIds">Log item identifiers</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the log items
      /// </returns>
      Task<IList<Log>> GetLogByIdsAsync(long[] logIds);

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
      Task<Log> InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "", BaseEntity subject = null);

      /// <summary>
      /// Information
      /// </summary>
      /// <param name="message">Message</param>
      /// <param name="exception">Exception</param>
      /// <param name="user">User</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InformationAsync(string message, Exception exception = null, User user = null);

      /// <summary>
      /// Warning
      /// </summary>
      /// <param name="message">Message</param>
      /// <param name="exception">Exception</param>
      /// <param name="user">User</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task WarningAsync(string message, Exception exception = null, User user = null);

      /// <summary>
      /// Error
      /// </summary>
      /// <param name="message">Message</param>
      /// <param name="exception">Exception</param>
      /// <param name="user">User</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task ErrorAsync(string message, Exception exception = null, User user = null);
   }
}