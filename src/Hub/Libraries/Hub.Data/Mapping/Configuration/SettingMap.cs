using Hub.Core.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Configuration;

/// <summary>
/// Mapping class
/// </summary>
public class SettingMap : AppEntityTypeConfiguration<Setting>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Setting> builder)
   {
      builder.ToTable("Settings");
      builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
      builder.Property(p => p.Value).HasMaxLength(6000).IsRequired();

      base.Configure(builder);
   }
}