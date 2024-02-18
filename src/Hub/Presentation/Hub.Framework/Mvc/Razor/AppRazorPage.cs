using Hub.Core.Infrastructure;
using Hub.Services.Localization;
using Hub.Web.Framework.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Localizer = Hub.Web.Framework.Localization.Localizer;

namespace Hub.Web.Framework.Mvc.Razor;

/// <summary>
/// Web view page
/// </summary>
/// <typeparam name="TModel">Model</typeparam>
public abstract class AppRazorPage<TModel> : RazorPage<TModel>
{
   private ILocalizationService _localizationService;
   private Localizer _localizer;

   /// <summary>
   /// Get a localized resources
   /// </summary>
   public Localizer T
   {
      get
      {
         _localizationService ??= EngineContext.Current.Resolve<ILocalizationService>();
         _localizer ??= (format, args) =>
         {
            var resFormat = _localizationService.GetResourceAsync(format).Result;
           
            return string.IsNullOrEmpty(resFormat)
            ? new LocalizedString(format)
            : new LocalizedString(args == null || args.Length == 0 ? resFormat : string.Format(resFormat, args));
         };

         return _localizer;
      }
   }
}

/// <summary>
/// Web view page
/// </summary>
public abstract class AppRazorPage : AppRazorPage<dynamic>
{
}