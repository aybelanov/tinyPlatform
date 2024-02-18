using Hub.Core;
using Hub.Data;
using Hub.Services.Clients;
using Hub.Services.Clients.Records;
using Hub.Services.Common;
using Hub.Services.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Media;

/// <summary>
/// Represents video segment cache middle ware
/// </summary>
public class VideoCacheMiddleware
{
   #region fields
  
   private readonly RequestDelegate _next;
   private static readonly string storageDir = CommonHelper.DefaultFileProvider.GetAbsolutePath(ClientDefaults.VideoStorageDirectory);

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public VideoCacheMiddleware(RequestDelegate next)
   {
      _next = next;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Invoke middleware actions
   /// </summary>
   /// <param name="context">HTTP context</param>
   /// <param name="permissionService">Permission service</param>
   /// <param name="recordService">Sensor record service</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public async Task InvokeAsync(HttpContext context, IPermissionService permissionService, ISensorRecordService recordService)
   {
      if (context.User.Identity.IsAuthenticated && await permissionService.AuthorizeAsync(StandardPermissionProvider.AllowGetData)
      && context.Request.Path.StartsWithSegments(new PathString(ClientDefaults.IpCamEndpoint), out var fileSegment)
      && fileSegment.HasValue)
      {
         var fileInfo = new FileInfo(Path.Combine(storageDir, fileSegment.Value.TrimStart('/')));
         if (!fileInfo.Exists && Guid.TryParse(Path.GetFileNameWithoutExtension(fileInfo.Name), out var guid))
         {
            var segment = await recordService.GetSegmentByGuidAsync(guid);
            if (segment != null)
            {
               File.WriteAllBytes(Path.Combine(storageDir, fileInfo.Name), segment.Binary);
            }
         }
         await _next(context);
      }
      else
      {
         context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
      }
   }

   #endregion
}
