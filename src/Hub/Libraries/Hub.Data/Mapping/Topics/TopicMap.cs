using Hub.Core.Domain.Topics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Topics;

/// <summary>
/// Mapping class
/// </summary>
public class TopicMap : AppEntityTypeConfiguration<Topic>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Topic> builder)
   {
      builder.ToTable("Topics");
      builder.HasOne<TopicTemplate>().WithMany().HasForeignKey(p => p.TopicTemplateId);

      base.Configure(builder);
   }
}
