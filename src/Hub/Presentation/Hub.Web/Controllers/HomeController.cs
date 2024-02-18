using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Controllers
{
   public partial class HomeController : BasePublicController
   {
      public virtual IActionResult Index()
      {
         return View();
      }
   }
}