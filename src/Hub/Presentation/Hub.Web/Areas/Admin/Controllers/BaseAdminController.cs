using Hub.Core.Domain.Common;
using Hub.Core.Infrastructure;
using Hub.Web.Framework;
using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Hub.Web.Areas.Admin.Controllers;

[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
[SaveSelectedTab]
[NotNullValidationMessage]
public abstract partial class BaseAdminController : BaseController
{
   /// <summary>
   /// Creates an object that serializes the specified object to JSON.
   /// </summary>
   /// <param name="data">The object to serialize.</param>
   /// <returns>The created object that serializes the specified data to JSON format for the response.</returns>
   public override JsonResult Json(object data)
   {
      //use IsoDateFormat on writing JSON text to fix issue with dates in grid
      var useIsoDateFormat = EngineContext.Current.Resolve<AdminAreaSettings>()?.UseIsoDateFormatInJsonResult ?? false;
      var serializerSettings = EngineContext.Current.Resolve<IOptions<MvcNewtonsoftJsonOptions>>()?.Value?.SerializerSettings
          ?? new JsonSerializerSettings();

      if (!useIsoDateFormat)
         return base.Json(data, serializerSettings);

      serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
      serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;

      return base.Json(data, serializerSettings);
   }

}