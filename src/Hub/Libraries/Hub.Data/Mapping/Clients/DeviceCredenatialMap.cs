﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Clients;

/// <summary>
/// Mapping class
/// </summary>
public class DeviceCredenatialMap : AppEntityTypeConfiguration<DeviceCredential>
{
    /// <summary>
    /// Configures a mapping
    /// </summary>
    /// <param name="builder">Entity type builder</param>
    public override void Configure(EntityTypeBuilder<DeviceCredential> builder)
    {
        builder.ToTable("DeviceCredentials");
        builder.Property(p => p.Password).HasMaxLength(200).IsRequired();
        builder.HasOne<Device>().WithMany().HasForeignKey(p => p.DeviceId);

        base.Configure(builder);
    }
}