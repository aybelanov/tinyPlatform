using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Messages
{
   /// <summary>
   /// Represents an email account model
   /// </summary>
   public partial record EmailAccountModel : BaseAppEntityModel
   {
      #region Properties

      [DataType(DataType.EmailAddress)]
      [AppResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Email")]
      public string Email { get; set; }

      [AppResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.DisplayName")]
      public string DisplayName { get; set; }

      [AppResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Host")]
      public string Host { get; set; }

      [AppResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Port")]
      public int Port { get; set; }

      [AppResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Username")]
      public string Username { get; set; }

      [AppResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Password")]
      [DataType(DataType.Password)]
      public string Password { get; set; }

      [AppResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.EnableSsl")]
      public bool EnableSsl { get; set; }

      [AppResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.UseDefaultCredentials")]
      public bool UseDefaultCredentials { get; set; }

      [AppResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.IsDefaultEmailAccount")]
      public bool IsDefaultEmailAccount { get; set; }

      [AppResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.SendTestEmailTo")]
      public string SendTestEmailTo { get; set; }

      #endregion
   }
}