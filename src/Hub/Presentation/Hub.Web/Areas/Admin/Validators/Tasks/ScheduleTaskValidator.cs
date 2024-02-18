using FluentValidation;
using Hub.Web.Areas.Admin.Models.Tasks;
using Hub.Core.Domain.ScheduleTasks;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Tasks;

public partial class ScheduleTaskValidator : BaseAppValidator<ScheduleTaskModel>
{
   public ScheduleTaskValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.ScheduleTasks.Name.Required"));
      RuleFor(x => x.Seconds).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("Admin.System.ScheduleTasks.Seconds.Positive"));

      SetDatabaseValidationRules<ScheduleTask>(mappingEntityAccessor);
   }
}