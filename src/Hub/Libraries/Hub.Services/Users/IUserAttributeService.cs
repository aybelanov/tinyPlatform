using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core.Domain.Users;

namespace Hub.Services.Users
{
   /// <summary>
   /// User attribute service
   /// </summary>
   public partial interface IUserAttributeService
   {
      /// <summary>
      /// Deletes a user attribute
      /// </summary>
      /// <param name="userAttribute">User attribute</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteUserAttributeAsync(UserAttribute userAttribute);

      /// <summary>
      /// Gets all user attributes
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user attributes
      /// </returns>
      Task<IList<UserAttribute>> GetAllUserAttributesAsync();

      /// <summary>
      /// Gets a user attribute 
      /// </summary>
      /// <param name="userAttributeId">User attribute identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user attribute
      /// </returns>
      Task<UserAttribute> GetUserAttributeByIdAsync(long userAttributeId);

      /// <summary>
      /// Inserts a user attribute
      /// </summary>
      /// <param name="userAttribute">User attribute</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertUserAttributeAsync(UserAttribute userAttribute);

      /// <summary>
      /// Updates the user attribute
      /// </summary>
      /// <param name="userAttribute">User attribute</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdateUserAttributeAsync(UserAttribute userAttribute);

      /// <summary>
      /// Deletes a user attribute value
      /// </summary>
      /// <param name="userAttributeValue">User attribute value</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteUserAttributeValueAsync(UserAttributeValue userAttributeValue);

      /// <summary>
      /// Gets user attribute values by user attribute identifier
      /// </summary>
      /// <param name="userAttributeId">The user attribute identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user attribute values
      /// </returns>
      Task<IList<UserAttributeValue>> GetUserAttributeValuesAsync(long userAttributeId);

      /// <summary>
      /// Gets a user attribute value
      /// </summary>
      /// <param name="userAttributeValueId">User attribute value identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user attribute value
      /// </returns>
      Task<UserAttributeValue> GetUserAttributeValueByIdAsync(long userAttributeValueId);

      /// <summary>
      /// Inserts a user attribute value
      /// </summary>
      /// <param name="userAttributeValue">User attribute value</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertUserAttributeValueAsync(UserAttributeValue userAttributeValue);

      /// <summary>
      /// Updates the user attribute value
      /// </summary>
      /// <param name="userAttributeValue">User attribute value</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdateUserAttributeValueAsync(UserAttributeValue userAttributeValue);
   }
}
