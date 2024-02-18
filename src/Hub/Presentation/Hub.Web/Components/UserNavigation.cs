﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;

namespace Hub.Web.Components;

public class UserNavigationViewComponent : AppViewComponent
{
   private readonly IUserModelFactory _userModelFactory;

   public UserNavigationViewComponent(IUserModelFactory userModelFactory)
   {
      _userModelFactory = userModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync(int selectedTabId = 0)
   {
      var model = await _userModelFactory.PrepareUserNavigationModelAsync(selectedTabId);
      return View(model);
   }
}
