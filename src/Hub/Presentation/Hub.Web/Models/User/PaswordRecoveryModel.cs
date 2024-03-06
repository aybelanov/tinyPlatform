using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Models.User;

public partial record PasswordRecoveryModel : BaseAppModel
{
   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Account.PasswordRecovery.Email")]
   public string Email { get; set; }

   public bool DisplayCaptcha { get; set; }
}