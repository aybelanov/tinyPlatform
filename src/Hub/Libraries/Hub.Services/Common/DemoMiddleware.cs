using Hub.Services.Localization;
using Hub.Services.Messages;
using Microsoft.AspNetCore.Http;
using Shared.Clients.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hub.Services.Common;

/// <summary>
/// Demo mode middleware
/// </summary>
/// <param name="next"></param>
public class DemoMiddleware(RequestDelegate next)
{
   #region fields
   /// <summary>
   /// Spoofed IP addpes for demo user 
   /// </summary>
   public static readonly IPAddress DemoIp = IPAddress.Parse("80.64.25.107");

   private static readonly string[] mvcPostAllowedPaths =
   [
      // SignalR
      "/dashhub",

      "/connect",

      // install POST 
      "/install",

      "List",
      "/Admin/User/AddressesSelect",
      "/Admin/User/SharedDevicesSelect",
      "/Admin/User/ListActivityLog",
      "/Admin/User/SharedDevicesSelect",
      "/Admin/User/OwnDevicesSelect",
      "/Admin/News/Comments",
      "/Admin/Blog/Comments",
      "/admin/preferences/savepreference",
      "/Admin/Setting/AllSettings",
      "/Admin/Country/States",
      "/Admin/Language/GetAvailableFlagFileNames",
      "/Admin/Language/Resources",
      "/Admin/Authentication/ExternalMethods",
      "/Admin/Authentication/MultiFactorMethods",
      "/Admin/Plugin/OfficialFeedSelect",
      "/Admin/Common/SeNames",
      "/Admin/Template/TopicTemplates",
      "/Admin/Device/LoadOwnerName",
      "/Admin/Device/DeviceUsersSelect",
      "/Admin/User/ExportXML",
      "/Admin/User/ExportExcel",
      "/Admin/Device/ExportExcel",
      "/Admin/Device/ExportXML",
      "/Admin/Poll/PollAnswers",
      "/Admin/Setting/ForceMultifactorAuthenticationWarning",
      "/Admin/Setting/DistributedCacheHighTrafficWarning",
      "/changecurrency",
      "/changelanguage",
      "/user/addresses",
      //"/user/changepassword",
      "/user/checkusernameavailability",
      "/eucookielawaccept",
      "/inboxupdate",
      "/newsletter/subscriptionactivation",
      "/poll/vote",
      "/privatemessages",
      "/search?",
      "/sendpm",
      "/sentupdate",
      "/serviceclosed",
      "/subscribenewsletter",
      "/topic/authenticate",
      "/viewpm",
      "/wishlist",
   ];

   private static readonly string[] grpcExcludedTemplate =
   [
      "Get",
      "Check"
   ];

   #endregion

   #region Methods

