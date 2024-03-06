using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Blogs;

/// <summary>
/// Mapping class
/// </summary>
public class BlogPostMap : AppEntityTypeConfiguration<BlogPost>
{
   #region Methods

   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<BlogPost> builder)
   {
      builder.ToTable("BlogPosts");
      builder.Property(p => p.Title).HasMaxLength(int.MaxValue).IsRequired();
      builder.Property(p => p.Body).IsRequired();
      builder.Property(p => p.MetaKeywords).HasMaxLength(400).IsRequired(false);
      builder.Property(p => p.MetaTitle).HasMaxLength(400).IsRequired(false);

      builder.HasOne<Language>().WithMany().HasForeignKey(x => x.LanguageId);

      base.Configure(builder);
   }

   #endregion
}