using Clients.Dash.Models;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Helpers;
using Microsoft.AspNetCore.Components;
using Shared.Clients;
using System;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Pages.Configuration.Users;

/// <summary>
/// Component partial class
/// </summary>
public partial class UserTable
{
   #region fields

   [Inject] ICommonService CommonService { get; set; }
   [Inject] IHelperService HelperService { get; set; }

   #endregion

   #region Methods

   /// <summary>
   /// Prepares user models by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamoc filter</param>
   /// <returns>User model collection</returns>
   protected async Task<IFilterableList<UserModel>> PrepareUserModelsAsync(DynamicFilter filter)
   {
      var users = await CommonService.GetUsersAsync(filter);
      var models = Auto.Mapper.Map<FilterableList<UserModel>>(users);
      models.TotalCount = users.TotalCount;

      foreach (var model in models)
         model.OnlineStatusString = await HelperService.GetOnlineStatusLocaleAsync(model.OnlineStatus);

      return models;
   }

   #endregion

   /// <summary>
   /// Represents user model class
   /// </summary>
   public class UserModel : BaseEntityModel
   {
      /// <summary>
      /// Username (nickname or email according hub settings)
      /// </summary>
      public string Username { get; set; }

      /// <summary>
      /// User online status string
      /// </summary>
      public string OnlineStatusString { get; set; }

      /// <summary>
      /// User online status
      /// </summary>
      public OnlineStatus OnlineStatus { get; set; }

      /// <summary>
      /// Last activity datetime on utc
      /// </summary>
      public DateTime LastActivityUtc { get; set; }

      /// <summary>
      /// Is user active (admin approved or not banned)
      /// </summary>
      public bool IsActive { get; set; }

      /// <summary>
      /// User avatar url
      /// </summary>
      public string AvatarUrl { get; set; }
   }
}

#pragma warning restore CS1591