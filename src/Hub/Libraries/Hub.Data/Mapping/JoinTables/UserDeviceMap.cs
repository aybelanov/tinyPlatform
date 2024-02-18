using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.JoinTables;

/// <summary>
/// Mapping class
/// </summary>
public class UserDeviceMap : AppEntityTypeConfiguration<UserDevice>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<UserDevice> builder)
   {
      builder.ToTable("UserDevices");
      builder.Ignore(p => p.Id);
      builder.HasKey(p => new { p.UserId, p.DeviceId });

      builder.HasOne<User>().WithMany().HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.NoAction);
      builder.HasOne<Device>().WithMany().HasForeignKey(b => b.DeviceId).OnDelete(DeleteBehavior.NoAction);

      base.Configure(builder);
   }
}
