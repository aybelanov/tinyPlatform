using Radzen;
using Radzen.Blazor;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Clients.Dash.Services.Validation;

/// <summary>
/// Represents an entity selector validator
/// </summary>
public class EntitySelectorValidator : ValidatorBase
{
   /// <summary>
   /// Default message for non valid state
   /// </summary>
   public override string Text { get; set; } = "Require to select a device";

   /// <summary>
   /// Validating the component field
   /// </summary>
   /// <param name="component">validating for compnent</param>
   /// <returns></returns>
   protected override bool Validate(IRadzenFormComponent component)
   {
      try
      {
         return component.HasValue
             && component is RadzenDropDown<int> entitySelecting
             && int.TryParse(entitySelecting.Value.ToString(), out var selectedEntityId)
             && entitySelecting.Data.ToDynamicList().Any(x => (int)x.Id == selectedEntityId);
      }
      catch { }

      return false;
   }
}
