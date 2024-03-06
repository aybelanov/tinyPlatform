using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Blogs;

/// <summary>
/// Mapping class
/// </summary>
public class BlogCommentMap : AppEntityTypeConfiguration<BlogComment>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<BlogComment> builder)
   {
      builder.ToTable("BlogComments");
      builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId);
      builder.HasOne<BlogPost>().WithMany().HasForeignKey(p => p.BlogPostId);

      base.Configure(builder);
   }
}
