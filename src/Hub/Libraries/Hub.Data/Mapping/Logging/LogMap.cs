using Hub.Core.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Logging;

/// <summary>
/// Mapping class
/// </summary>
public class LogMap : AppEntityTypeConfiguration<Log>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Log> builder)
   {
      builder.ToTable("Logs");
      builder.Property(p => p.IpAddress).HasMaxLength(200).IsRequired(false);
      builder.Property(p => p.FullMessage).HasMaxLength(int.MaxValue).IsRequired();
      //builder.HasIndex(p => p.IpAddress);
      builder.HasIndex(p => p.LogLevelId);


      base.Configure(builder);
   }
}
