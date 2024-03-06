using FluentValidation;
using Hub.Core.Domain.Messages;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Messages;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Messages
{
   public partial class CampaignValidator : BaseAppValidator<CampaignModel>
   {
      public CampaignValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
      {
         RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Milticast.Campaigns.Fields.Name.Required"));

         RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Milticast.Campaigns.Fields.Subject.Required"));

         RuleFor(x => x.Body).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Milticast.Campaigns.Fields.Body.Required"));

         SetDatabaseValidationRules<Campaign>(mappingEntityAccessor);
      }
   }
}