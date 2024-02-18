using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog post search model
/// </summary>
public partial record BlogPostSearchModel : BaseSearchModel
{
   #region Ctor

   public BlogPostSearchModel()
   {
   }

   #endregion

   #region Properties

   public string SearchTitle { get; set; }

   #endregion
}