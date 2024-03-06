using Radzen;
using Radzen.Blazor;
using System.Text.Json;

namespace Clients.Dash.Services.Validation;

/// <summary>
/// Represents a JSON format validator
/// </summary>
public class JsonFormatValidator : ValidatorBase
{
   /// <summary>
   /// Default message for non valid state
   /// </summary>
   public override string Text { get; set; } = "Json string required.";

   /// <summary>
   /// Validating the component field
   /// </summary>
   /// <param name="component">validating for compnent</param>
   /// <returns></returns>
   protected override bool Validate(IRadzenFormComponent component)
   {
      var value = component?.GetValue()?.ToString();

      if (string.IsNullOrWhiteSpace(value))
         return true;

      try
      {
         _ = JsonSerializer.Deserialize<object>(value);
         return true;
      }
      catch { }

      return false;
   }
}
