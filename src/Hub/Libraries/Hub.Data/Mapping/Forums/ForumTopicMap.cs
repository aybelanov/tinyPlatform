using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Forums;

/// <summary>
/// Mapping class
/// </summary>
public class ForumTopicMap : AppEntityTypeConfiguration<ForumTopic>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<ForumTopic> builder)
   {
      builder.ToTable("ForumTopics");
      builder.Property(p => p.Subject).IsRequired(false);
      builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
      builder.HasOne<Forum>().WithMany().HasForeignKey(p => p.ForumId);

      base.Configure(builder);
   }
}
