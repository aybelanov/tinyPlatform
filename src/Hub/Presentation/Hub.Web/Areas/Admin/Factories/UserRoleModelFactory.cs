using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Data.Extensions;
using Hub.Services.Seo;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories
{
   /// <summary>
   /// Represents the user role model factory implementation
   /// </summary>
   public partial class UserRoleModelFactory : IUserRoleModelFactory
   {
      #region Fields

      private readonly IBaseAdminModelFactory _baseAdminModelFactory;
      private readonly IUserService _userService;
      private readonly IUrlRecordService _urlRecordService;
      private readonly IWorkContext _workContext;

      #endregion

      #region Ctor

      public UserRoleModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
          IUserService userService,
          IUrlRecordService urlRecordService,
          IWorkContext workContext)
      {
         _baseAdminModelFactory = baseAdminModelFactory;
         _userService = userService;
         _urlRecordService = urlRecordService;
         _workContext = workContext;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Prepare user role search model
      /// </summary>
      /// <param name="searchModel">User role search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user role search model
      /// </returns>
      public virtual Task<UserRoleSearchModel> PrepareUserRoleSearchModelAsync(UserRoleSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //prepare page parameters
         searchModel.SetGridPageSize();

         return Task.FromResult(searchModel);
      }

      /// <summary>
      /// Prepare paged user role list model
      /// </summary>
      /// <param name="searchModel">User role search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user role list model
      /// </returns>
      public virtual async Task<UserRoleListModel> PrepareUserRoleListModelAsync(UserRoleSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //get user roles
         var userRoles = (await _userService.GetAllUserRolesAsync(true)).ToPagedList(searchModel);

         //prepare grid model
         var model = await new UserRoleListModel().PrepareToGridAsync(searchModel, userRoles, () =>
         {
            return userRoles.SelectAwait(async role =>
               {
                  //fill in model values from the entity
                  var userRoleModel = role.ToModel<UserRoleModel>();

                  // TODO some logic

                  return await Task.FromResult(userRoleModel);
               });
         });

         return model;
      }

      /// <summary>
      /// Prepare user role model
      /// </summary>
      /// <param name="model">User role model</param>
      /// <param name="userRole">User role</param>
      /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user role model
      /// </returns>
      public virtual Task<UserRoleModel> PrepareUserRoleModelAsync(UserRoleModel model, UserRole userRole, bool excludeProperties = false)
      {
         if (userRole != null)
         {
            //fill in model values from the entity
            model ??= userRole.ToModel<UserRoleModel>();
         }

         //set default values for the new model
         if (userRole == null)
            model.Active = true;


         return Task.FromResult(model);
      }


      #endregion
   }
}