using FluentValidation;
using Hub.Web.Areas.Admin.Models.Plugins;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Plugins
{
   public partial class PluginValidator : BaseAppValidator<PluginModel>
   {
      public PluginValidator(ILocalizationService localizationService)
      {
         RuleFor(x => x.FriendlyName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Plugins.Fields.FriendlyName.Required"));
      }
   }
}