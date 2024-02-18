using System.ComponentModel.DataAnnotations;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.User;

public partial record PasswordRecoveryModel : BaseAppModel
{
   [DataType(DataType.EmailAddress)]
   [AppResourceDisplayName("Account.PasswordRecovery.Email")]
   public string Email { get; set; }

   public bool DisplayCaptcha { get; set; }
}