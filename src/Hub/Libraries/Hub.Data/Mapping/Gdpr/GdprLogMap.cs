﻿using Hub.Core.Domain.Gdpr;
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
