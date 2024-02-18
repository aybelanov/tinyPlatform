using Clients.Dash.Models;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Clients.Dash.Shared.Components;

/// <summary>
/// Generic calss for CreateOrEditForm components
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class CreateOrEditForm<TItem> where TItem : BaseEntityModel
{
   /// <summary>
   /// Default ctor
   /// </summary>
   public CreateOrEditForm()
   {
      AfterSave = AfterDelete = async () => await JS.InvokeVoidAsync("history.back");
   }
}
