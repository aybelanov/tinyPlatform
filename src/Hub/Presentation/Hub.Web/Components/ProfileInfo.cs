using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Services.Users;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;

namespace Hub.Web.Components;

public class ProfileInfoViewComponent : AppViewComponent
{
   private readonly IUserService _userService;
   private readonly IProfileModelFactory _profileModelFactory;

   public ProfileInfoViewComponent(IUserService userService, IProfileModelFactory profileModelFactory)
   {
      _userService = userService;
      _profileModelFactory = profileModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync(long userProfileId)
   {
      var user = await _userService.GetUserByIdAsync(userProfileId);
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      var model = await _profileModelFactory.PrepareProfileInfoModelAsync(user);
      return View(model);
   }
}
