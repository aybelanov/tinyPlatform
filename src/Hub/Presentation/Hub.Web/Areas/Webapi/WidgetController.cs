using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Services.Clients.Widgets;
using Hub.Services.Localization;
using Hub.Services.Media;
using Hub.Services.Users;
using Hub.Web.Framework.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Clients.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Webapi;

[Route("webapi/[controller]/[action]")]
public class WidgetController(ILocalizationService localizationService,
   IWidgetService widgetService,
   IWorkContext workContext,
   IPictureService pictureService,
   IUserService userService,
   IDownloadService downloadService,
   UserSettings userSettings) : BaseWebapiController
{
   #region Methods

   [HttpPost("{id:long}")]
   [Produces("application/json")]
   public async Task<IActionResult> UploadIcon(long id)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);

      var uploadedFile = Request.Form.Files[0];
      //var iconSize = int.TryParse(Request.Form["iconSize"], out var _size) ? _size : 32;

      if (uploadedFile is null)
         return BadRequest(new { Error = "No files presented." });

      var user = await workContext.GetCurrentUserAsync();

      // security
      var widget = await widgetService.GetByIdAsync(id);

      if (widget is null)
         return NotFound(new { Error = "Widget not found." });

      if (widget.UserId != user.Id && !await userService.IsAdminAsync(user))
         return Unauthorized(new { Error = "Permission denied" });

      var contentType = uploadedFile?.ContentType.ToLowerInvariant();

      if (contentType is null || (!contentType.Equals("image/jpeg") && !contentType.Equals("image/gif") && !contentType.Equals("image/png")))
         return BadRequest(new { Error = await localizationService.GetResourceAsync("Account.Avatar.UploadRules") });

      try
      {
         var widgetIcon = await pictureService.GetPictureByIdAsync(widget.PictureId);
         if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
         {
            var iconMaxSize = userSettings.AvatarMaximumSizeBytes;
            if (uploadedFile.Length > iconMaxSize)
               throw new AppException(string.Format(await localizationService.GetResourceAsync("Account.Avatar.MaximumUploadedFileSize"), iconMaxSize));

            var iconPictureBinary = await downloadService.GetDownloadBitsAsync(uploadedFile);

            var seoFileName = Path.GetFileNameWithoutExtension(uploadedFile.FileName); //Guid.NewGuid().ToString("N")); //HashHelper.CreateHash(iconPictureBinary, "SHA1"));
            if (widgetIcon != null)
               widgetIcon = await pictureService.UpdatePictureAsync(widgetIcon.Id, iconPictureBinary, contentType, seoFileName);
            else
               widgetIcon = await pictureService.InsertPictureAsync(iconPictureBinary, contentType, seoFileName);
         }

         var widgetIconId = widgetIcon?.Id ?? 0L;

         widget.PictureId = widgetIconId;
         await widgetService.UpdateAsync(widget, false);

         var iconUrl = await pictureService.GetPictureUrlAsync(widget.PictureId);

         return Ok(new { Success = iconUrl, Error = string.Empty });
      }
      catch (Exception ex)
      {
         return BadRequest(new { Error = ex.Message });
      }
   }


   [HttpPost("{id:long}")]
   [Produces("application/json")]
   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public async Task<IActionResult> UploadLiveScheme(long id)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);

      var uploadedFile = Request.Form.Files[0];

      if (uploadedFile is null)
         return BadRequest(new { Error = "No files presented." });

      var user = await workContext.GetCurrentUserAsync();

      // security
      var widget = await widgetService.GetByIdAsync(id);

      if (widget is null)
         return NotFound(new { Error = "Widget not found." });

      if (widget.UserId != user.Id && !await userService.IsAdminAsync(user))
         return Unauthorized(new { Error = "Permission denied" });

      var contentType = uploadedFile?.ContentType.ToLowerInvariant();

      if (contentType is null || !contentType.Equals("image/svg+xml"))
         return BadRequest(new { Error = await localizationService.GetResourceAsync("Account.Avatar.UploadRules") });

      var widgetLiveScheme = await pictureService.GetPictureByIdAsync(widget.LiveSchemePictureId);
      
      if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
      {
         // TODO settings for SVG live schemes
         var iconMaxSize = 150_000;
         if (uploadedFile.Length > iconMaxSize)
            throw new AppException(string.Format(await localizationService.GetResourceAsync("Account.Avatar.MaximumUploadedFileSize"), iconMaxSize));

         var svgPictureBinary = await downloadService.GetDownloadBitsAsync(uploadedFile);

         var seoFileName = Path.GetFileNameWithoutExtension(uploadedFile.FileName);
        
         if (widgetLiveScheme != null)
            widgetLiveScheme = await pictureService.UpdatePictureAsync(widgetLiveScheme.Id, svgPictureBinary, contentType, seoFileName);
         else
            widgetLiveScheme = await pictureService.InsertPictureAsync(svgPictureBinary, contentType, seoFileName);
      }

      var widgetLiveSchemeId = widgetLiveScheme?.Id ?? 0L;

      widget.LiveSchemePictureId = widgetLiveSchemeId;
      await widgetService.UpdateAsync(widget, false);

      var iconUrl = await pictureService.GetPictureUrlAsync(widget.LiveSchemePictureId);

      return Ok(new { Success = iconUrl, Error = string.Empty });
   }
   #endregion
}
