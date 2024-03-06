using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.JoinTables;

/// <summary>
/// Mapping class
/// </summary>
public class UserMonitorMap : AppEntityTypeConfiguration<UserMonitor>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<UserMonitor> builder)
   {
      builder.ToTable("UserMonitors");
      builder.Ignore(p => p.Id);
      builder.HasKey(p => new { p.UserId, p.MonitorId });

      builder.HasOne<User>().WithMany().HasForeignKey(b => b.UserId);
      builder.HasOne<Monitor>().WithMany().HasForeignKey(b => b.MonitorId);

      base.Configure(builder);
   }
}
