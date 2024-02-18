using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core.Domain.Users;
using Hub.Web.Models.User;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the interface of the user model factory
/// </summary>
public partial interface IUserModelFactory
{
   /// <summary>
   /// Prepare the user info model
   /// </summary>
   /// <param name="model">User info model</param>
   /// <param name="user">User</param>
   /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
   /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user info model
   /// </returns>
   Task<UserInfoModel> PrepareUserInfoModelAsync(UserInfoModel model, User user, bool excludeProperties, string overrideCustomUserAttributesXml = "");

   /// <summary>
   /// Prepare the user register model
   /// </summary>
   /// <param name="model">User register model</param>
   /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
   /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
   /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user register model
   /// </returns>
   Task<RegisterModel> PrepareRegisterModelAsync(RegisterModel model, bool excludeProperties, string overrideCustomUserAttributesXml = "", bool setDefaultValues = false);

   /// <summary>
   /// Prepare the login model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the login model
   /// </returns>
   Task<LoginModel> PrepareLoginModelAsync();

   /// <summary>
   /// Prepare the password recovery model
   /// </summary>
   /// <param name="model">Password recovery model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the password recovery model
   /// </returns>
   Task<PasswordRecoveryModel> PreparePasswordRecoveryModelAsync(PasswordRecoveryModel model);

   /// <summary>
   /// Prepare the register result model
   /// </summary>
   /// <param name="resultId">Value of UserRegistrationType enum</param>
   /// <param name="returnUrl">URL to redirect</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the register result model
   /// </returns>
   Task<RegisterResultModel> PrepareRegisterResultModelAsync(int resultId, string returnUrl);

   /// <summary>
   /// Prepare the user navigation model
   /// </summary>
   /// <param name="selectedTabId">Identifier of the selected tab</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user navigation model
   /// </returns>
   Task<UserNavigationModel> PrepareUserNavigationModelAsync(int selectedTabId = 0);

   /// <summary>
   /// Prepare the user address list model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user address list model  
   /// </returns>
   Task<UserAddressListModel> PrepareUserAddressListModelAsync();

   /// <summary>
   /// Prepare the change password model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the change password model
   /// </returns>
   Task<ChangePasswordModel> PrepareChangePasswordModelAsync();

   /// <summary>
   /// Prepare the user avatar model
   /// </summary>
   /// <param name="model">User avatar model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user avatar model
   /// </returns>
   Task<UserAvatarModel> PrepareUserAvatarModelAsync(UserAvatarModel model);

   /// <summary>
   /// Prepare the GDPR tools model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the gDPR tools model
   /// </returns>
   Task<GdprToolsModel> PrepareGdprToolsModelAsync();

   /// <summary>
   /// Prepare the multi-factor authentication model
   /// </summary>
   /// <param name="model">Multi-factor authentication model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the multi-factor authentication model
   /// </returns>
   Task<MultiFactorAuthenticationModel> PrepareMultiFactorAuthenticationModelAsync(MultiFactorAuthenticationModel model);

   /// <summary>
   /// Prepare the multi-factor provider model
   /// </summary>
   /// <param name="providerModel">Multi-factor provider model</param>
   /// <param name="sysName">Multi-factor provider system name</param>
   /// <param name="isLogin">Is login page</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the multi-factor authentication model
   /// </returns>
   Task<MultiFactorAuthenticationProviderModel> PrepareMultiFactorAuthenticationProviderModelAsync(MultiFactorAuthenticationProviderModel providerModel, string sysName, bool isLogin = false);

   /// <summary>
   /// Prepare the custom user attribute models
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="overrideAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of the user attribute model
   /// </returns>
   Task<IList<UserAttributeModel>> PrepareCustomUserAttributesAsync(User user, string overrideAttributesXml = "");
}
