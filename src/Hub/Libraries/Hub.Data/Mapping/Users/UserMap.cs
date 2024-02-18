using Hub.Core.Domain.Users;
using Hub.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Users;

/// <summary>
/// Mapping class
/// </summary>
public class UserMap : AppEntityTypeConfiguration<User>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<User> builder)
   {
      builder.ToTable("Users");
      builder.Property(p => p.Username).HasMaxLength(1000).IsRequired(false);
      builder.Property(p => p.Email).HasMaxLength(1000).IsRequired(false);
      builder.Property(p => p.EmailToRevalidate).HasMaxLength(1000).IsRequired(false);
      builder.Property(p => p.SystemName).HasMaxLength(400).IsRequired(false);

      builder.HasSoftIndex(p => p.Email).IsUnique(false);

      base.Configure(builder);
   }
}