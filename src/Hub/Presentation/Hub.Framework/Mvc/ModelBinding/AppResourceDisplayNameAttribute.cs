using System.ComponentModel;
using Hub.Core;
using Hub.Core.Infrastructure;
using Hub.Services.Localization;

namespace Hub.Web.Framework.Mvc.ModelBinding
{
   /// <summary>
   /// Represents model attribute that specifies the display name by passed key of the locale resource
   /// </summary>
   public sealed class AppResourceDisplayNameAttribute : DisplayNameAttribute, IModelAttribute
   {
      #region Fields

      private string _resourceValue = string.Empty;

      #endregion

      #region Ctor

      /// <summary>
      /// Create instance of the attribute
      /// </summary>
      /// <param name="resourceKey">Key of the locale resource</param>
      public AppResourceDisplayNameAttribute(string resourceKey) : base(resourceKey)
      {
         ResourceKey = resourceKey;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets key of the locale resource 
      /// </summary>
      public string ResourceKey { get; set; }

      /// <summary>
      /// Gets the display name
      /// </summary>
      public override string DisplayName
      {
         get
         {
            //get working language identifier
            var workingLanguageId = EngineContext.Current.Resolve<IWorkContext>().GetWorkingLanguageAsync().Result.Id;

            //get locale resource value
            _resourceValue = EngineContext.Current.Resolve<ILocalizationService>().GetResourceAsync(ResourceKey, workingLanguageId, true, ResourceKey).Result;

            return _resourceValue;
         }
      }

      /// <summary>
      /// Gets name of the attribute
      /// </summary>
      public string Name => nameof(AppResourceDisplayNameAttribute);

      #endregion
   }
}
