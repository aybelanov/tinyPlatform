using Hub.Core.Domain.Directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Directory;

/// <summary>
/// Mapping class
/// </summary>
public class CountryMap : AppEntityTypeConfiguration<Country>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Country> builder)
   {
      builder.ToTable("Countries");
      builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
      builder.Property(p => p.TwoLetterIsoCode).HasMaxLength(2).IsRequired(false);
      builder.Property(p => p.ThreeLetterIsoCode).HasMaxLength(3).IsRequired(false);

      base.Configure(builder);
   }
}
