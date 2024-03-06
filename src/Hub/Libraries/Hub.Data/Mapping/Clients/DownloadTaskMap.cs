using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Clients;

/// <summary>
/// Mapping class
/// </summary>
public class DownloadTaskMap : AppEntityTypeConfiguration<DownloadTask>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<DownloadTask> builder)
   {
      builder.ToTable("DownloadTracker");
      builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId);

      base.Configure(builder);
   }
}
