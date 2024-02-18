using System.Threading.Tasks;
using Hub.Web.Models.Profile;
using Hub.Core.Domain.Users;

namespace Hub.Web.Factories
{
   /// <summary>
   /// Represents the interface of the profile model factory
   /// </summary>
   public partial interface IProfileModelFactory
   {
      /// <summary>
      /// Prepare the profile index model
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="page">Number of posts page; pass null to disable paging</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the profile index model
      /// </returns>
      Task<ProfileIndexModel> PrepareProfileIndexModelAsync(User user, int? page);

      /// <summary>
      /// Prepare the profile info model
      /// </summary>
      /// <param name="user">User</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the profile info model
      /// </returns>
      Task<ProfileInfoModel> PrepareProfileInfoModelAsync(User user);

      /// <summary>
      /// Prepare the profile posts model
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="page">Number of posts page</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the profile posts model  
      /// </returns>
      Task<ProfilePostsModel> PrepareProfilePostsModelAsync(User user, int page);
   }
}
