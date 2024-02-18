using Radzen;
using Radzen.Blazor;
using System;

namespace Clients.Dash.Services.Validation;


/// <summary>
/// Represents a GUID validator
/// </summary>
public class GuidValidator : ValidatorBase
{
   /// <summary>
   /// Default message for non valid state
   /// </summary>
   public override string Text { get; set; } = "Guid required.";

   /// <summary>
   /// Validating the component field
   /// </summary>
   /// <param name="component">validating for compnent</param>
   /// <returns></returns>
   protected override bool Validate(IRadzenFormComponent component)
       => component.HasValue && Guid.TryParse(component.GetValue().ToString(), out var guid) && guid != default;
}
