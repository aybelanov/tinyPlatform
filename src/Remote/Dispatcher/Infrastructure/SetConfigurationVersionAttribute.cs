using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Shared.Devices;
using Devices.Dispatcher.Services.Settings;
using Devices.Dispatcher.Configuration;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace Devices.Dispatcher.Infrastructure;

#pragma warning disable CS1591

/// <summary>
/// Represents the control configuration version and dispather  unique process attribute for incomming requests for sensor handlers 
/// </summary>
public class SetConfigurationVersionAttribute : TypeFilterAttribute
{
   public SetConfigurationVersionAttribute() : base(typeof(SetConfigurationVersionFilter))
   {
   }

   private class SetConfigurationVersionFilter : IAsyncResultFilter
   {
      private readonly ISettingService _settingService;


      public SetConfigurationVersionFilter(ISettingService settingService)
      {
         _settingService = settingService;
      }


      public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
      {
         var deviceSettings = await _settingService.LoadSettingAsync<DeviceSettings>();
         context.HttpContext.Response.Headers.Append(DispatcherDefaults.HeaderConfigurationKey, new StringValues(deviceSettings.ModifiedTicks.ToString()));

         // for handler reboot control for example
         if (Defaults.UniqueProcessGuid == Guid.Empty)
            Defaults.UniqueProcessGuid = Guid.NewGuid();

         context.HttpContext.Response.Headers.Append(DispatcherDefaults.HeaderProcessIdKey, new StringValues(Defaults.UniqueProcessGuid.ToString()));

         await next();
      }
   }
}
