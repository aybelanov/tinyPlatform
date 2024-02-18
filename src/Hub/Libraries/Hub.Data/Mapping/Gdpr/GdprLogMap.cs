using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Gdpr;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Gdpr;

/// <summary>
/// Mapping class
/// </summary>
public class GdprLogMap : AppEntityTypeConfiguration<GdprLog>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<GdprLog> builder)
   {
      builder.ToTable("GdprLogs");

      base.Configure(builder);
   }
}
