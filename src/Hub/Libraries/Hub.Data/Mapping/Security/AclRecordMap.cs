using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Security;

/// <summary>
/// Mapping class
/// </summary>
public class AclRecordMap : AppEntityTypeConfiguration<AclRecord>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<AclRecord> builder)
   {
      builder.ToTable("AclRecords");
      builder.Property(p => p.EntityName).HasMaxLength(400).IsRequired();
      builder.HasOne<UserRole>().WithMany().HasForeignKey(p => p.UserRoleId);

      base.Configure(builder);
   }
}
