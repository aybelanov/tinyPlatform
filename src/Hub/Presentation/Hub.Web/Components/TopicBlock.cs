﻿using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class TopicBlockViewComponent : AppViewComponent
{
   private readonly ITopicModelFactory _topicModelFactory;

   public TopicBlockViewComponent(ITopicModelFactory topicModelFactory)
   {
      _topicModelFactory = topicModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync(string systemName)
   {
      var model = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(systemName);
      if (model == null)
         return Content("");
      return View(model);
   }
}
