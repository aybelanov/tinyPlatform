using Hub.Core.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Logging;

/// <summary>
/// Mapping class
/// </summary>
public class ActivityLogMap : AppEntityTypeConfiguration<ActivityLog>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<ActivityLog> builder)
   {
      builder.ToTable("ActivityLogs");
      builder.Property(p => p.Comment).HasMaxLength(int.MaxValue).IsRequired();
      builder.Property(p => p.IpAddress).HasMaxLength(200).IsRequired(false);
      builder.Property(p => p.EntityName).HasMaxLength(400).IsRequired(false);
      builder.Property(p => p.SubjectName).HasMaxLength(400).IsRequired(false);

      base.Configure(builder);
   }
}
