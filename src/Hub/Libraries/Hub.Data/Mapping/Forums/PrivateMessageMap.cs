using Hub.Core.Domain.Forums;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Forums;

/// <summary>
/// Mapping class
/// </summary>
public class PrivateMessageMap : AppEntityTypeConfiguration<PrivateMessage>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<PrivateMessage> builder)
   {
      builder.ToTable("PrivateMessages");
      builder.Property(p => p.Subject).HasMaxLength(450).IsRequired();
      builder.Property(p => p.Text).IsRequired();
      builder.HasOne<User>().WithMany().HasForeignKey(p => p.FromUserId).OnDelete(DeleteBehavior.Restrict);
      builder.HasOne<User>().WithMany().HasForeignKey(p => p.ToUserId).OnDelete(DeleteBehavior.Restrict);

      base.Configure(builder);
   }
}
