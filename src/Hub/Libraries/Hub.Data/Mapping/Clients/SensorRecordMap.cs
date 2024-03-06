using Hub.Core.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Clients;

/// <summary>
/// Mapping class
/// </summary>
public class SensorRecordMap : AppEntityTypeConfiguration<SensorRecord>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<SensorRecord> builder)
   {
      builder.ToTable("SensorRecords");
      builder.Property(p => p.Metadata).HasMaxLength(200).IsRequired(false);
      builder.Property(p => p.EventTimestamp).IsRequired(true);
      builder.Property(p => p.Timestamp).IsRequired(true);

      builder.HasOne<Sensor>().WithMany().HasForeignKey(p => p.SensorId);

      base.Configure(builder);
   }
}