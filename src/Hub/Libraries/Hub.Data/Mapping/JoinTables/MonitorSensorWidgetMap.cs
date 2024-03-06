using Hub.Core.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.JoinTables;

/// <summary>
/// Mapping class
/// </summary>
public class MonitorSensorWidgetMap : AppEntityTypeConfiguration<Presentation>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Presentation> builder)
   {
      builder.ToTable("MonitorSensorWidgets");
      builder.HasKey(p => p.Id);
      builder.HasIndex(p => new { p.MonitorId, p.SensorWidgetId }).IsUnique();

      builder.HasOne<Monitor>().WithMany().HasForeignKey(p => p.MonitorId);
      builder.HasOne<SensorWidget>().WithMany().HasForeignKey(p => p.SensorWidgetId);

      base.Configure(builder);
   }
}