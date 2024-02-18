using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Users;

/// <summary>
/// Mapping class
/// </summary>
public class UserAttributeValueMap : AppEntityTypeConfiguration<UserAttributeValue>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<UserAttributeValue> builder)
   {
      builder.ToTable("UserAttributeValues");
      builder.Property(p => p.Name).HasMaxLength(400).IsRequired();

      base.Configure(builder);
   }
}
