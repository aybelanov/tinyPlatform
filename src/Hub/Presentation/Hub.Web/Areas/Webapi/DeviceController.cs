using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Services.Clients;
using Hub.Services.Devices;
using Hub.Services.Localization;
using Hub.Services.Media;
using Hub.Services.Users;
using Hub.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Webapi;

public class DeviceController(ILocalizationService localizationService,
   IHubDeviceService deviceService,
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
      var device = await deviceService.GetDeviceByIdAsync(id);

      if (device is null)
         return NotFound(new { Error = "Device not found." });

      if (device.OwnerId != user.Id && !await userService.IsAdminAsync(user))
         return Unauthorized(new { Error = "Permission denied" });

      var contentType = uploadedFile?.ContentType.ToLowerInvariant();

      if (contentType is null || (!contentType.Equals("image/jpeg") && !contentType.Equals("image/gif") && !contentType.Equals("image/png")))
         return BadRequest(new { Error = await localizationService.GetResourceAsync("Account.Avatar.UploadRules")});

      try
      {
         var deviceIcon = await pictureService.GetPictureByIdAsync(device.PictureId);
         if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
         {
            var iconMaxSize = userSettings.AvatarMaximumSizeBytes;
            if (uploadedFile.Length > iconMaxSize)
               throw new AppException(string.Format(await localizationService.GetResourceAsync("Account.Avatar.MaximumUploadedFileSize"), iconMaxSize));

            var iconPictureBinary = await downloadService.GetDownloadBitsAsync(uploadedFile);

            var seoFileName = Path.GetFileNameWithoutExtension(uploadedFile.FileName); //Guid.NewGuid().ToString("N")); //HashHelper.CreateHash(iconPictureBinary, "SHA1"));
            if (deviceIcon != null)
               deviceIcon = await pictureService.UpdatePictureAsync(deviceIcon.Id, iconPictureBinary, contentType, seoFileName); 
            else
               deviceIcon = await pictureService.InsertPictureAsync(iconPictureBinary, contentType, seoFileName);
         }

         var deviceIconId = deviceIcon?.Id ?? 0L;

         device.PictureId = deviceIconId;
         await deviceService.UpdateDeviceAsync(device);

         var iconUrl = await pictureService.GetPictureUrlAsync(device.PictureId);

         return Ok(new { Success = iconUrl, Error = string.Empty });
      }
      catch (Exception ex)
      {
         return BadRequest(new { Error = ex.Message });
      }
   }

   #endregion
}
