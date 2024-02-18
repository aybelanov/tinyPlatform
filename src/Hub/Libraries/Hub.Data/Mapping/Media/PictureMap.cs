using Hub.Core.Domain.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Media;

/// <summary>
/// Mapping class
/// </summary>
public class PictureMap : AppEntityTypeConfiguration<Picture>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Picture> builder)
   {
      builder.ToTable("Pictures");
      builder.Property(p => p.MimeType).HasMaxLength(40).IsRequired();
      builder.Property(p => p.SeoFilename).HasMaxLength(300).IsRequired(false);
    
      builder.HasOne<PictureBinary>().WithOne().HasForeignKey<PictureBinary>(p => p.PictureId);

      base.Configure(builder);
   }
}
