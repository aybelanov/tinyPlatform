using FluentAssertions;
using Hub.Core.Domain.Logging;
using Hub.Core.Domain.Users;
using Hub.Tests;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Data.Tests;

[TestFixture]
public class AppDataProviderTests : BaseAppTest
{
   [TearDown]
   public async Task TearDown()
   {
      try
      {
         using var scope = GetService<IServiceProvider>().CreateScope();
         var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();
         await dataProvider.TruncateAsync<Log>(true);
      }
      catch
      {
         //ignore 
      }
   }


   [Test]
   public async Task CanCreateTempDataStorage()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();
      var productRepository = scope.ServiceProvider.GetService<IRepository<User>>();

      var tableName = "TestTempDataTable".ToLower();

      await using var data = await dataProvider.CreateTempDataStorageAsync(tableName,
          productRepository.Table.Select(p => new { p.Username, p.Id, p.IsDeleted }));

      data.Count().Should().Be(productRepository.GetAll(query => query).Count);

      var rez = dataProvider.EntityFromSql<User>($"select * from {tableName}");

      rez.Count().Should().Be(data.Count());
   }

   [Test]
   public async Task CanInsertEntity()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      using var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();

      var logRecord = new Log { LogLevel = LogLevel.Information, FullMessage = "Test message 1", ShortMessage = "test short message 1" };
      await dataProvider.TruncateAsync<Log>();
      dataProvider.GetTable<Log>().Count().Should().Be(0);

      dataProvider.Add(logRecord);
      dataProvider.SaveChanges();

      dataProvider.GetTable<Log>().Count().Should().Be(1);
      await dataProvider.TruncateAsync<Log>();
      dataProvider.GetTable<Log>().Count().Should().Be(0);

      await dataProvider.AddAsync(logRecord);
      await dataProvider.SaveChangesAsync();

      dataProvider.GetTable<Log>().Count().Should().Be(1);
      await dataProvider.TruncateAsync<Log>();
   }

   [Test]
   public async Task CanUpdateEntity()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();

      var logRecord = new Log { LogLevel = LogLevel.Information, FullMessage = "Test message 1", ShortMessage = "test short message 1" };
      await dataProvider.TruncateAsync<Log>();
      dataProvider.GetTable<Log>().Count().Should().Be(0);

      dataProvider.Add(logRecord);
      dataProvider.SaveChanges();
      dataProvider.ChangeTracker.Clear();

      dataProvider.Update(new Log
      {
         Id = logRecord.Id,
         FullMessage = "Updated test log record full",
         ShortMessage = "Updated test log record short",
         LogLevel = LogLevel.Information,
      });
      dataProvider.SaveChanges();

      var updatedLog = dataProvider.GetTable<Log>().FirstOrDefault(tc => tc.Id == logRecord.Id);
      await dataProvider.TruncateAsync<Log>();

      logRecord.Id.Should().BeGreaterThan(0);
      updatedLog?.FullMessage.Should().NotBeEquivalentTo(logRecord.FullMessage);
   }

   [Test]
   public async Task CanUpdateEntities()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();

      var logRecord = new Log { LogLevel = LogLevel.Information, FullMessage = "Test message 1", ShortMessage = "test short message 1" };
      await dataProvider.TruncateAsync<Log>();
      dataProvider.GetTable<Log>().Count().Should().Be(0);
      dataProvider.Add(logRecord);
      dataProvider.SaveChanges();

      await dataProvider.BulkUpdateAsync(
         new[]
         {
             new Log
             {
                 Id = logRecord.Id,
                 FullMessage = "Updated test log record full",
                 ShortMessage = "Updated test log record short",
                 LogLevel = LogLevel.Information,
             }
         });

      dataProvider.SaveChanges();

      var updatedLog = dataProvider.GetTable<Log>().FirstOrDefault(tc => tc.Id == logRecord.Id);
      await dataProvider.TruncateAsync<Log>();

      logRecord.Id.Should().BeGreaterThan(0);
      updatedLog?.FullMessage.Should().NotBeEquivalentTo(logRecord.FullMessage);
   }

   [Test]
   public async Task CanDeleteEntity()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();

      var logRecord = new Log { LogLevel = LogLevel.Information, FullMessage = "Test message 1", ShortMessage = "test short message 1" };
      await dataProvider.TruncateAsync<Log>();
      dataProvider.GetTable<Log>().Count().Should().Be(0);
      dataProvider.Add(logRecord);
      dataProvider.SaveChanges();
      dataProvider.GetTable<Log>().Count().Should().Be(1);
      dataProvider.Remove(logRecord);
      dataProvider.SaveChanges();
      dataProvider.GetTable<Log>().Count().Should().Be(0);
   }

   [Test]
   public async Task CanBulkDeleteEntities()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();

      var logRecord = new Log { LogLevel = LogLevel.Information, FullMessage = "Test message 1", ShortMessage = "test short message 1" };
      await dataProvider.TruncateAsync<Log>();
      dataProvider.GetTable<Log>().Count().Should().Be(0);
      dataProvider.Add(logRecord);
      dataProvider.SaveChanges();

      dataProvider.GetTable<Log>().Count().Should().Be(1);
      await dataProvider.BulkDeleteAsync(new List<Log> { logRecord });
      dataProvider.GetTable<Log>().Count().Should().Be(0);
      dataProvider.Add(logRecord);
      dataProvider.SaveChanges();

      dataProvider.GetTable<Log>().Count().Should().Be(1);
      await dataProvider.BulkDeleteAsync<Log>(_ => true);
      dataProvider.GetTable<Log>().Count().Should().Be(0);
   }

   [Test]
   public async Task CanBulkInsertEntities()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();

      var logRecord = new Log { LogLevel = LogLevel.Information, FullMessage = "Test message 1", ShortMessage = "test short message 1" };

      await dataProvider.TruncateAsync<Log>();
      dataProvider.GetTable<Log>().Count().Should().Be(0);
      await dataProvider.BulkInsertAsync(new[] { logRecord });
      dataProvider.GetTable<Log>().Count().Should().Be(1);
      await dataProvider.TruncateAsync<Log>();
   }

   [Test]
   public async Task CanGetTable()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();

      var logRecord = new Log { LogLevel = LogLevel.Information, FullMessage = "Test message 1", ShortMessage = "test short message 1" };
      await dataProvider.TruncateAsync<Log>();
      dataProvider.GetTable<Log>().Count().Should().Be(0);
      dataProvider.Add(logRecord);
      dataProvider.SaveChanges();

      dataProvider.GetTable<Log>().Count().Should().Be(1);
      dataProvider.Remove(logRecord);
      dataProvider.SaveChanges();

      dataProvider.GetTable<Log>().Count().Should().Be(0);
   }


   [Test]
   public async Task CanExecuteNonQuery()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();

      //await dataProvider.ExecuteSqlCommandAsync("create table SearchTerm(Id INT, Keyword NVARCHAR, Count INT, CONSTRAINT \"PK_SearchTerm\" PRIMARY KEY (\"Id\"))");

      var rez = await dataProvider.ExecuteSqlCommandAsync("select * from Logs");
      rez.Should().Be(-1);

      rez = await dataProvider.ExecuteSqlCommandAsync($"insert into Logs(ShortMessage, FullMessage, LogLevel, LogLevelId, CreatedOnUtc) values('test task', 'ytu oiuoiu', 10, 10, {DateTime.Now.Ticks})");
      rez.Should().Be(1);

      rez = await dataProvider.ExecuteSqlCommandAsync("delete from Logs where Id=0");
      rez.Should().Be(0);

      var rez1 = await dataProvider.ExecuteSqlCommandAsync("select * from \"Logs\"");
      rez1.Should().Be(-1);

      rez1 = await dataProvider.ExecuteSqlCommandAsync($"insert into Logs(ShortMessage, FullMessage, LogLevel, LogLevelId, CreatedOnUtc) values('test task', 'ytu oiuoiu', 10, 10, {DateTime.Now.Ticks})");
      rez1.Should().Be(1);

      rez1 = await dataProvider.ExecuteSqlCommandAsync("delete from \"Logs\" where \"Id\"=0");
      rez1.Should().Be(0);

      await dataProvider.TruncateAsync<Log>();
   }

   [Test]
   public void CanQuery()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();

      var rez = dataProvider.EntityFromSql<User>("select * from Users");
      rez.Count().Should().BeGreaterThan(7);
      var rez1 = dataProvider.EntityFromSql<User>("select * from \"Users\"");
      rez1.Count().Should().BeGreaterThan(7);
   }

   [Test]
   public async Task CanTruncate()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dataProvider = scope.ServiceProvider.GetService<AppDbContext>();

      await dataProvider.TruncateAsync<Log>();
      dataProvider.Add(new Log { LogLevel = LogLevel.Information, FullMessage = "Test full message 1", ShortMessage = "Test short message 1" });
      dataProvider.SaveChanges();

      dataProvider.GetTable<Log>().Count().Should().Be(1);
      await dataProvider.TruncateAsync<Log>();
      dataProvider.GetTable<Log>().Count().Should().Be(0);
   }
}
