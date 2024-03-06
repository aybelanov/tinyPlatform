using FluentValidation;
using FluentValidation.Validators;
using System;

namespace Hub.Web.Framework.Validators;

/// <summary>
/// Decimal validator
/// </summary>
public class DecimalPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
   private readonly decimal _maxValue;

   /// <summary> Validator name </summary>
   public override string Name => "DecimalPropertyValidator";

   /// <summary> Param Ctor </summary>
   public DecimalPropertyValidator(decimal maxValue)
   {
      _maxValue = maxValue;
   }

   /// <summary>
   /// Is valid?
   /// </summary>
   /// <param name="context">Validation context</param>
   /// <param name="value">Checking value</param>
   /// <returns>Result</returns>
   public override bool IsValid(ValidationContext<T> context, TProperty value)
   {
      if (decimal.TryParse(value.ToString(), out var propertyValue))
         return Math.Round(propertyValue, 3) < _maxValue;

      return false;
   }

   /// <summary>
   /// Default message for invalid values
   /// </summary>
   /// <param name="errorCode">Error code</param>
   /// <returns>Message</returns>
   protected override string GetDefaultMessageTemplate(string errorCode) => "Decimal value is out of range";
}