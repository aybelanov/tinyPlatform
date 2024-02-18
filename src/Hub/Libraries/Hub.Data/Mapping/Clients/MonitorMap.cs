using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Clients;

/// <summary>
/// Mapping class
/// </summary>
public class MonitorMap : AppEntityTypeConfiguration<Monitor>
{
    /// <summary>
    /// Configures a mapping
    /// </summary>
    /// <param name="builder">Entity type builder</param>
    public override void Configure(EntityTypeBuilder<Monitor> builder)
    {
        builder.ToTable("Monitors");
        //builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        //builder.Property(p => p.Description).HasMaxLength(1000).IsRequired();
        //builder.Property(p => p.FullDescription).HasMaxLength(5000).IsRequired();

        //builder.HasMany(p => p.SensorWidgets).WithMany(p => p.Monitors).UsingEntity<MonitorSensorWidget>();
        //builder.HasMany(p => p.Panels).WithMany(p => p.Monitors).UsingEntity<MonitorPanel>();
        //builder.HasMany(p => p.Users).WithMany(p => p.Monitors).UsingEntity<UserMonitor>();
        builder.HasOne<User>().WithMany().HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.ClientCascade);

        base.Configure(builder);
    }
}