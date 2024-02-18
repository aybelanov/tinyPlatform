using Hub.Core.Domain.Messages;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Messages;

/// <summary>
/// Mapping class
/// </summary>
public class CampaignMap : AppEntityTypeConfiguration<Campaign>
{
   /// <summary>
   /// Apply entity configuration
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Campaign> builder)
   {
      builder.ToTable("Campaigns");
      builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
      builder.Property(p => p.Subject).HasMaxLength(int.MaxValue).IsRequired();
      builder.Property(p => p.Body).HasMaxLength(int.MaxValue).IsRequired();

      builder.HasOne<UserRole>().WithMany().HasForeignKey(p => p.UserRoleId);

      base.Configure(builder);
   }
}
