using Hub.Core.Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Security;

/// <summary>
/// Mapping class
/// </summary>
public class PermissionRecordMap : AppEntityTypeConfiguration<PermissionRecord>
{
   /// <summary>
   /// Apply entity configuration
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<PermissionRecord> builder)
   {
      builder.ToTable("PermissionRecords");
      builder.Property(p => p.Name).HasMaxLength(int.MaxValue).IsRequired();
      builder.Property(p => p.SystemName).HasMaxLength(255).IsRequired();
      builder.Property(p => p.Category).HasMaxLength(255).IsRequired();

      //builder.HasMany(p => p.UserRoles).WithMany(p => p.PermissionRecords).UsingEntity<PermissionRecordUserRole>();

      base.Configure(builder);
   }
}
