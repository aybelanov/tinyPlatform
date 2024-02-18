using Hub.Core.Domain.Seo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Seo;

/// <summary>
/// Mapping class
/// </summary>
public class UrlRecordMap : AppEntityTypeConfiguration<UrlRecord>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<UrlRecord> builder)
   {
      builder.ToTable("UrlRecords");
      builder.Property(p => p.EntityName).HasMaxLength(400).IsRequired();
      builder.Property(p => p.Slug).HasMaxLength(400).IsRequired();

      base.Configure(builder);
   }
}
