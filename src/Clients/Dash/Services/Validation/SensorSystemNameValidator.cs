using Clients.Dash.Pages.Configuration.Sensors;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Localization;
using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Threading.Tasks;

namespace Clients.Dash.Services.Validation;

/// <summary>
/// Sensor system name validator
/// </summary>
public class SensorSystemNameValidator : ValidatorAwaitBase
{
   [Inject] ISensorService SensorService { get; set; }
   [Inject] NotificationService NotificationService { get; set; }
   [Inject] Localizer Localizer { get; set; }

   /// <summary>
   /// Specifies the message displayed when the validator is invalid.
   /// </summary>
   [Parameter]
   public override string Text { get; set; } = "System name is wrong.";

   /// <summary>
   /// Maximal length
   /// </summary>
   [Parameter]
   public int Max { get; set; } = 24;

   /// <summary>
   /// Minimal length
   /// </summary>
   [Parameter]
   public int Min { get; set; } = 3;

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

         if (value.Length > Max || value.Length < Min)
            throw new Exception(Localizer["Validation.RequireLength", Min, Max]);

         var model = (SensorModel)EditContext.Model ?? throw new ArgumentNullException();

         if (model.DeviceId < 1)
            throw new Exception($"Invalid device id {model.DeviceId}");

         if (model.Id > 0)
         {
            var sensor = await SensorService.GetSensorByIdAsync(model.Id);
            if (sensor.SystemName.Trim().Equals(value))
               return true;
         }

         var result = await SensorService.CheckSystemNameAvailabilityAsync(value, model.DeviceId);

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
