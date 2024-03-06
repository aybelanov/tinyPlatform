using Hub.Core.Domain.Forums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Forums;

/// <summary>
/// Mapping class
/// </summary>
public class ForumMap : AppEntityTypeConfiguration<Forum>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Forum> builder)
   {
      builder.ToTable("Forums");
      builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
      builder.HasOne<ForumGroup>().WithMany().HasForeignKey(p => p.ForumGroupId);

      base.Configure(builder);
   }
}
