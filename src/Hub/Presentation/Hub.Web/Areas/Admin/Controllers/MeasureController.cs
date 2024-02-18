using System;
using System.Threading.Tasks;
using Hub.Core.Domain.Directory;
using Hub.Services.Configuration;
using Hub.Services.Directory;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Directory;
using Hub.Web.Framework.Mvc;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class MeasureController : BaseAdminController
{
   #region Fields

   private readonly IUserActivityService _userActivityService;
   private readonly ILocalizationService _localizationService;
   private readonly IMeasureModelFactory _measureModelFactory;
   private readonly IMeasureService _measureService;
   private readonly IPermissionService _permissionService;
   private readonly ISettingService _settingService;
   private readonly MeasureSettings _measureSettings;

   #endregion

   #region Ctor

   public MeasureController(IUserActivityService userActivityService,
       ILocalizationService localizationService,
       IMeasureModelFactory measureModelFactory,
       IMeasureService measureService,
       IPermissionService permissionService,
       ISettingService settingService,
       MeasureSettings measureSettings)
   {
      _userActivityService = userActivityService;
      _localizationService = localizationService;
      _measureModelFactory = measureModelFactory;
      _measureService = measureService;
      _permissionService = permissionService;
      _settingService = settingService;
      _measureSettings = measureSettings;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> List()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return AccessDeniedView();

      //prepare model
      var model = await _measureModelFactory.PrepareMeasureSearchModelAsync(new MeasureSearchModel());

      return View(model);
   }

   #region Weights

   [HttpPost]
   public virtual async Task<IActionResult> Weights(MeasureWeightSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _measureModelFactory.PrepareMeasureWeightListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> WeightUpdate(MeasureWeightModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return AccessDeniedView();

      if (!ModelState.IsValid)
         return ErrorJson(ModelState.SerializeErrors());

      var weight = await _measureService.GetMeasureWeightByIdAsync(model.Id);
      weight = model.ToEntity(weight);
      await _measureService.UpdateMeasureWeightAsync(weight);

      //activity log
      await _userActivityService.InsertActivityAsync("EditMeasureWeight",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMeasureWeight"), weight.Id), weight);

      return new NullJsonResult();
   }

   [HttpPost]
   public virtual async Task<IActionResult> WeightAdd(MeasureWeightModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return AccessDeniedView();

      if (!ModelState.IsValid)
         return ErrorJson(ModelState.SerializeErrors());

      var weight = new MeasureWeight();
      weight = model.ToEntity(weight);
      await _measureService.InsertMeasureWeightAsync(weight);

      //activity log
      await _userActivityService.InsertActivityAsync("AddNewMeasureWeight",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewMeasureWeight"), weight.Id), weight);

      return Json(new { Result = true });
   }

   [HttpPost]
   public virtual async Task<IActionResult> WeightDelete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return AccessDeniedView();

      //try to get a weight with the specified id
      var weight = await _measureService.GetMeasureWeightByIdAsync(id)
          ?? throw new ArgumentException("No weight found with the specified id", nameof(id));

      if (weight.Id == _measureSettings.BaseWeightId)
         return ErrorJson(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Weights.CantDeletePrimary"));

      await _measureService.DeleteMeasureWeightAsync(weight);

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteMeasureWeight",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMeasureWeight"), weight.Id), weight);

      return new NullJsonResult();
   }

   [HttpPost]
   public virtual async Task<IActionResult> MarkAsPrimaryWeight(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return AccessDeniedView();

      //try to get a weight with the specified id
      var weight = await _measureService.GetMeasureWeightByIdAsync(id)
          ?? throw new ArgumentException("No weight found with the specified id", nameof(id));

      _measureSettings.BaseWeightId = weight.Id;
      await _settingService.SaveSettingAsync(_measureSettings);

      return Json(new { result = true });
   }

   #endregion

   #region Dimensions

   [HttpPost]
   public virtual async Task<IActionResult> Dimensions(MeasureDimensionSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _measureModelFactory.PrepareMeasureDimensionListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> DimensionUpdate(MeasureDimensionModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return AccessDeniedView();

      if (!ModelState.IsValid)
         return ErrorJson(ModelState.SerializeErrors());

      var dimension = await _measureService.GetMeasureDimensionByIdAsync(model.Id);
      dimension = model.ToEntity(dimension);
      await _measureService.UpdateMeasureDimensionAsync(dimension);

      //activity log
      await _userActivityService.InsertActivityAsync("EditMeasureDimension",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMeasureDimension"), dimension.Id), dimension);

      return new NullJsonResult();
   }

   [HttpPost]
   public virtual async Task<IActionResult> DimensionAdd(MeasureDimensionModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return AccessDeniedView();

      if (!ModelState.IsValid)
         return ErrorJson(ModelState.SerializeErrors());

      var dimension = new MeasureDimension();
      dimension = model.ToEntity(dimension);
      await _measureService.InsertMeasureDimensionAsync(dimension);

      //activity log
      await _userActivityService.InsertActivityAsync("AddNewMeasureDimension",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewMeasureDimension"), dimension.Id), dimension);

      return Json(new { Result = true });
   }

   [HttpPost]
   public virtual async Task<IActionResult> DimensionDelete(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return AccessDeniedView();

      //try to get a dimension with the specified id
      var dimension = await _measureService.GetMeasureDimensionByIdAsync(id)
          ?? throw new ArgumentException("No dimension found with the specified id", nameof(id));

      if (dimension.Id == _measureSettings.BaseDimensionId)
         return ErrorJson(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Dimensions.CantDeletePrimary"));

      await _measureService.DeleteMeasureDimensionAsync(dimension);

      //activity log
      await _userActivityService.InsertActivityAsync("DeleteMeasureDimension",
          string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMeasureDimension"), dimension.Id), dimension);

      return new NullJsonResult();
   }

   [HttpPost]
   public virtual async Task<IActionResult> MarkAsPrimaryDimension(long id)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMeasureSettings))
         return AccessDeniedView();

      //try to get a dimension with the specified id
      var dimension = await _measureService.GetMeasureDimensionByIdAsync(id)
          ?? throw new ArgumentException("No dimension found with the specified id", nameof(id));

      _measureSettings.BaseDimensionId = dimension.Id;
      await _settingService.SaveSettingAsync(_measureSettings);

      return Json(new { result = true });
   }

   #endregion

   #endregion
}