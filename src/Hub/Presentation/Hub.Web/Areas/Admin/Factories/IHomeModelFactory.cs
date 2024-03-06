using Hub.Web.Areas.Admin.Models.Home;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the home models factory
/// </summary>
public partial interface IHomeModelFactory
{
   /// <summary>
   /// Prepare dashboard model
   /// </summary>
   /// <param name="model">Dashboard model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the dashboard model
   /// </returns>
   Task<DashboardModel> PrepareDashboardModelAsync(DashboardModel model);

   /// <summary>
   /// Prepare tinyPlatform news model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the tinyPlatform news model
   /// </returns>
   Task<TinyPlatformNewsModel> PrepareTinyPlatformNewsModelAsync();
}