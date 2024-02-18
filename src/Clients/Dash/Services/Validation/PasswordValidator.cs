using Clients.Dash.Pages.Configuration.Devices;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Localization;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System;
using System.Threading.Tasks;

namespace Clients.Dash.Services.Validation;

/// <summary>
/// Device password validator
/// </summary>
public class PasswordValidator : ValidatorAwaitBase
{
   [Inject] IDeviceService DeviceService { get; set; }
   [Inject] NotificationService NotificationService { get; set; }
   [Inject] Localizer Localizer { get; set; }

   /// <summary>
   /// Specifies the message displayed when the validator is invalid.
   /// </summary>
   [Parameter]
   public override string Text { get; set; } = "Password is wrong.";

   /// <summary>
   /// Runs validation against the specified component.
   /// </summary>
   /// <param name="component">The component to validate.</param>
   /// <returns>true if validation is successful, false otherwise.</returns>
   protected override async Task<bool> ValidateFieldAsync(IRadzenFormComponent component)
   {
      ArgumentNullException.ThrowIfNull(component);

      try
      {
         var value = component.GetValue()?.ToString();

         if (string.IsNullOrWhiteSpace(value))
            throw new Exception(Localizer["Validation.RequireValue"]);

         var model = (DeviceModel)EditContext.Model ?? throw new ArgumentNullException();
         
         var result = await DeviceService.CheckPasswordFormatAsync(value, model.Id);

         if (!string.IsNullOrEmpty(result.Error))
            throw new Exception(result.Error);

         return true;
      }
      catch (Exception ex)
      {
         if (ex.Message.Length > 100) NotificationService.Notify(NotificationSeverity.Error, ex.Message, duration: -1d);
         else Text = ex.Message;

         return false;
      }
   }
}
