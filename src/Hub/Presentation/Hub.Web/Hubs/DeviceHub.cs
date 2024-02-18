using Hub.Web.Framework.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace Hub.Web.Hubs
{
   /// <summary>
   /// A point-to-point unit to the servser service
   /// </summary>
   [Authorize(Policy = "Unit")]
   [EnableCors]
   public class DeviceHub : BaseHub
   {
   }
}