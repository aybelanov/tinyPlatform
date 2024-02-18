using Hub.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Common;

/// <summary>
/// Mapping class
/// </summary>
public class SearchTermMap : AppEntityTypeConfiguration<SearchTerm>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<SearchTerm> builder)
   {
      builder.ToTable("SearchTerms");
      base.Configure(builder);
   }
}
