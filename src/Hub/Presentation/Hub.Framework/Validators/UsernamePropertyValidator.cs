using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Hub.Core.Domain.Users;

namespace Hub.Web.Framework.Validators
{
   /// <summary>
   /// Username validator
   /// </summary>
   public class UsernamePropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
   {
      private readonly UserSettings _userSettings;

      /// <summary> validtaor name </summary>
      public override string Name => "UsernamePropertyValidator";

      /// <summary> IoC Ctor </summary>
      public UsernamePropertyValidator(UserSettings userSettings)
      {
         _userSettings = userSettings;
      }

      /// <summary>
      /// Is valid?
      /// </summary>
      /// <param name="context">Validation context</param>
      /// <param name="value">Checking value</param>
      /// <returns>Result</returns>
      public override bool IsValid(ValidationContext<T> context, TProperty value)
      {
         return IsValid(value as string, _userSettings);
      }

      /// <summary>
      /// Is valid?
      /// </summary>
      /// <param name="username">Username</param>
      /// <param name="userSettings">User settings</param>
      /// <returns>Result</returns>
      public static bool IsValid(string username, UserSettings userSettings)
      {
         if (userSettings.UsernameMinLenght > 0 && username.Length < userSettings.UsernameMinLenght)
            return false;

         if (!userSettings.UsernameValidationEnabled || string.IsNullOrEmpty(userSettings.UsernameValidationRule))
            return true;

         if (string.IsNullOrEmpty(username))
            return false;

         return userSettings.UsernameValidationUseRegex
             ? Regex.IsMatch(username, userSettings.UsernameValidationRule, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
             : username.All(l => userSettings.UsernameValidationRule.Contains(l));
      }

      /// <summary>
      /// Default message for invalid values
      /// </summary>
      /// <param name="errorCode">Error code</param>
      /// <returns></returns>
      protected override string GetDefaultMessageTemplate(string errorCode) => "Username is not valid";
   }
}
