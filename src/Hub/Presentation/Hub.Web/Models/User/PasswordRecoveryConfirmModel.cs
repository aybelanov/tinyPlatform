﻿using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Models.User;

public partial record PasswordRecoveryConfirmModel : BaseAppModel
{
   [DataType(DataType.Password)]
   [AppResourceDisplayName("Account.PasswordRecovery.NewPassword")]
   public string NewPassword { get; set; }

   [DataType(DataType.Password)]
   [AppResourceDisplayName("Account.PasswordRecovery.ConfirmNewPassword")]
   public string ConfirmNewPassword { get; set; }

   public bool DisablePasswordChanging { get; set; }
   public string Result { get; set; }

   public string ReturnUrl { get; set; }
}