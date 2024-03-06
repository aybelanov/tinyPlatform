using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Clients.Dash.Services.Validation;

/// <summary>
/// Represents dropdown multuselect validator
/// </summary>
public class DropDownMultiselectValidator : ValidatorBase
{
   /// <inheritdoc/>
   public override string Text { get; set; } = $"The limit of selected items has been exceeded";

   /// <summary>
   /// Maximum selected items
   /// </summary>
   [Parameter]
   public int MaxItems { get; set; } = int.MaxValue;

   /// <summary>
   /// Minimum selected items
   /// </summary>
   [Parameter]
   public int MinItems { get; set; }

   /// <inheritdoc/>
   protected override bool Validate(IRadzenFormComponent component)
   {

      if (component is not null && component.GetType().GetGenericTypeDefinition() == typeof(RadzenDropDown<>))
      {
         BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
         FieldInfo field = component.GetType().GetField("selectedItems", bindFlags);
         var value = field.GetValue(component);
         var count = ((IEnumerable)value).Cast<object>().Count();
         if (count <= MaxItems && count >= MinItems)
            return true;
      }

      return false;
   }
}
