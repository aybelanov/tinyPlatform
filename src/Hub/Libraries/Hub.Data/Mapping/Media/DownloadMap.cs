using Hub.Core.Domain.Media;
using Hub.Data.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Media;

/// <summary>
/// Mapping class
/// </summary>
public class DownloadMap : AppEntityTypeConfiguration<Download>
{
   /// <summary>
   /// Apply entity configuration
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Download> builder)
   {
      builder.ToTable("Downloads");

      base.Configure(builder);
   }
}
