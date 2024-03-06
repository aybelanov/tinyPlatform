﻿using Hub.Core.Domain.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Messages
{
   /// <summary>
   /// Campaign service
   /// </summary>
   public partial interface ICampaignService
   {
      /// <summary>
      /// Inserts a campaign
      /// </summary>
      /// <param name="campaign">Campaign</param>        
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertCampaignAsync(Campaign campaign);

      /// <summary>
      /// Updates a campaign
      /// </summary>
      /// <param name="campaign">Campaign</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdateCampaignAsync(Campaign campaign);

      /// <summary>
      /// IsDeleted a queued email
      /// </summary>
      /// <param name="campaign">Campaign</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteCampaignAsync(Campaign campaign);

      /// <summary>
      /// Gets a campaign by identifier
      /// </summary>
      /// <param name="campaignId">Campaign identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the campaign
      /// </returns>
      Task<Campaign> GetCampaignByIdAsync(long campaignId);

      /// <summary>
      /// Gets all campaigns
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the campaigns
      /// </returns>
      Task<IList<Campaign>> GetAllCampaignsAsync();

      /// <summary>
      /// Sends a campaign to specified emails
      /// </summary>
      /// <param name="campaign">Campaign</param>
      /// <param name="emailAccount">Email account</param>
      /// <param name="subscriptions">Subscriptions</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the otal emails sent
      /// </returns>
      Task<int> SendCampaignAsync(Campaign campaign, EmailAccount emailAccount,
          IEnumerable<NewsLetterSubscription> subscriptions);

      /// <summary>
      /// Sends a campaign to specified email
      /// </summary>
      /// <param name="campaign">Campaign</param>
      /// <param name="emailAccount">Email account</param>
      /// <param name="email">Email</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task SendCampaignAsync(Campaign campaign, EmailAccount emailAccount, string email);
   }
}
