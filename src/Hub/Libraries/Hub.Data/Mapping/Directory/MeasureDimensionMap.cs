using Hub.Core.Domain.Directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Directory;

/// <summary>
/// Mapping class
/// </summary>
public class MeasureDimensionMap : AppEntityTypeConfiguration<MeasureDimension>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<MeasureDimension> builder)
   {
      builder.ToTable("MeasureDimensions");
      builder.Property(p => p.Ratio).HasPrecision(12, 5);
      builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
      builder.Property(p => p.SystemKeyword).HasMaxLength(100).IsRequired();

      base.Configure(builder);
   }
}
