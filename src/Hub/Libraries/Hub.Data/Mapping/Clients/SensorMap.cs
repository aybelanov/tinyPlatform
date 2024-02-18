using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.Clients;
using Hub.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Clients;


/// <summary>
/// Mapping class
/// </summary>
public class SensorMap : AppEntityTypeConfiguration<Sensor>
{
    /// <summary>
    /// Configures a mapping
    /// </summary>
    /// <param name="builder">Entity type builder</param>
    public override void Configure(EntityTypeBuilder<Sensor> builder)
    {
        builder.ToTable("Sensors");
        builder.Property(p => p.SystemName).HasMaxLength(100).IsRequired();
        builder.HasOne<Device>().WithMany().HasForeignKey(p => p.DeviceId);

        builder.HasSoftIndex(p => new { p.DeviceId, p.SystemName }).IsUnique();

        base.Configure(builder);
    }
}
