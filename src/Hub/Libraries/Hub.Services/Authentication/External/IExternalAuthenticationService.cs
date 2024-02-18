﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Core.Domain.Users;

namespace Hub.Services.Authentication.External
{
   /// <summary>
   /// External authentication service
   /// </summary>
   public partial interface IExternalAuthenticationService
   {
      /// <summary>
      /// Authenticate user by passed parameters
      /// </summary>
      /// <param name="parameters">External authentication parameters</param>
      /// <param name="returnUrl">URL to which the user will return after authentication</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the result of an authentication
      /// </returns>
      Task<IActionResult> AuthenticateAsync(ExternalAuthenticationParameters parameters, string returnUrl = null);

      /// <summary>
      /// Get the external authentication records by identifier
      /// </summary>
      /// <param name="externalAuthenticationRecordId">External authentication record identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the result
      /// </returns>
      Task<ExternalAuthenticationRecord> GetExternalAuthenticationRecordByIdAsync(long externalAuthenticationRecordId);

      /// <summary>
      /// Get all the external authentication records by user
      /// </summary>
      /// <param name="user">User</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user
      /// </returns>
      Task<IList<ExternalAuthenticationRecord>> GetUserExternalAuthenticationRecordsAsync(User user);

      /// <summary>
      /// Delete the external authentication record
      /// </summary>
      /// <param name="externalAuthenticationRecord">External authentication record</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteExternalAuthenticationRecordAsync(ExternalAuthenticationRecord externalAuthenticationRecord);

      /// <summary>
      /// Associate external account with user
      /// </summary>
      /// <param name="user">User</param>
      /// <param name="parameters">External authentication parameters</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task AssociateExternalAccountWithUserAsync(User user, ExternalAuthenticationParameters parameters);

      /// <summary>
      /// Get the particular user with specified parameters
      /// </summary>
      /// <param name="parameters">External authentication parameters</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the user
      /// </returns>
      Task<User> GetUserByExternalAuthenticationParametersAsync(ExternalAuthenticationParameters parameters);

      /// <summary>
      /// Remove the association
      /// </summary>
      /// <param name="parameters">External authentication parameters</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task RemoveAssociationAsync(ExternalAuthenticationParameters parameters);
   }
}