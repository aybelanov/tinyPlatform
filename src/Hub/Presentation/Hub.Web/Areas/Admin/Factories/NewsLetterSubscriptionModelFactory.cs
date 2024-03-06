using Hub.Data.Extensions;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Messages;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Web.Framework.Models.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories
{
   /// <summary>
   /// Represents the newsletter subscription model factory implementation
   /// </summary>
   public partial class NewsletterSubscriptionModelFactory : INewsletterSubscriptionModelFactory
   {
      #region Fields

      private readonly IBaseAdminModelFactory _baseAdminModelFactory;
      private readonly IDateTimeHelper _dateTimeHelper;
      private readonly ILocalizationService _localizationService;
      private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;

      #endregion

      #region Ctor

      public NewsletterSubscriptionModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
          IDateTimeHelper dateTimeHelper,
          ILocalizationService localizationService,
          INewsLetterSubscriptionService newsLetterSubscriptionService)
      {
         _baseAdminModelFactory = baseAdminModelFactory;
         _dateTimeHelper = dateTimeHelper;
         _localizationService = localizationService;
         _newsLetterSubscriptionService = newsLetterSubscriptionService;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Prepare newsletter subscription search model
      /// </summary>
      /// <param name="searchModel">Newsletter subscription search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the newsletter subscription search model
      /// </returns>
      public virtual async Task<NewsletterSubscriptionSearchModel> PrepareNewsletterSubscriptionSearchModelAsync(NewsletterSubscriptionSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //prepare available user roles
         await _baseAdminModelFactory.PrepareUserRolesAsync(searchModel.AvailableUserRoles);

         //prepare "activated" filter (0 - all; 1 - activated only; 2 - deactivated only)
         searchModel.ActiveList.Add(new SelectListItem
         {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.Milticast.NewsLetterSubscriptions.List.SearchActive.All")
         });
         searchModel.ActiveList.Add(new SelectListItem
         {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.Milticast.NewsLetterSubscriptions.List.SearchActive.ActiveOnly")
         });
         searchModel.ActiveList.Add(new SelectListItem
         {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.Milticast.NewsLetterSubscriptions.List.SearchActive.NotActiveOnly")
         });

         //prepare page parameters
         searchModel.SetGridPageSize();

         return searchModel;
      }

      /// <summary>
      /// Prepare paged newsletter subscription list model
      /// </summary>
      /// <param name="searchModel">Newsletter subscription search model</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the newsletter subscription list model
      /// </returns>
      public virtual async Task<NewsletterSubscriptionListModel> PrepareNewsletterSubscriptionListModelAsync(NewsletterSubscriptionSearchModel searchModel)
      {
         if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

         //get parameters to filter newsletter subscriptions
         var isActivatedOnly = searchModel.ActiveId == 0 ? null : searchModel.ActiveId == 1 ? true : (bool?)false;
         var startDateValue = !searchModel.StartDate.HasValue ? null
             : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
         var endDateValue = !searchModel.EndDate.HasValue ? null
             : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

         //get newsletter subscriptions
         var newsletterSubscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(email: searchModel.SearchEmail,
             userRoleId: searchModel.UserRoleId,
             isActive: isActivatedOnly,
             createdFromUtc: startDateValue,
             createdToUtc: endDateValue,
             pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

         //prepare list model
         var model = await new NewsletterSubscriptionListModel().PrepareToGridAsync(searchModel, newsletterSubscriptions, () =>
         {
            return newsletterSubscriptions.SelectAwait(async subscription =>
               {
                  //fill in model values from the entity
                  var subscriptionModel = subscription.ToModel<NewsletterSubscriptionModel>();

                  //convert dates to the user time
                  subscriptionModel.CreatedOn = (await _dateTimeHelper.ConvertToUserTimeAsync(subscription.CreatedOnUtc, DateTimeKind.Utc)).ToString();

                  return subscriptionModel;
               });
         });

         return model;
      }

      #endregion
   }
}