using Hub.Core.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Messages;

/// <summary>
/// Mapping class
/// </summary>
public class QueuedEmailMap : AppEntityTypeConfiguration<QueuedEmail>
{
   /// <summary>
   /// Apply entity configuration
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<QueuedEmail> builder)
   {
      builder.ToTable("QueuedEmails");
      builder.Property(p => p.From).HasMaxLength(500).IsRequired();
      builder.Property(p => p.FromName).HasMaxLength(500).IsRequired(false);
      builder.Property(p => p.To).HasMaxLength(500).IsRequired(false);
      builder.Property(p => p.ToName).HasMaxLength(500).IsRequired(false);
      builder.Property(p => p.ReplyTo).HasMaxLength(500).IsRequired(false);
      builder.Property(p => p.ReplyToName).HasMaxLength(500).IsRequired(false);
      builder.Property(p => p.CC).HasMaxLength(500).IsRequired(false);
      builder.Property(p => p.Bcc).HasMaxLength(500).IsRequired(false);
      builder.Property(p => p.Subject).HasMaxLength(500).IsRequired(false);

      base.Configure(builder);
   }
}
