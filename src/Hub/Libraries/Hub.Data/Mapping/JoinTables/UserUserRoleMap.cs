using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.JoinTables;

/// <summary>
/// Mapping class
/// </summary>
public class UserUserRoleMap : AppEntityTypeConfiguration<UserUserRole>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<UserUserRole> builder)
   {
      builder.ToTable("UserUserRoles");
      builder.Ignore(p => p.Id);
      builder.HasKey(p => new { p.UserId, p.UserRoleId });

      builder.HasOne<User>().WithMany().HasForeignKey(b => b.UserId);
      builder.HasOne<UserRole>().WithMany().HasForeignKey(b => b.UserRoleId);

      base.Configure(builder);
   }
}
