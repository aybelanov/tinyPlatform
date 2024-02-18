using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Web.Areas.OpenId.Domain;
using Hub.Web.Areas.OpenId.Models;

namespace Hub.Web.Areas.OpenId.Factories;

/// <summary>
/// Represents an openid interface 
/// </summary>
public interface IOpenIdModelFactory
{
   /// <summary>
   /// Prepare an access token and id token
   /// </summary>
   /// <param name="subjectId">Subject identifier</param>
   /// <returns></returns>
   Task<ClientTokenInfoModel> PrepareClientTokensAsync(AuthCode authCode);

   /// <summary>
   /// Prepares device token
   /// </summary>
   /// <param name="systemName">Device system name</param>
   /// <param name="secret">Device secret (password)</param>
   /// <returns></returns>
   Task<DeviceTokenInfoModel> PrepareDeviceTokenAsync(string systemName, string secret);

   /// <summary>
   /// Prepare a JSON Web Key Set
   /// </summary>
   /// <returns></returns>
   Task<JwksModel> PrepareJwksAsync();

   /// <summary>
   /// Prepare an openid cinfiguration
   /// </summary>
   /// <returns></returns>
   Task<OpenIdConfigModel> PrepareOpenIdConfigAsync();

   /// <summary>
   /// Prepares a checksession model
   /// </summary>
   /// <returns></returns>
   Task<CheckSessionModel> PrepareCheckSessionAsync();

   /// <summary>
   /// Prepares a userinfo model
   /// </summary>
   /// <returns>User info model</returns>
   Task<UserInfoModel> PrepareUserInfoModelAsync();
}
