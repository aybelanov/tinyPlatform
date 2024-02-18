using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Domain;
using Devices.Dispatcher.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Devices.Dispatcher.Data;

#pragma warning disable CS1591

public class AppDbContext : DbContext
{
   public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
   {

   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Sensor>(x => x.HasIndex(x => x.SystemName).IsUnique());
      modelBuilder.Entity<Setting>(entity => entity.HasData(CommonHelper.SettingsSeed(new DeviceSettings())));
   }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      base.OnConfiguring(optionsBuilder);
   }

   public DbSet<Sensor> Sensors { get; set; }
   public DbSet<SensoRecord> SensorRecords { get; set; }
   public DbSet<Setting> Settings { get; set; }
}
