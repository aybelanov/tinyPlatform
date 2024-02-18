using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Users;
using Hub.Services.Common;
using Hub.Services.Configuration;
using Hub.Services.Localization;
using Hub.Services.Messages;
using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Models.Common;
using Hub.Web.Areas.Admin.Models.Home;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class HomeController : BaseAdminController
{
   #region Fields

   private readonly AdminAreaSettings _adminAreaSettings;
   private readonly ICommonModelFactory _commonModelFactory;
   private readonly IHomeModelFactory _homeModelFactory;
   private readonly ILocalizationService _localizationService;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly ISettingService _settingService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public HomeController(AdminAreaSettings adminAreaSettings,
       ICommonModelFactory commonModelFactory,
       IHomeModelFactory homeModelFactory,
       ILocalizationService localizationService,
       INotificationService notificationService,
       IPermissionService permissionService,
       ISettingService settingService,
       IGenericAttributeService genericAttributeService,
       IWorkContext workContext)
   {
      _adminAreaSettings = adminAreaSettings;
      _commonModelFactory = commonModelFactory;
      _homeModelFactory = homeModelFactory;
      _localizationService = localizationService;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _settingService = settingService;
      _workContext = workContext;
      _genericAttributeService = genericAttributeService;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> Index()
   {
      //display a warning to a platform owner if there are some error
      var user = await _workContext.GetCurrentUserAsync();
      var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.HideConfigurationStepsAttribute);
      var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.CloseConfigurationStepsAttribute);

      if ((hideCard || closeCard) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
      {
         var warnings = await _commonModelFactory.PrepareSystemWarningModelsAsync();
         if (warnings.Any(warning => warning.Level == SystemWarningLevel.Fail ||
                                     warning.Level == SystemWarningLevel.CopyleftRemovalKey ||
                                     warning.Level == SystemWarningLevel.Warning))
            _notificationService.WarningNotification(
                string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.Errors"),
                Url.Action("Warnings", "Common")),
                //do not encode URLs
                false);
      }

      //prepare model
      var model = await _homeModelFactory.PrepareDashboardModelAsync(new DashboardModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> TinyPlatformNewsHideAdv()
   {
      _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
      await _settingService.SaveSettingAsync(_adminAreaSettings);

      return Content("Setting changed");
   }

   #endregion
}