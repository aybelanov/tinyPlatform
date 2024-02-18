using FluentValidation;
using Hub.Web.Areas.Admin.Models.Blogs;
using Hub.Core.Domain.Blogs;
using Hub.Data.Mapping;
using Hub.Services.Localization;
using Hub.Services.Seo;
using Hub.Web.Framework.Validators;

namespace Hub.Web.Areas.Admin.Validators.Blogs;

public partial class BlogPostValidator : BaseAppValidator<BlogPostModel>
{
   public BlogPostValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
   {
      RuleFor(x => x.Title)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Fields.Title.Required"));

      RuleFor(x => x.Body)
          .NotEmpty()
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Fields.Body.Required"));

      //blog tags should not contain dots
      //current implementation does not support it because it can be handled as file extension
      RuleFor(x => x.Tags)
          .Must(x => x == null || !x.Contains('.'))
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags.NoDots"));

      RuleFor(x => x.SeName).Length(0, AppSeoDefaults.SearchEngineNameLength)
          .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), AppSeoDefaults.SearchEngineNameLength);

      SetDatabaseValidationRules<BlogPost>(mappingEntityAccessor);
   }
}