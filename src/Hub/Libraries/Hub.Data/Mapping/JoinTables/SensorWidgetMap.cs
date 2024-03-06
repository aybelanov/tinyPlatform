using Hub.Core.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.JoinTables;

/// <summary>
/// Mapping class
/// </summary>
public class SensorWidgetMap : AppEntityTypeConfiguration<SensorWidget>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<SensorWidget> builder)
   {
      builder.ToTable("SensorWidgets");
      builder.HasKey(p => p.Id);
      builder.HasIndex(p => new { p.WidgetId, p.SensorId }).IsUnique();

      builder.HasOne<Widget>().WithMany().HasForeignKey(p => p.WidgetId).OnDelete(DeleteBehavior.NoAction);
      builder.HasOne<Sensor>().WithMany().HasForeignKey(p => p.SensorId).OnDelete(DeleteBehavior.NoAction);

      base.Configure(builder);
   }
}