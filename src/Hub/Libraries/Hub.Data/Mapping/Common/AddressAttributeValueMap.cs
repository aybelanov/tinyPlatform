using Hub.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Common;

/// <summary>
/// Mapping class
/// </summary>
public class AddressAttributeValueMap : AppEntityTypeConfiguration<AddressAttributeValue>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<AddressAttributeValue> builder)
   {
      builder.ToTable("AddressAttributeValues");
      builder.Property(p => p.Name).HasMaxLength(400).IsRequired();

      base.Configure(builder);
   }
}
