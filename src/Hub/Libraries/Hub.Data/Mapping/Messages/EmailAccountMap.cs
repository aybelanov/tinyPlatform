using Hub.Core.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Messages;

/// <summary>
/// Mapping class
/// </summary>
public class EmailAccountMap : AppEntityTypeConfiguration<EmailAccount>
{
   /// <summary>
   /// Apply entity configuration
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<EmailAccount> builder)
   {
      builder.ToTable("EmailAccounts");
      builder.Property(p => p.DisplayName).HasMaxLength(255).IsRequired(false);
      builder.Property(p => p.Email).HasMaxLength(255).IsRequired();
      builder.Property(p => p.Host).HasMaxLength(255).IsRequired();
      builder.Property(p => p.Username).HasMaxLength(255).IsRequired();
      builder.Property(p => p.Password).HasMaxLength(255).IsRequired();

      base.Configure(builder);
   }
}
