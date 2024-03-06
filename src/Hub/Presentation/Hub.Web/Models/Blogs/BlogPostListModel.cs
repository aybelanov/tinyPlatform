using Hub.Web.Framework.Models;
using System.Collections.Generic;

namespace Hub.Web.Models.Blogs;

public partial record BlogPostListModel : BaseAppModel
{
   public BlogPostListModel()
   {
      PagingFilteringContext = new BlogPagingFilteringModel();
      BlogPosts = new List<BlogPostModel>();
   }

   public long WorkingLanguageId { get; set; }
   public BlogPagingFilteringModel PagingFilteringContext { get; set; }
   public IList<BlogPostModel> BlogPosts { get; set; }
}