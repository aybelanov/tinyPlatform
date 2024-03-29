﻿using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System;
using System.Threading.Tasks;

namespace Clients.Dash.Services.Validation;

/// <summary>
/// Username validator
/// </summary>
public class UserNameValidator : ValidatorAwaitBase
{
   [Inject] ICommonService CommonService { get; set; }
   [Inject] NotificationService NotificationService { get; set; }
   [Inject] Localizer Localizer { get; set; }
   [Inject] AuthenticationStateProvider AuthProvider { get; set; }

   /// <summary>
   /// Specifies the message displayed when the validator is invalid.
   /// </summary>
   [Parameter]
   public override string Text { get; set; } = "User does not exist.";

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

         var state = await AuthProvider.GetAuthenticationStateAsync();
         if (state.User?.Identity.IsAuthenticated != true)
            throw new Exception(Localizer["Validation.NotAuthenticated"]);

         value = value.Trim();

         if (state.User?.Identity.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase) == true)
            throw new Exception(Localizer["Validation.TheSameUser"]);

         var result = await CommonService.CheckUserNameAvailabilityAsync(value);

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
