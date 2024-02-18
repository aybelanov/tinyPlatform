using Hub.Core.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Messages;

/// <summary>
/// Mapping class
/// </summary>
public class MessageTemplateMap : AppEntityTypeConfiguration<MessageTemplate>
{
   /// <summary>
   /// Apply entity configuration
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<MessageTemplate> builder)
   {
      builder.ToTable("MessageTemplates");
      builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
      builder.Property(p => p.BccEmailAddresses).HasMaxLength(200).IsRequired(false);
      builder.Property(p => p.Subject).HasMaxLength(1000).IsRequired(false);
      //don't create an ForeignKey for the EmailAccount table, because this field may by zero

      base.Configure(builder);
   }
}
