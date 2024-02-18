using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.Gdpr;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Gdpr;

/// <summary>
/// Mapping class
/// </summary>
public class GdprConsentMap : AppEntityTypeConfiguration<GdprConsent>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<GdprConsent> builder)
   {
      builder.ToTable("GdprConsents");
      builder.Property(p=>p.Message).IsRequired();

      base.Configure(builder);
   }
}
