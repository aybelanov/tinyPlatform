using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Users;

/// <summary>
/// Mapping class
/// </summary>
public class UserAttributeMap : AppEntityTypeConfiguration<UserAttribute>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<UserAttribute> builder)
   {
      builder.ToTable("UserAttributes");
      builder.Property(p => p.Name).HasMaxLength(400).IsRequired();
      builder.HasMany<UserAttributeValue>().WithOne().HasForeignKey(p => p.UserAttributeId);

      base.Configure(builder);
   }
}
