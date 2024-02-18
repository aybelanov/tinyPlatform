using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Polls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Polls;

/// <summary>
/// Mapping class
/// </summary>
public class PollMap : AppEntityTypeConfiguration<Poll>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Poll> builder)
   {
      builder.ToTable("Polls");
      builder.Property(p => p.Name).IsRequired();
      builder.HasOne<Language>().WithMany().HasForeignKey(p => p.LanguageId);

      base.Configure(builder);
   }
}
