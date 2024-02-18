using FluentValidation;
using Hub.Core.Domain.Users;
using Hub.Services.Localization;

namespace Hub.Web.Framework.Validators;

/// <summary>
/// Validator extensions
/// </summary>
public static class ValidatorExtensions
{
   /// <summary>
   /// GetTable credit card validator
   /// </summary>
   /// <typeparam name="TModel">Type of model being validated</typeparam>
   /// <param name="ruleBuilder">Rule builder</param>
   /// <returns>Result</returns>
   public static IRuleBuilderOptions<TModel, string> IsCreditCard<TModel>(this IRuleBuilder<TModel, string> ruleBuilder)
   {
      return ruleBuilder.SetValidator(new CreditCardPropertyValidator<TModel, string>());
   }

   /// <summary>
   /// GetTable decimal validator
   /// </summary>
   /// <typeparam name="TModel">Type of model being validated</typeparam>
   /// <param name="ruleBuilder">Rule builder</param>
   /// <param name="maxValue">Maximum value</param>
   /// <returns>Result</returns>
   public static IRuleBuilderOptions<TModel, decimal> IsDecimal<TModel>(this IRuleBuilder<TModel, decimal> ruleBuilder, decimal maxValue)
   {
      return ruleBuilder.SetValidator(new DecimalPropertyValidator<TModel, decimal>(maxValue));
   }

   /// <summary>
   /// GetTable username validator
   /// </summary>
   /// <typeparam name="TModel">Type of model being validated</typeparam>
   /// <param name="ruleBuilder">Rule builder</param>
   /// <param name="userSettings">User settings</param>
   /// <returns>Result</returns>
   public static IRuleBuilderOptions<TModel, string> IsUsername<TModel>(this IRuleBuilder<TModel, string> ruleBuilder,
       UserSettings userSettings)
   {
      return ruleBuilder.SetValidator(new UsernamePropertyValidator<TModel, string>(userSettings));
   }

   /// <summary>
   /// GetTable phone number validator
   /// </summary>
   /// <typeparam name="TModel">Type of model being validated</typeparam>
   /// <param name="ruleBuilder">Rule builder</param>
   /// <param name="userSettings">User settings</param>
   /// <returns>Result</returns>
   public static IRuleBuilderOptions<TModel, string> IsPhoneNumber<TModel>(this IRuleBuilder<TModel, string> ruleBuilder,
       UserSettings userSettings)
   {
      return ruleBuilder.SetValidator(new PhoneNumberPropertyValidator<TModel, string>(userSettings));
   }

   /// <summary>
   /// Implement password validator
   /// </summary>
   /// <typeparam name="TModel">Type of model being validated</typeparam>
   /// <param name="ruleBuilder">Rule builder</param>
   /// <param name="localizationService">Localization service</param>
   /// <param name="userSettings">User settings</param>
   /// <returns>Result</returns>
   public static IRuleBuilder<TModel, string> IsPassword<TModel>(this IRuleBuilder<TModel, string> ruleBuilder,
       ILocalizationService localizationService, UserSettings userSettings)
   {
      var regExp = "^";
      //Passwords must be at least X characters and contain the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*-)
      regExp += userSettings.PasswordRequireUppercase ? "(?=.*?[A-Z])" : "";
      regExp += userSettings.PasswordRequireLowercase ? "(?=.*?[a-z])" : "";
      regExp += userSettings.PasswordRequireDigit ? "(?=.*?[0-9])" : "";
      regExp += userSettings.PasswordRequireNonAlphanumeric ? "(?=.*?[#?!@$%^&*-])" : "";
      regExp += $".{{{userSettings.PasswordMinLength},}}$";

      var options = ruleBuilder
          .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Validation.Password.IsNotEmpty"))
          .Matches(regExp).WithMessageAwait(async () => string.Format(await localizationService.GetResourceAsync("Validation.Password.Rule"),
          string.Format(await localizationService.GetResourceAsync("Validation.Password.LengthValidation"), userSettings.PasswordMinLength),
          userSettings.PasswordRequireUppercase ? await localizationService.GetResourceAsync("Validation.Password.RequireUppercase") : "",
          userSettings.PasswordRequireLowercase ? await localizationService.GetResourceAsync("Validation.Password.RequireLowercase") : "",
          userSettings.PasswordRequireDigit ? await localizationService.GetResourceAsync("Validation.Password.RequireDigit") : "",
          userSettings.PasswordRequireNonAlphanumeric ? await localizationService.GetResourceAsync("Validation.Password.RequireNonAlphanumeric") : ""));

      return options;
   }
}