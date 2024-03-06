﻿using FluentValidation;
using FluentValidation.Validators;
using System.Linq;

namespace Hub.Web.Framework.Validators;

/// <summary>
/// Credit card validator
/// </summary>
public class CreditCardPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
   /// <summary> Validator name </summary>
   public override string Name => "CreditCardPropertyValidator";

   /// <summary>
   /// Is valid?
   /// </summary>
   /// <param name="context">Validation context</param>
   /// <param name="value">Checking value</param>
   /// <returns>Result</returns>
   public override bool IsValid(ValidationContext<T> context, TProperty value)
   {
      var ccValue = value as string;
      if (string.IsNullOrWhiteSpace(ccValue))
         return false;

      ccValue = ccValue.Replace(" ", "");
      ccValue = ccValue.Replace("-", "");

      var checksum = 0;
      var evenDigit = false;

      //http://www.beachnet.com/~hstiles/cardtype.html
      foreach (var digit in ccValue.Reverse())
      {
         if (!char.IsDigit(digit))
            return false;

         var digitValue = (digit - '0') * (evenDigit ? 2 : 1);
         evenDigit = !evenDigit;

         while (digitValue > 0)
         {
            checksum += digitValue % 10;
            digitValue /= 10;
         }
      }

      return checksum % 10 == 0;
   }

   /// <summary>
   /// Default message for invalid values
   /// </summary>
   /// <param name="errorCode">Error code</param>
   /// <returns>Message</returns>
   protected override string GetDefaultMessageTemplate(string errorCode) => "Credit card number is not valid";
}
