using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Forums;

/// <summary>
/// Mapping class
/// </summary>
public class ForumPostMap : AppEntityTypeConfiguration<ForumPost>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<ForumPost> builder)
   {
      builder.ToTable("ForumPosts");
      builder.Property(p => p.Text).IsRequired();
      builder.Property(p => p.IPAddress).HasMaxLength(100).IsRequired(false);

      builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
      builder.HasOne<ForumTopic>().WithMany().HasForeignKey(p => p.ForumTopicId);

      base.Configure(builder);
   }
}
