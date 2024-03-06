using Hub.Core.Domain.News;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.News;

/// <summary>
/// Mapping class
/// </summary>
public class NewsItemMap : AppEntityTypeConfiguration<NewsItem>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<NewsItem> builder)
   {
      builder.ToTable("NewsItems");

      base.Configure(builder);
   }
}
