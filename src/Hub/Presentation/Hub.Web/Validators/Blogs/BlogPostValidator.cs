﻿using FluentValidation;
using Hub.Services.Localization;
using Hub.Web.Framework.Validators;
using Hub.Web.Models.Blogs;

namespace Hub.Web.Validators.Blogs;

public partial class BlogPostValidator : BaseAppValidator<BlogPostModel>
{
   public BlogPostValidator(ILocalizationService localizationService)
   {
      RuleFor(x => x.AddNewComment.CommentText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Blog.Comments.CommentText.Required")).When(x => x.AddNewComment != null);
   }
}