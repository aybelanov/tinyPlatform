using FluentValidation;
using Hub.Web.Areas.Admin.Models.News;
using Hub.Core.Domain.News;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Services.Seo;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.News
{
   public partial class NewsItemValidator : BaseAppValidator<NewsItemModel>
   {
      public NewsItemValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
      {
         RuleFor(x => x.Title).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Fields.Title.Required"));

         RuleFor(x => x.Short).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Fields.Short.Required"));

         RuleFor(x => x.Full).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Fields.Full.Required"));

         RuleFor(x => x.SeName).Length(0, AppSeoDefaults.SearchEngineNameLength)
             .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), AppSeoDefaults.SearchEngineNameLength);

         SetDatabaseValidationRules<NewsItem>(mappingEntityAccessor);
      }
   }
}