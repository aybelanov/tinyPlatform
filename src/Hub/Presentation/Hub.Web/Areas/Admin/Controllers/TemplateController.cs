using Hub.Core.Domain.Topics;
using Hub.Services.Localization;
using Hub.Services.Security;
using Hub.Services.Topics;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Templates;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Controllers
{
   public partial class TemplateController : BaseAdminController
   {
      #region Fields

      private readonly ILocalizationService _localizationService;
      private readonly IPermissionService _permissionService;
      private readonly ITemplateModelFactory _templateModelFactory;
      private readonly ITopicTemplateService _topicTemplateService;

      #endregion

      #region Ctor

      public TemplateController(ILocalizationService localizationService,
          IPermissionService permissionService,
          ITemplateModelFactory templateModelFactory,
          ITopicTemplateService topicTemplateService)
      {
         _localizationService = localizationService;
         _permissionService = permissionService;
         _templateModelFactory = templateModelFactory;
         _topicTemplateService = topicTemplateService;
      }

      #endregion

      #region Methods

      public virtual async Task<IActionResult> List()
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();

         //prepare model
         var model = await _templateModelFactory.PrepareTemplatesModelAsync(new TemplatesModel());

         return View(model);
      }

      #region Topic templates

      [HttpPost]
      public virtual async Task<IActionResult> TopicTemplates(TopicTemplateSearchModel searchModel)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return await AccessDeniedDataTablesJson();

         //prepare model
         var model = await _templateModelFactory.PrepareTopicTemplateListModelAsync(searchModel);

         return Json(model);
      }

      [HttpPost]
      public virtual async Task<IActionResult> TopicTemplateUpdate(TopicTemplateModel model)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();

         if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

         //try to get a topic template with the specified id
         var template = await _topicTemplateService.GetTopicTemplateByIdAsync(model.Id)
             ?? throw new ArgumentException("No template found with the specified id");

         template = model.ToEntity(template);
         await _topicTemplateService.UpdateTopicTemplateAsync(template);

         return new NullJsonResult();
      }

      [HttpPost]
      public virtual async Task<IActionResult> TopicTemplateAdd(TopicTemplateModel model)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();

         if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

         var template = new TopicTemplate();
         template = model.ToEntity(template);
         await _topicTemplateService.InsertTopicTemplateAsync(template);

         return Json(new { Result = true });
      }

      [HttpPost]
      public virtual async Task<IActionResult> TopicTemplateDelete(long id)
      {
         if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();

         if ((await _topicTemplateService.GetAllTopicTemplatesAsync()).Count == 1)
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

         //try to get a topic template with the specified id
         var template = await _topicTemplateService.GetTopicTemplateByIdAsync(id)
             ?? throw new ArgumentException("No template found with the specified id");

         await _topicTemplateService.DeleteTopicTemplateAsync(template);

         return new NullJsonResult();
      }

      #endregion

      #endregion
   }
}