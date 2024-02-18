using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Web.Models.PrivateMessages;
using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Core.Domain.Forums;
using Hub.Services.Users;
using Hub.Services.Forums;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Web.Models.Common;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the private message model factory
/// </summary>
public partial class PrivateMessagesModelFactory : IPrivateMessagesModelFactory
{
   #region Fields

   private readonly UserSettings _userSettings;
   private readonly ForumSettings _forumSettings;
   private readonly IUserService _userService;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly IForumService _forumService;
   private readonly ILocalizationService _localizationService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public PrivateMessagesModelFactory(UserSettings userSettings,
       ForumSettings forumSettings,
       IUserService userService,
       IDateTimeHelper dateTimeHelper,
       IForumService forumService,
       ILocalizationService localizationService,
       IWorkContext workContext)
   {
      _userSettings = userSettings;
      _forumSettings = forumSettings;
      _userService = userService;
      _dateTimeHelper = dateTimeHelper;
      _forumService = forumService;
      _localizationService = localizationService;
      
      _workContext = workContext;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare the private message index model
   /// </summary>
   /// <param name="page">Number of items page; pass null to disable paging</param>
   /// <param name="tab">Tab name</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the private message index model
   /// </returns>
   public virtual Task<PrivateMessageIndexModel> PreparePrivateMessageIndexModelAsync(int? page, string tab)
   {
      var inboxPage = 0;
      var sentItemsPage = 0;
      var sentItemsTabSelected = false;

      switch (tab)
      {
         case "inbox":
            if (page.HasValue)
               inboxPage = page.Value;

            break;
         case "sent":
            if (page.HasValue)
               sentItemsPage = page.Value;

            sentItemsTabSelected = true;

            break;
         default:
            break;
      }

      var model = new PrivateMessageIndexModel
      {
         InboxPage = inboxPage,
         SentItemsPage = sentItemsPage,
         SentItemsTabSelected = sentItemsTabSelected
      };

      return Task.FromResult(model);
   }

   /// <summary>
   /// Prepare the inbox model
   /// </summary>
   /// <param name="page">Number of items page</param>
   /// <param name="tab">Tab name</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the private message list model
   /// </returns>
   public virtual async Task<PrivateMessageListModel> PrepareInboxModelAsync(int page, string tab)
   {
      if (page > 0)
         page -= 1;

      var pageSize = _forumSettings.PrivateMessagesPageSize;

      var messages = new List<PrivateMessageModel>();
      var user = await _workContext.GetCurrentUserAsync();
      var list = await _forumService.GetAllPrivateMessagesAsync(0, user.Id, null, null, false, string.Empty, page, pageSize);

      foreach (var pm in list)
         messages.Add(await PreparePrivateMessageModelAsync(pm));

      var pagerModel = new PagerModel(_localizationService)
      {
         PageSize = list.PageSize,
         TotalRecords = list.TotalCount,
         PageIndex = list.PageIndex,
         ShowTotalSummary = false,
         RouteActionName = "PrivateMessagesPaged",
         UseRouteLinks = true,
         RouteValues = new PrivateMessageRouteValues { pageNumber = page, tab = tab }
      };

      var model = new PrivateMessageListModel
      {
         Messages = messages,
         PagerModel = pagerModel
      };

      return model;
   }

   /// <summary>
   /// Prepare the sent model
   /// </summary>
   /// <param name="page">Number of items page</param>
   /// <param name="tab">Tab name</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the private message list model
   /// </returns>
   public virtual async Task<PrivateMessageListModel> PrepareSentModelAsync(int page, string tab)
   {
      if (page > 0)
         page -= 1;

      var pageSize = _forumSettings.PrivateMessagesPageSize;

      var messages = new List<PrivateMessageModel>();
      var user = await _workContext.GetCurrentUserAsync();
      var list = await _forumService.GetAllPrivateMessagesAsync(user.Id, 0, null, false, null, string.Empty, page, pageSize);

      foreach (var pm in list)
         messages.Add(await PreparePrivateMessageModelAsync(pm));

      var pagerModel = new PagerModel(_localizationService)
      {
         PageSize = list.PageSize,
         TotalRecords = list.TotalCount,
         PageIndex = list.PageIndex,
         ShowTotalSummary = false,
         RouteActionName = "PrivateMessagesPaged",
         UseRouteLinks = true,
         RouteValues = new PrivateMessageRouteValues { pageNumber = page, tab = tab }
      };

      var model = new PrivateMessageListModel
      {
         Messages = messages,
         PagerModel = pagerModel
      };

      return model;
   }

   /// <summary>
   /// Prepare the send private message model
   /// </summary>
   /// <param name="userTo">User, recipient of the message</param>
   /// <param name="replyToPM">Private message, pass if reply to a previous message is need</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the send private message model
   /// </returns>
   public virtual async Task<SendPrivateMessageModel> PrepareSendPrivateMessageModelAsync(User userTo, PrivateMessage replyToPM)
   {
      if (userTo == null)
         throw new ArgumentNullException(nameof(userTo));

      var model = new SendPrivateMessageModel
      {
         ToUserId = userTo.Id,
         UserToName = await _userService.FormatUsernameAsync(userTo),
         AllowViewingToProfile = _userSettings.AllowViewingProfiles && !await _userService.IsGuestAsync(userTo)
      };

      if (replyToPM == null)
         return model;

      var user = await _workContext.GetCurrentUserAsync();
      if (replyToPM.ToUserId == user.Id ||
          replyToPM.FromUserId == user.Id)
      {
         model.ReplyToMessageId = replyToPM.Id;
         model.Subject = $"Re: {replyToPM.Subject}";
      }

      return model;
   }

   /// <summary>
   /// Prepare the private message model
   /// </summary>
   /// <param name="pm">Private message</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the private message model
   /// </returns>
   public virtual async Task<PrivateMessageModel> PreparePrivateMessageModelAsync(PrivateMessage pm)
   {
      if (pm == null)
         throw new ArgumentNullException(nameof(pm));

      var fromUser = await _userService.GetUserByIdAsync(pm.FromUserId);
      var toUser = await _userService.GetUserByIdAsync(pm.ToUserId);

      var model = new PrivateMessageModel
      {
         Id = pm.Id,
         FromUserId = pm.FromUserId,
         UserFromName = await _userService.FormatUsernameAsync(fromUser),
         AllowViewingFromProfile = _userSettings.AllowViewingProfiles && !await _userService.IsGuestAsync(fromUser),
         ToUserId = pm.ToUserId,
         UserToName = await _userService.FormatUsernameAsync(toUser),
         AllowViewingToProfile = _userSettings.AllowViewingProfiles && !await _userService.IsGuestAsync(toUser),
         Subject = pm.Subject,
         Message = _forumService.FormatPrivateMessageText(pm),
         CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(pm.CreatedOnUtc, DateTimeKind.Utc),
         IsRead = pm.IsRead,
      };

      return model;
   }

   #endregion
}