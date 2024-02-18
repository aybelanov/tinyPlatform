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

public class SensorController(ILocalizationService localizationService,
   IHubDeviceService deviceService,
   IHubSensorService sensorService,
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
      var sensor = await sensorService.GetSensorByIdAsync(id);

      if (sensor is null)
         return NotFound(new { Error = "Sensor has not found." });

      var device = await deviceService.GetDeviceByIdAsync(sensor.DeviceId);

      if (device is null)
         return NotFound(new { Error = "Sensor's device has not found." });

      if (device.OwnerId != user.Id && !await userService.IsAdminAsync(user))
         return Unauthorized(new { Error = "Permission denied" });

      var contentType = uploadedFile?.ContentType.ToLowerInvariant();

      if (contentType is null || (!contentType.Equals("image/jpeg") && !contentType.Equals("image/gif") && !contentType.Equals("image/png")))
         return BadRequest(new { Error = await localizationService.GetResourceAsync("Account.Avatar.UploadRules") });

      try
      {
         var sensorIcon = await pictureService.GetPictureByIdAsync(sensor.PictureId);

         if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
         {
            var iconMaxSize = userSettings.AvatarMaximumSizeBytes;
            if (uploadedFile.Length > iconMaxSize)
               throw new AppException(string.Format(await localizationService.GetResourceAsync("Account.Avatar.MaximumUploadedFileSize"), iconMaxSize));

            var iconPictureBinary = await downloadService.GetDownloadBitsAsync(uploadedFile);

            var seoFileName = Path.GetFileNameWithoutExtension(uploadedFile.FileName); //Guid.NewGuid().ToString("N")); //HashHelper.CreateHash(iconPictureBinary, "SHA1"));
            if (sensorIcon != null)
               sensorIcon = await pictureService.UpdatePictureAsync(sensorIcon.Id, iconPictureBinary, contentType, seoFileName);
            else
               sensorIcon = await pictureService.InsertPictureAsync(iconPictureBinary, contentType, seoFileName);
         }

         var sensorIconId = sensorIcon?.Id ?? 0L;

         sensor.PictureId = sensorIconId;
         await sensorService.UpdateAsync(sensor);

         var iconUrl = await pictureService.GetPictureUrlAsync(sensor.PictureId);

         return Ok(new { Success = iconUrl, Error = string.Empty });
      }
      catch (Exception ex)
      {
         return BadRequest(new { Error = ex.Message });
      }
   }

   #endregion
}
