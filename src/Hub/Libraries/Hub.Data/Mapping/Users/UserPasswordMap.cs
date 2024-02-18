using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Users;

/// <summary>
/// Mapping class
/// </summary>
public class UserPasswordMap : AppEntityTypeConfiguration<UserPassword>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<UserPassword> builder)
   {
      builder.ToTable("UserPasswords");

      builder.HasOne<User>().WithMany().HasForeignKey(b => b.UserId);

      base.Configure(builder);
   }
}