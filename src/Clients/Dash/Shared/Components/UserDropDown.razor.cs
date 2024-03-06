using Clients.Dash.Domain;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Security;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Shared.Components;

#pragma warning disable CS1591

public partial class UserDropDown : IDisposable
{
   #region Services

   [Inject] ICommonService CommonService { get; set; }
   [Inject] PermissionService PermissionService { get; set; }

   #endregion

   #region Parameters

   [Parameter]
   public long? UserId { get; set; }

   [Parameter]
   public EventCallback<long?> UserIdChanged { get; set; }

   [Parameter]
   public bool IsLoading { get; set; }

   [Parameter]
   public EventCallback<bool> IsLoadingChanged { get; set; }

   [Parameter]
   public EventCallback<UserSelectItem> Change { get; set; }

   [Parameter]
   public bool Disabled { get; set; }

   [Parameter]
   public string Style { get; set; }

   [Parameter]
   public string Name { get; set; }


   [Parameter]
   public bool AllowClear { get; set; }

   [Parameter]
   public string Placeholder { get; set; }


   /// <summary>
   /// Additional query
   /// </summary>
   [Parameter]
   public string Query { get; set; }

   [Parameter(CaptureUnmatchedValues = true)]
   public Dictionary<string, object> Attributes { get; set; }

   #endregion

   #region fields

   IFilterableList<UserSelectItem> _availableUsers = new FilterableList<UserSelectItem>() { new() };
   UserSelectItem _selectedUser;
   EventHandler<bool> _adminModeChangeHandler;
   RadzenDropDown<UserSelectItem> _selectedDropDown;
   DynamicFilter _filter;

   #endregion



   protected override void OnInitialized()
   {
      _adminModeChangeHandler = async (o, e) => await PermissionService_AdminModeChanged(o, e);
      PermissionService.AdminModeChanged += _adminModeChangeHandler;
      base.OnInitialized();
   }

   private async Task PermissionService_AdminModeChanged(object sender, bool e)
   {
      await UserChange(e ? _selectedUser : null);
   }

   #region Methods

   private async Task LoadUsers(LoadDataArgs args)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(args);
      filter.Query = Query;

      if (!filter.Equals(_filter))
      {
         _filter = filter;
         await IsLoadingChanged.InvokeAsync(true);
         await Task.Yield();
         try
         {
            if (!string.IsNullOrEmpty(filter.Filter))
               _filter.Filter = $"(Username == null ? \"\" : Username).ToLower().Contains(\"{args.Filter.ToLower()}\")";

            _availableUsers = await CommonService.GetUserSelecItemsAsync(filter);

            if (_selectedUser != null)
            {
               var item = _availableUsers.FirstOrDefault(x => x.Id == _selectedUser.Id);
               if (item != null)
               {
                  _availableUsers.Remove(item);
                  _availableUsers.Add(_selectedUser);
                  _availableUsers = new FilterableList<UserSelectItem>(_availableUsers.OrderBy(x => x.Id), _availableUsers.TotalCount);
               }
            }

            //var newItems = await CommonService.GetUserSelecItemsAsync(filter);

            //var items = _availableUsers.ToList();
            //items.AddRange(newItems);
            //items = items.Where(x => x.Id > 0).DistinctBy(x => x.Id).ToList();

            //if (_selectedUser != null)
            //{
            //   var item = items.FirstOrDefault(x => x.Id == _selectedUser.Id);
            //   if (item != null)
            //   {
            //      items.Remove(item);
            //      items.Add(_selectedUser);
            //   }
            //}

            //_availableUsers = new FilterableList<UserSelectItem>(items.OrderBy(x => x.Id), newItems.TotalCount);
         }
         catch (Exception ex)
         {
            await ErrorService.HandleError(this, ex);
         }
         finally
         {
            if (IsLoadingChanged.HasDelegate)
            {
               await IsLoadingChanged.InvokeAsync(false);
            }
            else
            {
               StateHasChanged();
            }
         }
      }
   }

   private async Task UserChange(object obj)
   {
      _selectedUser = obj as UserSelectItem;
      await UserIdChanged.InvokeAsync(_selectedUser?.Id);
      await Change.InvokeAsync(_selectedUser);
   }

   public void Dispose()
   {
      PermissionService.AdminModeChanged -= _adminModeChangeHandler;
   }

   #endregion
}
