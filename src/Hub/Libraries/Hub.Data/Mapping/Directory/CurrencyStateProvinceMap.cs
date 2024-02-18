using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.Directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Directory;

/// <summary>
/// Mapping class
/// </summary>
public class CurrencyStateProvinceMap : AppEntityTypeConfiguration<CurrencyStateProvince>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<CurrencyStateProvince> builder)
   {
      builder.ToTable("CurrencyStateProvinces");
      builder.Ignore(p => p.Id);
      builder.HasKey(p => new { p.CurrencyId, p.StateProvinceId });

      builder.HasOne<StateProvince>().WithMany().HasForeignKey(p => p.StateProvinceId);
      builder.HasOne<Currency>().WithMany().HasForeignKey(p => p.CurrencyId);  

      base.Configure(builder);
   }
}
