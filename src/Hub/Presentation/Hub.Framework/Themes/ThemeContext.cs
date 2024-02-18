using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain;
using Hub.Core.Domain.Users;
using Hub.Services.Common;
using Hub.Services.Themes;

namespace Hub.Web.Framework.Themes
{
   /// <summary>
   /// Represents the theme context implementation
   /// </summary>
   public partial class ThemeContext : IThemeContext
   {
      #region Fields

      private readonly IGenericAttributeService _genericAttributeService;
      private readonly IThemeProvider _themeProvider;
      private readonly IWorkContext _workContext;
      private readonly AppInfoSettings _appInformationSettings;

      private string _cachedThemeName;

      #endregion

      #region Ctor

      /// <summary> IoC Ctor </summary>
      public ThemeContext(IGenericAttributeService genericAttributeService,
          IThemeProvider themeProvider,
          IWorkContext workContext,
          AppInfoSettings appInformationSettings)
      {
         _genericAttributeService = genericAttributeService;
         _themeProvider = themeProvider;
         _workContext = workContext;
         _appInformationSettings = appInformationSettings;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Get or set current theme system name
      /// </summary>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task<string> GetWorkingThemeNameAsync()
      {
         if (!string.IsNullOrEmpty(_cachedThemeName))
            return _cachedThemeName;

         var themeName = string.Empty;

         //whether users are allowed to select a theme
         var user = await _workContext.GetCurrentUserAsync();
         if (_appInformationSettings.AllowUserToSelectTheme &&
             user != null)
         {
            themeName = await _genericAttributeService.GetAttributeAsync<string>(user,
                AppUserDefaults.WorkingThemeNameAttribute);
         }

         //if not, try to get default platform theme
         if (string.IsNullOrEmpty(themeName))
            themeName = _appInformationSettings.DefaultAppTheme;

         //ensure that this theme exists
         if (!await _themeProvider.ThemeExistsAsync(themeName))
            //if it does not exist, try to get the first one
            themeName = (await _themeProvider.GetThemesAsync()).FirstOrDefault()?.SystemName
                        ?? throw new Exception("No theme could be loaded");

         //cache theme system name
         _cachedThemeName = themeName;

         return themeName;
      }

      /// <summary>
      /// GetTable current theme system name
      /// </summary>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task SetWorkingThemeNameAsync(string workingThemeName)
      {
         //whether users are allowed to select a theme
         var user = await _workContext.GetCurrentUserAsync();
         if (!_appInformationSettings.AllowUserToSelectTheme ||
             user == null)
            return;

         //save selected by user theme system name
         await _genericAttributeService.SaveAttributeAsync(user,
             AppUserDefaults.WorkingThemeNameAttribute, workingThemeName);

         //clear cache
         _cachedThemeName = null;
      }

      #endregion
   }
}