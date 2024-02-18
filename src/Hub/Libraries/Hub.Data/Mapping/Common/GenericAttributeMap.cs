using Hub.Core.Domain.Common;
using Hub.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.Common;

/// <summary>
/// Mapping class
/// </summary>
public class GenericAttributeMap : AppEntityTypeConfiguration<GenericAttribute>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<GenericAttribute> builder)
   {
      builder.ToTable("GenericAttributes");
      builder.Property(p => p.KeyGroup).HasMaxLength(400).IsRequired();
      builder.Property(p => p.Key).HasMaxLength(400).IsRequired();
      builder.Property(p => p.Value).HasMaxLength(int.MaxValue).IsRequired();

      builder.HasSoftIndex(x => new { x.EntityId, x.KeyGroup });
      //builder.HasSoftIndex(x => new { x.EntityId, x.KeyGroup, x.Key }).IsUnique();

      base.Configure(builder);
   }
}
