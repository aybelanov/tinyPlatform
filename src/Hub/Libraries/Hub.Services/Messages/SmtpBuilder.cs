﻿using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Messages;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Hub.Services.Messages
{
   /// <summary>
   /// SMTP Builder
   /// </summary>
   public class SmtpBuilder : ISmtpBuilder
   {
      #region Fields

      private readonly EmailAccountSettings _emailAccountSettings;
      private readonly IEmailAccountService _emailAccountService;

      #endregion

      #region Ctor

      /// <summary>
      /// IoC Ctor
      /// </summary>

      public SmtpBuilder(EmailAccountSettings emailAccountSettings, IEmailAccountService emailAccountService)
      {
         _emailAccountSettings = emailAccountSettings;
         _emailAccountService = emailAccountService;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Create a new SMTP client for a specific email account
      /// </summary>
      /// <param name="emailAccount">Email account to use. If null, then would be used EmailAccount by default</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the an SMTP client that can be used to send email messages
      /// </returns>
      public virtual async Task<SmtpClient> BuildAsync(EmailAccount emailAccount = null)
      {
         if (emailAccount is null)
         {
            emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            ?? throw new AppException("Email account could not be loaded");
         }

         var client = new SmtpClient
         {
            ServerCertificateValidationCallback = ValidateServerCertificate
         };

         try
         {
            await client.ConnectAsync(
                emailAccount.Host,
                emailAccount.Port,
                emailAccount.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable);

            if (emailAccount.UseDefaultCredentials)
            {
               await client.AuthenticateAsync(CredentialCache.DefaultNetworkCredentials);
            }
            else if (!string.IsNullOrWhiteSpace(emailAccount.Username))
            {
               await client.AuthenticateAsync(new NetworkCredential(emailAccount.Username, emailAccount.Password));
            }

            return client;
         }
         catch (Exception ex)
         {
            client.Dispose();
            throw new AppException(ex.Message, ex);
         }
      }

      /// <summary>
      /// Validates the remote Secure Sockets Layer (SSL) certificate used for authentication.
      /// </summary>
      /// <param name="sender">An object that contains state information for this validation.</param>
      /// <param name="certificate">The certificate used to authenticate the remote party.</param>
      /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
      /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
      /// <returns>A System.Boolean value that determines whether the specified certificate is accepted for authentication</returns>
      public virtual bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
      {
         //By default, server certificate verification is disabled.
         return true;
      }

      #endregion
   }
}