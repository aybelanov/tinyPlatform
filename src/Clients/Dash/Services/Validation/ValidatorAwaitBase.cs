using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Radzen;
using System;
using System.Threading.Tasks;

namespace Clients.Dash.Services.Validation;

/// <summary>
/// Base class of Radzen validator components with async/await support.
/// </summary>
public abstract class ValidatorAwaitBase : RadzenComponent, IRadzenFormValidator
{
   /// <summary>
   /// Stores the validation messages.
   /// </summary>
   protected ValidationMessageStore messages;

   /// <summary>
   /// Gets or sets the form which contains this validator.
   /// </summary>
   [CascadingParameter]
   public IRadzenForm Form { get; set; }

   /// <summary>
   /// Specifies the component which this validator should validate. 
   /// Must be set to the Radzen.IRadzenFormComponent.Name of an existing component.
   /// </summary>
   [Parameter]
   public string Component { get; set; }

   /// <summary>
   /// Specifies the message displayed when the validator is invalid.
   /// </summary>
   [Parameter]
#pragma warning disable BL0007 // Component parameters should be auto properties
   public abstract string Text { get; set; }
#pragma warning restore BL0007 // Component parameters should be auto properties

   /// <summary>
   /// Determines if the validator is displayed as a popup or not. Set to false by default.
   /// </summary>
   [Parameter]
   public bool Popup { get; set; }

   /// <summary>
   /// Is component validating
   /// </summary>
   [Parameter]
   public bool IsValidating { get; set; }

   /// <summary>
   /// IsValidate event callback
   /// </summary>
   [Parameter]
   public EventCallback<bool> IsValidatingChanged { get; set; }

   /// <summary>Returns the validity status.</summary>
   /// <value>true if this validator is valid; otherwise, false.</value>
   public bool IsValid { get; protected set; } = true;

   /// <summary>
   ///  Provided by the Radzen.Blazor.RadzenTemplateForm`1 which contains this validator. Used internally.
   /// </summary>
   /// <value>
   /// The edit context.
   /// </value>     
   [CascadingParameter]
   public EditContext EditContext { get; set; }

   private FieldIdentifier FieldIdentifier { get; set; }
   private EventHandler<FieldChangedEventArgs> FieldChangedHandler;
   private EventHandler<ValidationRequestedEventArgs> ValidationModelHandler;
   private EventHandler<ValidationStateChangedEventArgs> ValidationStateHandler;
   private TaskCompletionSource FieldValidatingTask;

   /// <summary>
   /// Default ctor
   /// </summary>
   public ValidatorAwaitBase() : base()
   {
      FieldChangedHandler = async (o, e) => await ValidateFieldAsync(o, e);
      ValidationModelHandler = async (o, e) => await ValidationRequestedAsync(o, e);
      ValidationStateHandler = async (o, e) => await ValidationStateChangedAsync(o, e);
   }

   /// <inheritdoc/>
   public override async Task SetParametersAsync(ParameterView parameters)
   {
      await base.SetParametersAsync(parameters);
      if (Visible)
      {
         if (EditContext != null && messages == null)
         {
            messages = new ValidationMessageStore(EditContext);
            Unsubscribe();
            EditContext.OnFieldChanged += FieldChangedHandler;
            EditContext.OnValidationRequested += ValidationModelHandler;
            EditContext.OnValidationStateChanged += ValidationStateHandler;
         }
      }
      else
      {
         RemoveFromEditContext();
      }
   }

   private void Unsubscribe()
   {
      EditContext.OnFieldChanged -= FieldChangedHandler;
      EditContext.OnValidationRequested -= ValidationModelHandler;
      EditContext.OnValidationStateChanged -= ValidationStateHandler;
   }

   private void RemoveFromEditContext()
   {
      if (EditContext != null && messages != null)
      {
         Unsubscribe();
         if (FieldIdentifier.FieldName != null)
         {
            ValidationMessageStore validationMessageStore = messages;
            FieldIdentifier fieldIdentifier = FieldIdentifier;
            validationMessageStore.Clear(in fieldIdentifier);
         }
      }

      messages = null;
      IsValid = true;
   }

   private async Task ValidateFieldAsync(object sender, FieldChangedEventArgs args)
   {
      IRadzenFormComponent radzenFormComponent = Form.FindComponent(Component)
         ?? throw new InvalidOperationException("Cannot find component with Name " + Component);

      if (radzenFormComponent != null && args.FieldIdentifier.Equals(radzenFormComponent.FieldIdentifier))
      {
         await IsValidatingChanged.InvokeAsync(true);

         if (FieldValidatingTask is null || FieldValidatingTask.Task.IsCompleted)
            FieldValidatingTask = new();

         IsValid = await ValidateFieldAsync(radzenFormComponent);
         FieldValidatingTask.SetResult();

         await IsValidatingChanged.InvokeAsync(false);

         await ValidateModelAsync();
      }
   }

   private async Task ValidationRequestedAsync(object sender, ValidationRequestedEventArgs args)
   {
      try
      {
         await ValidateModelAsync();
      }
      catch (Exception ex)
      {
         FieldValidatingTask?.SetResult();
         throw new Exception("validation failed", ex);
      }
   }

   private async Task ValidateModelAsync()
   {
      IRadzenFormComponent radzenFormComponent = Form.FindComponent(Component)
         ?? throw new InvalidOperationException("Cannot find component with Name " + Component);

      if (radzenFormComponent.FieldIdentifier.FieldName != null)
      {
         // button "submit" had been clicked before fields have been read
         if (FieldValidatingTask is null)
         {
            FieldIdentifier fieldId = radzenFormComponent.FieldIdentifier;
            await ValidateFieldAsync(EditContext, new FieldChangedEventArgs(in fieldId));
         }
         else
         {
            await FieldValidatingTask.Task;
         }

         ValidationMessageStore validationMessageStore = messages;
         FieldIdentifier fieldIdentifier = radzenFormComponent.FieldIdentifier;
         validationMessageStore.Clear(in fieldIdentifier);

         if (!IsValid)
         {
            ValidationMessageStore validationMessageStore2 = messages;
            fieldIdentifier = radzenFormComponent.FieldIdentifier;
            validationMessageStore2.Add(in fieldIdentifier, Text);
         }

         EditContext?.NotifyValidationStateChanged();
      }

      FieldIdentifier = radzenFormComponent.FieldIdentifier;
      await Task.CompletedTask;
   }

   /// <inheritdoc/>
   protected override string GetComponentCssClass()
   {
      return "rz-message rz-messages-error " + (Popup ? "rz-message-popup" : "");
   }

   /// <summary>
   /// Runs validation against the specified component's field.
   /// </summary>
   /// <param name="component">The component to validate.</param>
   /// <returns>true if validation is successful, false otherwise.</returns>
   protected abstract Task<bool> ValidateFieldAsync(IRadzenFormComponent component);


   private async Task ValidationStateChangedAsync(object sender, ValidationStateChangedEventArgs e)
   {
      StateHasChanged();
      await Task.CompletedTask;
   }

   /// <inheritdoc/>
   public override void Dispose()
   {
      base.Dispose();
      RemoveFromEditContext();
   }

   /// <inheritdoc/>
   protected override void BuildRenderTree(RenderTreeBuilder builder)
   {
      if (Visible && !IsValid)
      {
         builder.OpenElement(0, "div");
         builder.AddAttribute(1, "style", Style);
         builder.AddAttribute(2, "class", GetCssClass());
         builder.AddMultipleAttributes(3, base.Attributes);
         builder.AddContent(4, Text);
         builder.CloseElement();
      }
   }
}