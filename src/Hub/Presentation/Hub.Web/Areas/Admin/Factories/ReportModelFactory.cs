using Hub.Core;
using Hub.Services.Directory;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Users;
using Hub.Web.Areas.Admin.Models.Reports;
using Hub.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories
{
   /// <summary>
   /// Represents the report model factory implementation
   /// </summary>
   public partial class ReportModelFactory : IReportModelFactory
   {
      #region Fields

      private readonly IBaseAdminModelFactory _baseAdminModelFactory;
      private readonly ICountryService _countryService;
      private readonly IUserReportService _userReportService;
      private readonly IUserService _userService;
      private readonly IDateTimeHelper _dateTimeHelper;
      private readonly ILocalizationService _localizationService;
      private readonly IWorkContext _workContext;

      #endregion

      #region Ctor

      public ReportModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
          ICountryService countryService,
          IUserReportService userReportService,
          IUserService userService,
          IDateTimeHelper dateTimeHelper,
          ILocalizationService localizationService,
          IWorkContext workContext)
      {
         _baseAdminModelFactory = baseAdminModelFactory;
         _countryService = countryService;
         _userReportService = userReportService;
         _userService = userService;
         _dateTimeHelper = dateTimeHelper;
         _localizationService = localizationService;
         _workContext = workContext;
      }

      #endregion

      #region Methods

      #region User reports

      /// <summary>
      /// Prepare user reports search model
      /// </summary>
      /// <param name="searchModel">User reports search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user reports search model
      /// </returns>
      public virtual async Task<UserReportsSearchModel> PrepareUserReportsSearchModelAsync(UserReportsSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //prepare nested search models
         await PrepareRegisteredUsersReportSearchModelAsync(searchModel.RegisteredUsers);

         return searchModel;
      }

      /// <summary>
      /// Prepare registered users report search model
      /// </summary>
      /// <param name="searchModel">Registered users report search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the registered users report search model
      /// </returns>
      protected virtual Task<RegisteredUsersReportSearchModel> PrepareRegisteredUsersReportSearchModelAsync(RegisteredUsersReportSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //prepare page parameters
         searchModel.SetGridPageSize();

         return Task.FromResult(searchModel);
      }

      /// <summary>
      /// Prepare paged registered users report list model
      /// </summary>
      /// <param name="searchModel">Registered users report search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the registered users report list model
      /// </returns>
      public virtual async Task<RegisteredUsersReportListModel> PrepareRegisteredUsersReportListModelAsync(RegisteredUsersReportSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //get report items
         var reportItems = new List<RegisteredUsersReportModel>
            {
                new RegisteredUsersReportModel
                {
                    Period = await _localizationService.GetResourceAsync("Admin.Reports.Users.RegisteredUsers.Fields.Period.7days"),
                    Users = await _userReportService.GetRegisteredUsersReportAsync(7)
                },
                new RegisteredUsersReportModel
                {
                    Period = await _localizationService.GetResourceAsync("Admin.Reports.Users.RegisteredUsers.Fields.Period.14days"),
                    Users = await _userReportService.GetRegisteredUsersReportAsync(14)
                },
                new RegisteredUsersReportModel
                {
                    Period = await _localizationService.GetResourceAsync("Admin.Reports.Users.RegisteredUsers.Fields.Period.month"),
                    Users = await _userReportService.GetRegisteredUsersReportAsync(30)
                },
                new RegisteredUsersReportModel
                {
                    Period = await _localizationService.GetResourceAsync("Admin.Reports.Users.RegisteredUsers.Fields.Period.year"),
                    Users = await _userReportService.GetRegisteredUsersReportAsync(365)
                }
            };

         var pagedList = reportItems.ToPagedList(searchModel);

         //prepare list model
         var model = new RegisteredUsersReportListModel().PrepareToGrid(searchModel, pagedList, () => pagedList);

         return model;
      }

      #endregion

      #endregion
   }
}