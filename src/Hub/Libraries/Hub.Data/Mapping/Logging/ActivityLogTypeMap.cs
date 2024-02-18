using Hub.Core.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Logging;

/// <summary>
/// Mapping class
/// </summary>
public class ActivityLogTypeMap : AppEntityTypeConfiguration<ActivityLogType>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<ActivityLogType> builder)
   {
      builder.ToTable("ActivityLogTypes");
      builder.Property(p => p.SystemKeyword).HasMaxLength(100).IsRequired();
      builder.Property(p => p.Name).HasMaxLength(200).IsRequired();

      base.Configure(builder);
   }
}
