using Hub.Core.Domain.Polls;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Polls;

/// <summary>
/// Mapping class
/// </summary>
public class PollVotingRecordMap : AppEntityTypeConfiguration<PollVotingRecord>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<PollVotingRecord> builder)
   {
      builder.ToTable("PollVotingRecords");
      builder.HasOne<PollAnswer>().WithMany().HasForeignKey(p => p.PollAnswerId);
      builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId);

      base.Configure(builder);
   }
}
