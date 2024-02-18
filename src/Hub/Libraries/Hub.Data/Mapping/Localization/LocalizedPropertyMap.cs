using Hub.Core.Domain.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Localization;

/// <summary>
/// Mapping class
/// </summary>
public class LocalizedPropertyMap : AppEntityTypeConfiguration<LocalizedProperty>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<LocalizedProperty> builder)
   {
      builder.ToTable("LocalizedProperties");
      builder.Property(p => p.LocaleKeyGroup).HasMaxLength(400).IsRequired();
      builder.Property(p => p.LocaleKey).HasMaxLength(400).IsRequired();
      builder.Property(p => p.LocaleValue).HasMaxLength(int.MaxValue).IsRequired();

      builder.HasOne<Language>().WithMany().HasForeignKey(p => p.LanguageId);

      builder.HasIndex(p => new { p.LocaleKeyGroup, p.LanguageId });
      builder.HasIndex(p => new { p.EntityId, p.LocaleKey });

      base.Configure(builder);
   }
}
