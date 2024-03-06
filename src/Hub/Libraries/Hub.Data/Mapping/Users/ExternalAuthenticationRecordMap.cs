using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Users;

/// <summary>
/// Mapping class
/// </summary>
public class ExternalAuthenticationRecordMap : AppEntityTypeConfiguration<ExternalAuthenticationRecord>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<ExternalAuthenticationRecord> builder)
   {
      builder.ToTable("ExternalAuthenticationRecord");
      builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId);

      base.Configure(builder);
   }
}
