using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Forums;

/// <summary>
/// Mapping class
/// </summary>
public class ForumPostVoteMap : AppEntityTypeConfiguration<ForumPostVote>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<ForumPostVote> builder)
   {
      builder.ToTable("ForumPostVotes");
      builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);

      base.Configure(builder);
   }
}
