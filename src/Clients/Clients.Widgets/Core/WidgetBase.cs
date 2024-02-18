using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.Widgets.Core;

/// <summary>
/// Represents a widget base abstract class
/// </summary>
public abstract class WidgetBase : ComponentBase, IWidgetBase, IDisposable, IAsyncDisposable
{
   /// <summary>
   /// Identifier
   /// </summary>
   [Parameter]
   public string Id { get; set; }

   /// <summary>
   /// Chart container class
   /// </summary>
   [Parameter]
   public string Class { get; set; }

   /// <summary>
   /// Chart container style
   /// </summary>
   [Parameter]
   public string Style { get; set; }

   /// <summary>
   /// Color theme
   /// </summary>
   [Parameter]
   public string Theme { get; set; }

   /// <summary>
   /// Current culture
   /// </summary>
   [Parameter]
   public string Culture { get; set; }

   /// <summary>
   /// Empty text for widget
   /// </summary>
   [Parameter]
   public string EmptyText { get; set; }

   /// <summary>
   /// Wrapper attributes dictionary 
   /// </summary>
   [Parameter(CaptureUnmatchedValues = true)]
   public Dictionary<string, object> Attributes { get; set; }

   /// <summary>
   /// Is a widget loading
   /// </summary>
   [Parameter]
   public bool IsLoading { get; set; }

   /// <summary>
   /// Loading stub for charts
   /// </summary>
   protected RenderFragment LoadingStub => b =>
   {
      if (IsLoading)
      {
         b.OpenElement(1, "div");
         b.AddAttribute(2, "class", "widget-loading");
         b.CloseElement();
         b.OpenElement(3, "div");
         b.AddAttribute(4, "class", "widget-loading-content");
         b.OpenElement(5, "img");
         b.AddAttribute(6, "src", "_content/Clients.Widgets/img/loading.svg");
         b.AddAttribute(7, "class", "loading-icons filter-white-theme");
         b.CloseElement();
         b.CloseElement();
      }
   };

   /// <summary>
   /// Represents a reference to a rendered element.
   /// </summary>
   public ElementReference Element { get; set; }

   /// <summary>
   /// Default ctor
   /// </summary>
   public WidgetBase()
   {
      Id = Guid.NewGuid().ToString("N");
      Culture = "en";
   }

   /// <inheritdoc/>
   public virtual void Dispose()
   {

   }

   /// <inheritdoc/>
   public virtual ValueTask DisposeAsync()
     => ValueTask.CompletedTask;
}
