﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Messages;
using Hub.Data;

namespace Hub.Services.Messages;

/// <summary>
/// Email account service
/// </summary>
public partial class EmailAccountService : IEmailAccountService
{
   #region Fields

   private readonly IRepository<EmailAccount> _emailAccountRepository;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>

   public EmailAccountService(IRepository<EmailAccount> emailAccountRepository)
   {
      _emailAccountRepository = emailAccountRepository;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Inserts an email account
   /// </summary>
   /// <param name="emailAccount">Email account</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertEmailAccountAsync(EmailAccount emailAccount)
   {
      if (emailAccount == null)
         throw new ArgumentNullException(nameof(emailAccount));

      emailAccount.Email = CommonHelper.EnsureNotNull(emailAccount.Email);
      emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
      emailAccount.Host = CommonHelper.EnsureNotNull(emailAccount.Host);
      emailAccount.Username = CommonHelper.EnsureNotNull(emailAccount.Username);
      emailAccount.Password = CommonHelper.EnsureNotNull(emailAccount.Password);

      emailAccount.Email = emailAccount.Email.Trim();
      emailAccount.DisplayName = emailAccount.DisplayName.Trim();
      emailAccount.Host = emailAccount.Host.Trim();
      emailAccount.Username = emailAccount.Username.Trim();
      emailAccount.Password = emailAccount.Password.Trim();

      emailAccount.Email = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
      emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
      emailAccount.Host = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
      emailAccount.Username = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
      emailAccount.Password = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

      await _emailAccountRepository.InsertAsync(emailAccount);
   }

   /// <summary>
   /// Updates an email account
   /// </summary>
   /// <param name="emailAccount">Email account</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateEmailAccountAsync(EmailAccount emailAccount)
   {
      if (emailAccount == null)
         throw new ArgumentNullException(nameof(emailAccount));

      emailAccount.Email = CommonHelper.EnsureNotNull(emailAccount.Email);
      emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
      emailAccount.Host = CommonHelper.EnsureNotNull(emailAccount.Host);
      emailAccount.Username = CommonHelper.EnsureNotNull(emailAccount.Username);
      emailAccount.Password = CommonHelper.EnsureNotNull(emailAccount.Password);

      emailAccount.Email = emailAccount.Email.Trim();
      emailAccount.DisplayName = emailAccount.DisplayName.Trim();
      emailAccount.Host = emailAccount.Host.Trim();
      emailAccount.Username = emailAccount.Username.Trim();
      emailAccount.Password = emailAccount.Password.Trim();

      emailAccount.Email = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
      emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
      emailAccount.Host = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
      emailAccount.Username = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
      emailAccount.Password = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

      await _emailAccountRepository.UpdateAsync(emailAccount);
   }

   /// <summary>
   /// Deletes an email account
   /// </summary>
   /// <param name="emailAccount">Email account</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteEmailAccountAsync(EmailAccount emailAccount)
   {
      if (emailAccount == null)
         throw new ArgumentNullException(nameof(emailAccount));

      if ((await GetAllEmailAccountsAsync()).Count == 1)
         throw new AppException("You cannot delete this email account. At least one account is required.");

      await _emailAccountRepository.DeleteAsync(emailAccount);
   }

   /// <summary>
   /// Gets an email account by identifier
   /// </summary>
   /// <param name="emailAccountId">The email account identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the email account
   /// </returns>
   public virtual async Task<EmailAccount> GetEmailAccountByIdAsync(long emailAccountId)
   {
      return await _emailAccountRepository.GetByIdAsync(emailAccountId, cache => default);
   }

   /// <summary>
   /// Gets all email accounts
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the email accounts list
   /// </returns>
   public virtual async Task<IList<EmailAccount>> GetAllEmailAccountsAsync()
   {
      var emailAccounts = await _emailAccountRepository.GetAllAsync(query =>
      {
         return from ea in query
                orderby ea.Id
                select ea;
      }, cache => default);

      return emailAccounts;
   }

   #endregion
}