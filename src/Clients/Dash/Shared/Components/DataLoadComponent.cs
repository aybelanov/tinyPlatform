using Clients.Dash.Shared.Communication;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Clients.Dash.Shared.Components;

#pragma warning disable CS1591

public class DataLoadComponent : ComponentBase, IDisposable
{
   [Inject] DataLoadProcess DataLoadProcess { get; set; }

   protected bool isLoading;

   protected override void OnInitialized()
   {
      DataLoadProcess.LoadProcessStarting += DataLoadProcess_LoadProcessStarting;
      DataLoadProcess.LoadProcessEnded += DataLoadProcess_LoadProcessEnded;
      base.OnInitialized();
   }

   private Task DataLoadProcess_LoadProcessEnded()
   {
      isLoading = false;
      StateHasChanged();
      return Task.CompletedTask;
   }

   private void DataLoadProcess_LoadProcessStarting()
   {
      isLoading = true;
      StateHasChanged();
   }


   public virtual void Dispose()
   {
      DataLoadProcess.LoadProcessStarting -= DataLoadProcess_LoadProcessStarting;
      DataLoadProcess.LoadProcessEnded -= DataLoadProcess_LoadProcessEnded;
   }
}

#pragma warning restore CS1591
