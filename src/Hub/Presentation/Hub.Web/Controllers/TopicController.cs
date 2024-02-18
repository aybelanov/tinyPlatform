﻿using System.Threading.Tasks;
using Hub.Web.Factories;
using Microsoft.AspNetCore.Mvc;
using Hub.Services.Localization;
using Hub.Services.Security;
using Hub.Services.Topics;
using Hub.Web.Framework;
using Hub.Web.Framework.Mvc.Filters;

namespace Hub.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class TopicController : BasePublicController
{
   #region Fields

   private readonly IAclService _aclService;
   private readonly ILocalizationService _localizationService;
   private readonly IPermissionService _permissionService; 
   private readonly ITopicModelFactory _topicModelFactory;
   private readonly ITopicService _topicService;

   #endregion

   #region Ctor

   public TopicController(IAclService aclService,
       ILocalizationService localizationService,
       IPermissionService permissionService,
       ITopicModelFactory topicModelFactory,
       ITopicService topicService)
   {
      _aclService = aclService;
      _localizationService = localizationService;
      _permissionService = permissionService;
      _topicModelFactory = topicModelFactory;
      _topicService = topicService;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> TopicDetails(long topicId)
   {
      //allow administrators to preview any topic
      var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics);

      var model = await _topicModelFactory.PrepareTopicModelByIdAsync(topicId, hasAdminAccess);
      if (model == null)
         return InvokeHttp404();

      //display "edit" (manage) link
      if (hasAdminAccess)
         DisplayEditLink(Url.Action("Edit", "Topic", new { id = model.Id, area = AreaNames.Admin }));

      //template
      var templateViewPath = await _topicModelFactory.PrepareTemplateViewPathAsync(model.TopicTemplateId);
      return View(templateViewPath, model);
   }

   [CheckLanguageSeoCode(true)]
   public virtual async Task<IActionResult> TopicDetailsPopup(string systemName)
   {
      var model = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(systemName);
      if (model == null)
         return InvokeHttp404();

      ViewBag.IsPopup = true;

      //template
      var templateViewPath = await _topicModelFactory.PrepareTemplateViewPathAsync(model.TopicTemplateId);
      return PartialView(templateViewPath, model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> Authenticate(long id, string password)
   {
      var authResult = false;
      var title = string.Empty;
      var body = string.Empty;
      var error = string.Empty;

      var topic = await _topicService.GetTopicByIdAsync(id);
      if (topic != null &&
          topic.Published &&
          //password protected?
          topic.IsPasswordProtected &&
          //ACL (access control list)
          await _aclService.AuthorizeAsync(topic))
         if (topic.Password != null && topic.Password.Equals(password))
         {
            authResult = true;
            title = await _localizationService.GetLocalizedAsync(topic, x => x.Title);
            body = await _localizationService.GetLocalizedAsync(topic, x => x.Body);
         }
         else
            error = await _localizationService.GetResourceAsync("Topic.WrongPassword");

      return Json(new { Authenticated = authResult, Title = title, Body = body, Error = error });
   }

   #endregion
}