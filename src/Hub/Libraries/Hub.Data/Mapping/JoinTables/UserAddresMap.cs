using Hub.Core.Domain.Common;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.JoinTables;

/// <summary>
/// Mapping class
/// </summary>
public class UserAddresMap : AppEntityTypeConfiguration<UserAddress>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<UserAddress> builder)
   {
      builder.ToTable("UserAddresses");
      builder.Ignore(p => p.Id);
      builder.HasKey(p => new { p.UserId, p.AddressId });

      builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId);
      builder.HasOne<Address>().WithMany().HasForeignKey(p => p.AddressId);

      base.Configure(builder);
   }
}