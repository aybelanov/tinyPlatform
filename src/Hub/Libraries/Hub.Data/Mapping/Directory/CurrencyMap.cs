using Hub.Core.Domain.Directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Directory;

/// <summary>
/// Mapping class
/// </summary>
public class CurrencyMap : AppEntityTypeConfiguration<Currency>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Currency> builder)
   {
      builder.ToTable("Currencies");
      builder.Property(p => p.Rate).HasPrecision(12, 5);
      builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
      builder.Property(p => p.CurrencyCode).HasMaxLength(5).IsRequired();
      builder.Property(p => p.DisplayLocale).HasMaxLength(50).IsRequired(false);
      builder.Property(p => p.CustomFormatting).HasMaxLength(50).IsRequired(false);

      // has many-to-many via CurrencyStateProvinceMap

      base.Configure(builder);
   }
}
