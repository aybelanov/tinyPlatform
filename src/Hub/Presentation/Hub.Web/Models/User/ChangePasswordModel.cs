using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Models.User;

public partial record ChangePasswordModel : BaseAppModel
{
   [DataType(DataType.Password)]
   [AppResourceDisplayName("Account.ChangePassword.Fields.OldPassword")]
   public string OldPassword { get; set; }

   [DataType(DataType.Password)]
   [AppResourceDisplayName("Account.ChangePassword.Fields.NewPassword")]
   public string NewPassword { get; set; }

   [DataType(DataType.Password)]
   [AppResourceDisplayName("Account.ChangePassword.Fields.ConfirmNewPassword")]
   public string ConfirmNewPassword { get; set; }
}