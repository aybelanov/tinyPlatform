using Hub.Core.Domain.ScheduleTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Hub.Data.Mapping.ScheduleTasks;

/// <summary>
/// Mapping class
/// </summary>
public class ScheduleTaskMap : AppEntityTypeConfiguration<ScheduleTask>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<ScheduleTask> builder)
   {
      builder.ToTable("ScheduleTasks");
      builder.Property(p => p.Name).HasMaxLength(int.MaxValue).IsRequired();
      builder.Property(p => p.Type).HasMaxLength(int.MaxValue).IsRequired();

      base.Configure(builder);
   }
}
