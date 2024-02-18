using Hub.Core.Domain.Directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Directory;

/// <summary>
/// Mapping class
/// </summary>
public class StateProvinceMap : AppEntityTypeConfiguration<StateProvince>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<StateProvince> builder)
   {
      builder.ToTable("StateProvinces");
      builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
      builder.Property(p => p.Abbreviation).HasMaxLength(100).IsRequired(false);

      // has many-to-many via CurrencyStateProvinceMap

      base.Configure(builder);
   }
}