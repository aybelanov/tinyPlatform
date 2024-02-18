using System.Threading.Tasks;

namespace Hub.Services.Users
{
   /// <summary>
   /// User report service interface
   /// </summary>
   public partial interface IUserReportService
   {
      /// <summary>
      /// Gets a report of users registered in the last days
      /// </summary>
      /// <param name="days">Users registered in the last days</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the number of registered users
      /// </returns>
      Task<int> GetRegisteredUsersReportAsync(int days);
   }
}