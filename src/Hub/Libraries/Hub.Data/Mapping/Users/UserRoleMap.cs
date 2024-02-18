using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Users;

/// <summary>
/// Mapping class
/// </summary>
public class UserRoleMap : AppEntityTypeConfiguration<UserRole>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<UserRole> builder)
   {
      builder.ToTable("UserRoles");
      builder.Property(p => p.Name).HasMaxLength(255).IsRequired();
      builder.Property(p => p.SystemName).HasMaxLength(255).IsRequired(false);

      //builder.HasMany(p => p.AclRecords).WithOne(p => p.UserRole).HasForeignKey(p => p.UserRoleId);
      //builder.HasMany(p => p.Users).WithMany(x => x.UserRoles).UsingEntity<UserUserRole>();


      base.Configure(builder);
   }
}
