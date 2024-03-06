using FluentValidation.AspNetCore;
using System;

namespace Hub.Web.Framework.Validators
{
   /// <summary>
   /// Represents attribute that used to mark model for the forced validation. 
   /// Without this attribute, the model passed in the parameter will not be validated. It's used to prevent auto-validation of child models.
   /// </summary>
   [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
   public sealed class ValidateAttribute : CustomizeValidatorAttribute
   {
      /// <summary> Default Ctor </summary>
      public ValidateAttribute()
      {
         //specify rule set
         RuleSet = AppValidationDefaults.ValidationRuleSet;
      }
   }
}