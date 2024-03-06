using Hub.Core.Domain.Forums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Forums;

/// <summary>
/// Mapping class
/// </summary>
public class ForumGroupMap : AppEntityTypeConfiguration<ForumGroup>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<ForumGroup> builder)
   {
      builder.ToTable("ForumGroups");
      builder.Property(p => p.Name).HasMaxLength(200).IsRequired();

      base.Configure(builder);
   }
}
