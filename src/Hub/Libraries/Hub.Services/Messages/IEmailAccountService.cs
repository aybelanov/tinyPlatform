using Hub.Core.Domain.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Messages
{
   /// <summary>
   /// Email account service
   /// </summary>
   public partial interface IEmailAccountService
   {
      /// <summary>
      /// Inserts an email account
      /// </summary>
      /// <param name="emailAccount">Email account</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertEmailAccountAsync(EmailAccount emailAccount);

      /// <summary>
      /// Updates an email account
      /// </summary>
      /// <param name="emailAccount">Email account</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdateEmailAccountAsync(EmailAccount emailAccount);

      /// <summary>
      /// Deletes an email account
      /// </summary>
      /// <param name="emailAccount">Email account</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteEmailAccountAsync(EmailAccount emailAccount);

      /// <summary>
      /// Gets an email account by identifier
      /// </summary>
      /// <param name="emailAccountId">The email account identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the email account
      /// </returns>
      Task<EmailAccount> GetEmailAccountByIdAsync(long emailAccountId);

      /// <summary>
      /// Gets all email accounts
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the email accounts list
      /// </returns>
      Task<IList<EmailAccount>> GetAllEmailAccountsAsync();
   }
}
