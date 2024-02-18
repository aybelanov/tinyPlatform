namespace Hub.Web.Framework.Validators;

/// <summary>
/// Represents default values related to validation
/// </summary>
public static partial class AppValidationDefaults
{
   /// <summary>
   /// Gets the name of a rule set used to validate model
   /// </summary>
   public static string ValidationRuleSet => "Validate";

   /// <summary>
   /// Gets the name of a locale used in not-null validation
   /// </summary>
   public static string NotNullValidationLocaleName => "Admin.Common.Validation.NotEmpty";
}