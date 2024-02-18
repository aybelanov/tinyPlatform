using Hub.Core.Domain.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Localization;

/// <summary>
/// Mapping class
/// </summary>
public class LanguageMap : AppEntityTypeConfiguration<Language>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Language> builder)
   {
      builder.ToTable("Languages");
      builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
      builder.Property(p => p.LanguageCulture).HasMaxLength(20).IsRequired();
      builder.Property(p => p.UniqueSeoCode).HasMaxLength(2).IsRequired(false);
      builder.Property(p => p.FlagImageFileName).HasMaxLength(50).IsRequired(false);

      base.Configure(builder);
   }
}
