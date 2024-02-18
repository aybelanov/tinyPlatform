using System;
using System.Threading.Tasks;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Helpers;
using Shared.Clients.Configuration;

namespace Hub.Services.Users
{
   /// <summary>
   /// User report service
   /// </summary>
   public partial class UserReportService : IUserReportService
   {
      #region Fields

      private readonly IUserService _userService;
      private readonly IDateTimeHelper _dateTimeHelper;
      private readonly IRepository<User> _userRepository;

      #endregion

      #region Ctor

      /// <summary> IoC Ctor </summary>
      public UserReportService(IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IRepository<User> userRepository)
      {
         _userService = userService;
         _dateTimeHelper = dateTimeHelper;
         _userRepository = userRepository;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Gets a report of users registered in the last days
      /// </summary>
      /// <param name="days">Users registered in the last days</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the number of registered users
      /// </returns>
      public virtual async Task<int> GetRegisteredUsersReportAsync(int days)
      {
         var date = (await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now)).AddDays(-days);

         var registeredUserRole = await _userService.GetUserRoleBySystemNameAsync(UserDefaults.RegisteredRoleName);
         if (registeredUserRole == null)
            return 0;

         return (await _userService.GetAllUsersAsync(
             date.ToUniversalTime(),
             userRoleIds: [registeredUserRole.Id])).Count;
      }

      #endregion
   }
}