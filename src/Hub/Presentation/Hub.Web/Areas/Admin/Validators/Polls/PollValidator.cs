using FluentValidation;
using Hub.Web.Areas.Admin.Models.Polls;
using Hub.Core.Domain.Polls;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Polls
{
   public partial class PollValidator : BaseAppValidator<PollModel>
   {
      public PollValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
      {
         RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Fields.Name.Required"));

         SetDatabaseValidationRules<Poll>(mappingEntityAccessor);
      }
   }
}