using Hub.Core.Domain.Topics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Topics;

/// <summary>
/// Mapping class
/// </summary>
public class TopicTemplateMap : AppEntityTypeConfiguration<TopicTemplate>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<TopicTemplate> builder)
   {
      builder.ToTable("TopicTemplates");
      builder.Property(p => p.Name).HasMaxLength(400).IsRequired();
      builder.Property(p => p.ViewPath).HasMaxLength(400).IsRequired();

      base.Configure(builder);
   }
}
