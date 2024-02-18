using Hub.Core.Domain.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Media;

/// <summary>
/// Mapping class
/// </summary>
public class PictureBinaryMap : AppEntityTypeConfiguration<PictureBinary>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<PictureBinary> builder)
   {
      builder.ToTable("PictureBinaries");

      base.Configure(builder);
   }
}
