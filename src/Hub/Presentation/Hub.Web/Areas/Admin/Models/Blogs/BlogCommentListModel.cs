using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog comment list model
/// </summary>
public partial record BlogCommentListModel : BasePagedListModel<BlogCommentModel>
{
}