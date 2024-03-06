using Hub.Core.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Clients;


/// <summary>
/// Mapping class
/// </summary>
public class VideoSegmentMap : AppEntityTypeConfiguration<VideoSegment>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<VideoSegment> builder)
   {
      builder.ToTable("VideoSegments");
      builder.Property(p => p.InboundName).HasMaxLength(200).IsRequired();
      builder.HasOne<Sensor>().WithMany().HasForeignKey(p => p.IpcamId);
      builder.HasOne<VideoSegmentBinary>().WithOne().HasForeignKey<VideoSegmentBinary>(p => p.VideoSegmentId).OnDelete(DeleteBehavior.Cascade);

      base.Configure(builder);
   }
}
