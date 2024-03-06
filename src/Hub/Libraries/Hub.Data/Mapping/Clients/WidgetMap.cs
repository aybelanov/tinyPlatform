using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Clients;

/// <summary>
/// Mapping class
/// </summary>
public class WidgetMap : AppEntityTypeConfiguration<Widget>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<Widget> builder)
   {
      builder.ToTable("Widgets");
      builder.Property(p => p.Adjustment).HasMaxLength(5000).IsRequired(false);

      builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId);

      base.Configure(builder);
   }
}