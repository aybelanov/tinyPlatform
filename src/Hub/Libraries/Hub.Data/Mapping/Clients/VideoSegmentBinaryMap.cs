using Hub.Core.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Clients;


/// <summary>
/// Mapping class
/// </summary>
public class VideoSegmentBinaryMap : AppEntityTypeConfiguration<VideoSegmentBinary>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<VideoSegmentBinary> builder)
   {
      builder.ToTable("VideoSegmentBinaries");

      base.Configure(builder);
   }
}
