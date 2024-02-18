using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using static Clients.Widgets.Core.OpenLayerBase;

namespace Clients.Widgets;

/// <summary>
/// Component partial class
/// </summary>
public partial class SvgScheme
{
   #region Parameters

   /// <summary>
   /// Initial svg image
   /// </summary>
   [Parameter] public string ImageUrl {  get; set; }
   [Parameter] public string Width { get; set; }
   [Parameter] public string Height { get; set; }
   [Parameter] public string Config { get; set; }
   #endregion

   #region fields

   private IJSObjectReference schemeJs;
   protected DotNetObjectReference<SvgScheme> _objRef;

   private static IJSObjectReference _jsModule;
   private static JSObject _jsHostModule;
   private TaskCompletionSource _jsImportReady = new();

   #endregion

   #region Methods

   protected override async Task OnAfterRenderAsync(bool firstRender)
   {
      await base.OnAfterRenderAsync(firstRender);

      if (firstRender)
      {
         _jsHostModule ??= await JSHost.ImportAsync("SvgScheme.razor.js", "../_content/Clients.Widgets/SvgScheme.razor.js");
         _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Clients.Widgets/SvgScheme.razor.js");
         _objRef = DotNetObjectReference.Create(this);
         await _jsModule.InvokeVoidAsync("initScheme", Id, _objRef, Theme, Culture, ImageUrl, Config);

         _jsImportReady.SetResult();
      }
   }


   public async Task Update(IEnumerable<DataRecord> records) 
   {
      if (records?.Any() != true) 
         return;

      var dataset = new List<object>()
      {
         records.Select(x=>x.EventTimestamp).ToArray(),
         records.Select(x=>x.Value).ToArray(),
         records.Select(x=>x.JsonValue ?? string.Empty).ToArray(),
         records.Select(x=>x.Metadata ?? string.Empty).ToArray(),

      }.ToArray();
      
      await _jsImportReady.Task;
      Update(Id, dataset);
   }

   #endregion

   #region JsInterop

   [JSImport("updateScheme", "SvgScheme.razor.js")]
   internal static partial void Update(
      [JSMarshalAs<JSType.String>] string id,
      [JSMarshalAs<JSType.Array<JSType.Any>>] object[] data);

   [JSInvokable]
   public Task SchemeCallback(object args) 
   {
      return Task.CompletedTask;
   }

   #endregion

   #region Disposing

   /// <summary>
   /// <see href="https://learn.microsoft.com/ru-ru/dotnet/standard/garbage-collection/implementing-disposeasync"/>
   /// </summary>
   public override void Dispose()
   {
      Dispose(disposing: true);
      base.Dispose();
      GC.SuppressFinalize(this);
   }

   public override async ValueTask DisposeAsync()
   {
      await DisposeAsyncCore().ConfigureAwait(false);
      await base.DisposeAsync().ConfigureAwait(false);

      Dispose(disposing: false);
      GC.SuppressFinalize(this);
   }

   protected virtual void Dispose(bool disposing)
   {
      if (disposing)
      {
         _objRef?.Dispose();
         _objRef = null;
      }
   }

   protected virtual async ValueTask DisposeAsyncCore()
   {
      if (_jsModule is not null)
         await _jsModule.InvokeVoidAsync("dispose", Id).ConfigureAwait(false);

      if (schemeJs != null)
      {
         await schemeJs.DisposeAsync().ConfigureAwait(false);
         schemeJs = null;
      }

      _objRef?.Dispose();
      _objRef = null;
   }

   #endregion

   #region Nested classes

   /// <summary>
   /// Representa a incomming data 
   /// </summary>
   public record DataRecord
   {
      /// <summary>
      /// Record identifier 
      /// </summary>
      public long Id { get; set; }  

      /// <summary>
      /// Data record event timestamp
      /// </summary>
      public double EventTimestamp { get;set; }

      /// <summary>
      /// Scalar value
      /// </summary>
      public double Value { get; set; }

      /// <summary>
      /// JSON data value
      /// </summary>
      public string JsonValue { get; set; }

      /// <summary>
      /// Byte array contatining data values
      /// </summary>
      public byte[] Bytes { get; set; }

      /// <summary>
      /// Description of the data values
      /// </summary>
      public string Metadata { get; set; }
   }

   #endregion
}
