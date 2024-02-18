using Hub.Core.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Messages;

/// <summary>
/// Mapping class
/// </summary>
public class NewsLetterSubscriptionMap : AppEntityTypeConfiguration<NewsLetterSubscription>
{
   /// <summary>
   /// Apply entity configuration
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<NewsLetterSubscription> builder)
   {
      builder.ToTable("NewsLetterSubscriptions");
      builder.Property(p => p.Email).HasMaxLength(255).IsRequired();

      base.Configure(builder);
   }
}
