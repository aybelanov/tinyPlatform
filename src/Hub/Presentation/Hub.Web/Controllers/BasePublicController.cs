using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Controllers
{
   [WwwRequirement]
   [CheckLanguageSeoCode]
   [CheckAccessPublicPlatform]
   [CheckAccessClosedPlatform]
   public abstract partial class BasePublicController : BaseController
   {
      protected virtual IActionResult InvokeHttp404()
      {
         Response.StatusCode = 404;
         return new EmptyResult();
      }
   }
}