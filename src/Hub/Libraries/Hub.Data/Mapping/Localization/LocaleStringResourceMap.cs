using Hub.Core.Domain.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Localization;

/// <summary>
/// Mapping class
/// </summary>
public class LocaleStringResourceMap : AppEntityTypeConfiguration<LocaleStringResource>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<LocaleStringResource> builder)
   {
      builder.ToTable("LocaleStringResources");
      builder.Property(p => p.ResourceName).HasMaxLength(200).IsRequired();
      builder.Property(p => p.ResourceValue).HasMaxLength(int.MaxValue).IsRequired();

      builder.HasOne<Language>().WithMany().HasForeignKey(p => p.LanguageId);

      base.Configure(builder);
   }
}
