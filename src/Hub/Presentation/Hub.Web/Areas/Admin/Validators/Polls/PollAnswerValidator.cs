using FluentValidation;
using Hub.Core.Domain.Polls;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Models.Polls;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Polls
{
   public partial class PollAnswerValidator : BaseAppValidator<PollAnswerModel>
   {
      public PollAnswerValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
      {
         //if validation without this set rule is applied, in this case nothing will be validated
         //it's used to prevent auto-validation of child models
         RuleSet(AppValidationDefaults.ValidationRuleSet, () =>
         {
            RuleFor(model => model.Name)
                   .NotEmpty()
                   .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Answers.Fields.Name.Required"));

            SetDatabaseValidationRules<PollAnswer>(mappingEntityAccessor);
         });
      }
   }
}