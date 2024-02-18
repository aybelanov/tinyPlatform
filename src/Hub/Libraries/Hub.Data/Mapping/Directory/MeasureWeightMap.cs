using Hub.Core.Domain.Directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Directory;

/// <summary>
/// Mapping class
/// </summary>
public class MeasureWeightMap : AppEntityTypeConfiguration<MeasureWeight>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<MeasureWeight> builder)
   {
      builder.ToTable("MeasureWeights");
      builder.Property(p => p.Ratio).HasPrecision(12, 5);
      builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
      builder.Property(p => p.SystemKeyword).HasMaxLength(20).IsRequired();

      base.Configure(builder);
   }
}
