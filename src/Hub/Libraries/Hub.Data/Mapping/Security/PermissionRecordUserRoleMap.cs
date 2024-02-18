using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Security;

/// <summary>
/// Mapping class
/// </summary>
public class PermissionRecordUserRoleMap : AppEntityTypeConfiguration<PermissionRecordUserRole>
{
   /// <summary>
   /// Apply entity configuration
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<PermissionRecordUserRole> builder)
   {
      builder.ToTable("PermissionRecordUserRoles");
      builder.Ignore(p => p.Id);
      builder.HasKey(x => new { x.UserRoleId, x.PermissionRecordId });

      builder.HasOne<PermissionRecord>().WithMany().HasForeignKey(x => x.PermissionRecordId);
      builder.HasOne<UserRole>().WithMany().HasForeignKey(x=> x.UserRoleId);  

      base.Configure(builder);
   }
}