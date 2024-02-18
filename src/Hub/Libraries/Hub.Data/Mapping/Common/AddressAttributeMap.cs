using Hub.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Common;

/// <summary>
/// Mapping class
/// </summary>
public class AddressAttributeMap : AppEntityTypeConfiguration<AddressAttribute>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<AddressAttribute> builder)
   {
      builder.ToTable("AddressAttributes");
      builder.Property(p => p.Name).HasMaxLength(400).IsRequired();

      base.Configure(builder);
   }
}
