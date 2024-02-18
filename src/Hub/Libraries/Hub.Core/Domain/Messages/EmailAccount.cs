﻿using System.Collections.Generic;
using Shared.Common;

namespace Hub.Core.Domain.Messages;

/// <summary>
/// Represents an email account
/// </summary>
public partial class EmailAccount : BaseEntity
{
   /// <summary>
   /// Gets or sets an email address
   /// </summary>
   public string Email { get; set; }

   /// <summary>
   /// Gets or sets an email display name
   /// </summary>
   public string DisplayName { get; set; }

   /// <summary>
   /// Gets or sets an email host
   /// </summary>
   public string Host { get; set; }

   /// <summary>
   /// Gets or sets an email port
   /// </summary>
   public int Port { get; set; }

   /// <summary>
   /// Gets or sets an email user name
   /// </summary>
   public string Username { get; set; }

   /// <summary>
   /// Gets or sets an email password
   /// </summary>
   public string Password { get; set; }

   /// <summary>
   /// Gets or sets a value that controls whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection
   /// </summary>
   public bool EnableSsl { get; set; }

   /// <summary>
   /// Gets or sets a value that controls whether the default system credentials of the application are sent with requests.
   /// </summary>
   public bool UseDefaultCredentials { get; set; }


//   #region Navigation
//#pragma warning disable CS1591

//   public List<QueuedEmail> QueuedEmails { get; set; } = new();
//   public List<MessageTemplate> MessageTemplates { get; set; } = new();

//#pragma warning restore CS1591
//   #endregion

}
