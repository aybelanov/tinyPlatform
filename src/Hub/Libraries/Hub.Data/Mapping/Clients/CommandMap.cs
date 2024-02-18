using Hub.Core.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Clients;

/// <summary>
/// Mapping class
/// </summary>
public class CommandMap : AppEntityTypeConfiguration<Command>
{
    /// <summary>
    /// Configures a mapping
    /// </summary>
    /// <param name="builder">Entity type builder</param>
    public override void Configure(EntityTypeBuilder<Command> builder)
    {
        builder.ToTable("DeviceCommands");
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Arguments).HasMaxLength(4000).IsRequired(false);

        builder.HasOne<Device>().WithMany().HasForeignKey(p => p.DeviceId);

        base.Configure(builder);
    }
}
