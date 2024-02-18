using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.Users;

namespace Hub.Services.ExportImport;

/// <summary>
/// Export manager interface
/// </summary>
public partial interface IExportManager
{
   /// <summary>
   /// Export user list to XLSX
   /// </summary>
   /// <param name="users">Users</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task<byte[]> ExportUsersToXlsxAsync(IList<User> users);

   /// <summary>
   /// Export device list to XLSX
   /// </summary>
   /// <param name="devices">Devices</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task<byte[]> ExportDevicesToXlsxAsync(IList<Device> devices);

   /// <summary>
   /// Export user list to XML
   /// </summary>
   /// <param name="users">Users</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result in XML format
   /// </returns>
   Task<string> ExportUsersToXmlAsync(IList<User> users);

   /// <summary>
   /// Export device list to XML
   /// </summary>
   /// <param name="devices">Devices</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result in XML format
   /// </returns>
   Task<string> ExportDevicesToXmlAsync(IList<Device> devices);

   /// <summary>
   /// Export newsletter subscribers to TXT
   /// </summary>
   /// <param name="subscriptions">Subscriptions</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result in TXT (string) format
   /// </returns>
   Task<string> ExportNewsletterSubscribersToTxtAsync(IList<NewsLetterSubscription> subscriptions);

   /// <summary>
   /// Export states to TXT
   /// </summary>
   /// <param name="states">States</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result in TXT (string) format
   /// </returns>
   Task<string> ExportStatesToTxtAsync(IList<StateProvince> states);

   /// <summary>
   /// Export user info (GDPR request) to XLSX 
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user GDPR info
   /// </returns>
   Task<byte[]> ExportUserGdprInfoToXlsxAsync(User user);
}