   /// <summary>
   /// Invoke middleware actions
   /// </summary>
   /// <param name="context">HTTP context</param>
   /// <param name="notification">Notifiction service</param>
   /// <param name="localization">Localization service</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public async Task InvokeAsync(HttpContext context, INotificationService notification, ILocalizationService localization)
   {
      //whether database is installed
      if (context.User?.Identity.IsAuthenticated == true && context.User.IsInRole(UserDefaults.DemoRoleName))
      {
         if (context.Request.Method.Equals("POST"))
         {
            var pathString = context.Request.Path.ToString();

            // grpc request
            if (pathString.StartsWith("/clientpackage."))
            {
               if (!grpcExcludedTemplate.Where(pathString.Contains).Any())
               {
                  context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                  //await context.Response.WriteAsJsonAsync(new { Error = "It is a demo account. Demo user cannot change data." });
                  return;
               }
            }
            // clents webapi
            else if (pathString.StartsWith("/webapi/Device/UploadIcon", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/webapi/Sensor/UploadIcon", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/webapi/Widget/UploadIcon", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/webapi/Widget/UploadLiveScheme", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            // MVC request
            // button action
            else if (pathString.StartsWith("/Admin/UserActivityLog/ClearAll", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/UserActivityLog/ActivityLogs?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/DeviceActivityLog/ClearAll", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/DeviceActivityLog/ActivityLogs?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/NewsLetterSubscription/ExportCSV", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/NewsLetterSubscription/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/NewsLetterSubscription/ImportCsv", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/NewsLetterSubscription/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/News/ApproveSelected", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/News/NewsComments?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/News/DisapproveSelected", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/News/NewsComments?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/News/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/News/NewsItems?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/News/CommentDelete", StringComparison.InvariantCultureIgnoreCase))
            {
               //context.Response.Redirect($"/Admin/News/NewsComments?notify=demo
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/News/CommentUpdate", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/News/DeleteSelectedComments", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/News/NewsComments?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Blog/ApproveSelected", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Blog/BlogComments?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Blog/DisapproveSelected", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Blog/BlogComments?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Blog/DeleteSelectedComments", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Blog/BlogComments?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Blog/CommentDelete", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Blog/CommentUpdate", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Blog/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Blog/BlogPosts?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Log/List", StringComparison.InvariantCultureIgnoreCase) || pathString.StartsWith("/Admin/Log/DeleteSelected", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Log/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/User/SendEmail", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/User/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/User/SendPm", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/User/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/User/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/User/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/UserRole/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/UserRole/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/UserActivityLog/SaveTypes", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/UserActivityLog/ActivityTypes?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Device/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Device/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/DeviceActivityLog/SaveTypes", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/DeviceActivityLog/ActivityTypes?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/MessageTemplate/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/MessageTemplate/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Topic/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Topic/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Poll/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Poll/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Poll/PollAnswerUpdate", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Poll/PollAnswerDelete", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Forum/DeleteForumGroup", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Forum/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Forum/DeleteForum", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Forum/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/EmailAccount/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/EmailAccount/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Country/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Country/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Country/PublishSelected", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Country/UnpublishSelected", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Country/ImportCsv", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Country/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Country/StateDelete", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Country/StateCreatePopup", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Language/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Language/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Language/ImportXml", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Language/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Language/ResourceUpdate", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Language/ResourceDelete", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Language/ResourceAdd", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Currency/MarkAsPrimaryExchangeRateCurrency", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Currency/MarkAsPrimaryPlatformCurrency", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/Currency/Delete", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Currency/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Common/RestartApplication", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Common/ClearCache", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/Plugin/UploadPluginsAndThemes", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/Plugin/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/QueuedEmail/DeleteSelected", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            else if (pathString.StartsWith("/Admin/QueuedEmail/List", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/QueuedEmail/List?notify=demo");
               return;
            }
            else if (pathString.StartsWith("/Admin/ScheduleTask/TaskUpdate", StringComparison.InvariantCultureIgnoreCase))
            {
               await context.Response.WriteAsJsonAsync(new { Error = "It's a demo account. Demo user cannot change data." });
               return;
            }
            // allowed action
            else if (!mvcPostAllowedPaths.Where(x => pathString.Contains(x, StringComparison.InvariantCultureIgnoreCase)).Any())
            {
               context.Response.Redirect($"{pathString}?notify=demo");
               return;
            }
         }
         else if (context.Request.Method.Equals("GET"))
         {
            if (context.Request.Query.ContainsKey("notify"))
            {
               notification.SuccessNotification(await localization.GetResourceAsync("Demo.PermissionDenied.Notification"));
            }
            else if (context.Request.Path.StartsWithSegments("/Admin/ScheduleTask/RunNow", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/ScheduleTask/List?notify=demo");
               return;
            }
            else if (context.Request.Path.StartsWithSegments("/Admin/EmailAccount/MarkAsDefaultEmail", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin/EmailAccount/List?notify=demo");
               return;
            }
            else if (context.Request.Path.StartsWithSegments("/Admin/Common/RestartApplication", StringComparison.InvariantCultureIgnoreCase))
            {
               context.Response.Redirect($"/Admin?notify=demo");
               return;
            }
         }
      }

      //or call the next middleware in the request pipeline
      await next(context);
   }

   #endregion
}
