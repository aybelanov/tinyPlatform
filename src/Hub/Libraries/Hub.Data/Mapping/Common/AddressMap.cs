using Hub.Core.Domain.Common;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Common;

/// <summary>
/// Mapping class
/// </summary>
public class AddressMap : AppEntityTypeConfiguration<Address>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Address> builder)
   {
      builder.ToTable("Addresses");
      builder.HasOne<StateProvince>().WithMany().HasForeignKey(x => x.StateProvinceId).OnDelete(DeleteBehavior.NoAction).IsRequired(false);

      // has many-to-many via UserAddress

      base.Configure(builder);
   }
}
