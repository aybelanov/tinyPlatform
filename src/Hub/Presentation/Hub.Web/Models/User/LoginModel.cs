using System.ComponentModel.DataAnnotations;
using Hub.Core.Domain.Users;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.User;

public partial record LoginModel : BaseAppModel
{
   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Account.Login.Fields.Email")]
   public string Email { get; set; }

   public bool UsernamesEnabled { get; set; }

   public UserRegistrationType RegistrationType { get; set; }

   [AppResourceDisplayName("Account.Login.Fields.Username")]
   public string Username { get; set; }

   [DataType(DataType.Password)]
   [AppResourceDisplayName("Account.Login.Fields.Password")]
   public string Password { get; set; }

   [AppResourceDisplayName("Account.Login.Fields.RememberMe")]
   public bool RememberMe { get; set; }

   public bool DisplayCaptcha { get; set; }
}