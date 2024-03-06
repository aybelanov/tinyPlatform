using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Clients;

/// <summary>
/// Mapping class
/// </summary>
public class DeviceMap : AppEntityTypeConfiguration<Device>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Device> builder)
   {
      builder.ToTable("Devices");
      builder.Property(p => p.Guid).IsRequired();
      builder.Property(p => p.SystemName).HasMaxLength(200).IsRequired();
      builder.HasOne<User>().WithMany().HasForeignKey(p => p.OwnerId);
      builder.HasSoftIndex(p => p.SystemName).IsUnique();

      base.Configure(builder);
   }
}
