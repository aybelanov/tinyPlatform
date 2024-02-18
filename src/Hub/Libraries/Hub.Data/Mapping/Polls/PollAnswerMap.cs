using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.Polls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Polls;

/// <summary>
/// Mapping class
/// </summary>
public class PollAnswerMap : AppEntityTypeConfiguration<PollAnswer>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<PollAnswer> builder)
   {
      builder.ToTable("PollAnswers");
      builder.Property(p => p.Name).IsRequired();
      builder.HasOne<Poll>().WithMany().HasForeignKey(p=>p.PollId); 

      base.Configure(builder);
   }
}
