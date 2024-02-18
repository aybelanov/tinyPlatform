using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Hub.Core.Domain.Users;

namespace Hub.Web.Framework.Validators
{
   /// <summary>
   /// Phohe number validator
   /// </summary>
   public class PhoneNumberPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
   {
      private readonly UserSettings _userSettings;

      /// <summary> Validator name </summary>
      public override string Name => "PhoneNumberPropertyValidator";

      /// <summary> IoC Ctor </summary>
      public PhoneNumberPropertyValidator(UserSettings userSettings)
      {
         _userSettings = userSettings;
      }

      /// <summary>
      /// Is valid?
      /// </summary>
      /// <param name="context">Validation context</param>
      /// <param name="value">validating value</param>
      /// <returns>Result</returns>
      public override bool IsValid(ValidationContext<T> context, TProperty value)
      {
         return IsValid(value as string, _userSettings);
      }

      /// <summary>
      /// Is valid?
      /// </summary>
      /// <param name="phoneNumber">Phone number</param>
      /// <param name="userSettings">User settings</param>
      /// <returns>Result</returns>
      public static bool IsValid(string phoneNumber, UserSettings userSettings)
      {
         if (!userSettings.PhoneNumberValidationEnabled || string.IsNullOrEmpty(userSettings.PhoneNumberValidationRule))
            return true;

         if (string.IsNullOrEmpty(phoneNumber))
            return false;

         return userSettings.PhoneNumberValidationUseRegex
             ? Regex.IsMatch(phoneNumber, userSettings.PhoneNumberValidationRule, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
             : phoneNumber.All(l => userSettings.PhoneNumberValidationRule.Contains(l));
      }

      /// <summary>
      /// Default message for invalid values
      /// </summary>
      /// <param name="errorCode">Error code</param>
      /// <returns></returns>
      protected override string GetDefaultMessageTemplate(string errorCode) => "Phone number is not valid";
   }
}
