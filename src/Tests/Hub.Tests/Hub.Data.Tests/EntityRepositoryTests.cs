using FluentAssertions;
using Hub.Core.Caching;
using Hub.Core.Domain.Logging;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hub.Data.Tests;

[TestFixture]
public class EntityRepositoryTests : BaseAppTest
{
   private IStaticCacheManager _cacheManager;
   private CacheKey _cacheKey;

   [OneTimeSetUp]
   public void OneTimeSetUp()
   {
      _cacheManager = GetService<IStaticCacheManager>();
      _cacheKey = new CacheKey("EntityRepositoryTestsCacheKey");
   }

   //[SetUp]
   //public async Task SetUp()
   //{
   //   using var scope = GetService<IServiceProvider>().CreateScope();
   //   var logRecordRepository = scope.ServiceProvider.GetService<IRepository<Log>>();
   //   await logRecordRepository.TruncateAsync(true);
   //}

   [TearDown]
   public async Task TearDown()
   {
      try
      {
         //var logRecordRepository = GetService<IRepository<Log>>();
         var userRepository = GetService<IRepository<User>>();
         var user = userRepository.Table.IgnoreQueryFilters().First(x => x.Id == 2);
         user.IsDeleted = false;
         await userRepository.UpdateAsync(user);
         using var scope = GetService<IServiceProvider>().CreateScope();
         var logRecordRepository = scope.ServiceProvider.GetService<IRepository<Log>>();
         await logRecordRepository.TruncateAsync(true);
         // logRecordRepository.TruncateAsync(true);
         await _cacheManager.ClearAsync();
      }
      catch
      {
         //ignore 
      }
   }

   [Test]
   public async Task CanGetById()
   {
      var userRepository = GetService<IRepository<User>>();

      var user = await userRepository.GetByIdAsync(1);
      user.Should().NotBeNull();

      user = await userRepository.GetByIdAsync(2);
      user.IsDeleted = true;
      await userRepository.UpdateAsync(user);

      user = userRepository.Table.IgnoreQueryFilters().FirstOrDefault(x => x.Id == 2);
      user.Should().NotBeNull();
      user = await userRepository.GetByIdAsync(2);
      user.Should().BeNull();

      user = await _cacheManager.GetAsync(_cacheKey, () => default(User));
      user.Should().BeNull();

      await userRepository.GetByIdAsync(1, _ => _cacheKey);
      user = await _cacheManager.GetAsync(_cacheKey, () => default(User));
      user.Should().NotBeNull();
   }

   [Test]
   public async Task CanGetByIds()
   {
      var userRepository = GetService<IRepository<User>>();

      var user = await userRepository.GetByIdAsync(2);
      user.IsDeleted = true;
      await userRepository.UpdateAsync(user);

      var ids = new List<long> { 1, 2, 3 };

      IList<User> users = await userRepository.Table.IgnoreQueryFilters().Where(x => ids.Contains(x.Id)).ToListAsync();
      users.Count.Should().Be(3);

      users = await userRepository.GetByIdsAsync(ids);
      users.Count.Should().Be(2);

      users = await _cacheManager.GetAsync(_cacheKey, () => default(IList<User>));
      users.Should().BeNull();

      await userRepository.GetByIdsAsync(ids, _ => _cacheKey);
      users = await _cacheManager.GetAsync(_cacheKey, () => default(IList<User>));
      users.Count.Should().Be(2);
   }

   [Test]
   public async Task CanGetAll()
   {
      var userRepository = GetService<IRepository<User>>();

      var user = await userRepository.GetByIdAsync(2);
      user.IsDeleted = true;
      await userRepository.UpdateAsync(user);

      var asyncRez = await userRepository.GetAllAsync(query => query, null);
      asyncRez.Count.Should().BeGreaterThan(0);

      var taskRez = await userRepository.GetAllAsync(Task.FromResult);
      taskRez.Count.Should().BeGreaterThan(0);

      var rez = userRepository.GetAll(query => query, manager => _cacheKey);
      rez.Count.Should().BeGreaterThan(0);

      rez = userRepository.GetAll(query => query);
      rez.Count.Should().BeGreaterThan(0);

      asyncRez.Count.Should().Be(rez.Count);
      rez.Count.Should().Be(taskRez.Count);

      rez = await userRepository.GetAllAsync(Task.FromResult, manager => Task.FromResult(_cacheKey));
      rez.Count.Should().BeGreaterThan(0);
      var fullCount = rez.Count;

      await _cacheManager.RemoveAsync(_cacheKey);

      rez = await userRepository.GetAllAsync(Task.FromResult, manager => Task.FromResult(default(CacheKey)));
      rez.Count.Should().BeGreaterThan(0);
      rez.Count.Should().BeLessThanOrEqualTo(fullCount);
   }

   [Test]
   public async Task CanGetAllPaged()
   {
      var userRepository = GetService<IRepository<User>>();

      var user = await userRepository.GetByIdAsync(2);
      user.IsDeleted = true;
      await userRepository.UpdateAsync(user);

      var rez = await userRepository.GetAllPagedAsync(query => query);
      rez.Count.Should().BeGreaterThan(0);
      var fullCount = rez.Count;

      rez = await userRepository.GetAllPagedAsync(query => query);
      rez.Count.Should().BeGreaterThan(0);
      rez.Count.Should().BeLessThanOrEqualTo(fullCount);
   }

   [Test]
   public async Task CanInsert()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var logRecordRepository = scope.ServiceProvider.GetService<IRepository<Log>>();
      var logRecord = new Log { LogLevel = LogLevel.Information, FullMessage = "test log record", ShortMessage = "test log record short" };
      logRecord.Id.Should().Be(0);

      await logRecordRepository.InsertAsync(logRecord);
      await logRecordRepository.DeleteAsync(logRecord);

      logRecord.Id.Should().BeGreaterThan(0);

      Assert.Throws<AggregateException>(() => logRecordRepository.InsertAsync(default(Log)).Wait());
   }

   [Test]
   public async Task CanUpdate()
   {
      var logRecord = new Log();

      using (var scope = GetService<IServiceProvider>().CreateScope())
      {
         var logRecordRepository = scope.ServiceProvider.GetRequiredService<IRepository<Log>>();
         logRecord.Id.Should().Be(0);
      }

      using (var scope = GetService<IServiceProvider>().CreateScope())
      {
         logRecord = new Log { LogLevel = LogLevel.Information, FullMessage = "test log record", ShortMessage = "test log record short" };
         var logRecordRepository = scope.ServiceProvider.GetRequiredService<IRepository<Log>>();
         await logRecordRepository.InsertAsync(logRecord);
      }

      using (var scope = GetService<IServiceProvider>().CreateScope())
      {
         var logRecordRepository = scope.ServiceProvider.GetRequiredService<IRepository<Log>>();
         await logRecordRepository.UpdateAsync(new Log
         {
            Id = logRecord.Id,
            FullMessage = "Updated test tax category",
            ShortMessage = "test log record short",
            LogLevelId = 20
         });
      }

      var updatedLog = new Log();
      using (var scope = GetService<IServiceProvider>().CreateScope())
      {
         var logRecordRepository = scope.ServiceProvider.GetRequiredService<IRepository<Log>>();
         updatedLog = await logRecordRepository.GetByIdAsync(logRecord.Id);
      }

      using (var scope = GetService<IServiceProvider>().CreateScope())
      {
         var logRecordRepository = scope.ServiceProvider.GetRequiredService<IRepository<Log>>();
         await logRecordRepository.DeleteAsync(logRecord);
         logRecord.Id.Should().BeGreaterThan(0);
         updatedLog.FullMessage.Should().NotBeEquivalentTo(logRecord.FullMessage);
      }
   }


   [Test]
   public async Task CanDelete()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var logRecordRepository = scope.ServiceProvider.GetRequiredService<IRepository<Log>>();
      var logRecord = new Log { LogLevel = LogLevel.Information, FullMessage = "test log record full", ShortMessage = "test log record short" };
      await logRecordRepository.InsertAsync(logRecord);
      await logRecordRepository.DeleteAsync(logRecord);
      logRecord.Id.Should().BeGreaterThan(0);
      logRecord = await logRecordRepository.GetByIdAsync(logRecord.Id);
      logRecord.Should().BeNull();

      Assert.Throws<AggregateException>(() => logRecordRepository.DeleteAsync(default(Log)).Wait());
      Assert.Throws<AggregateException>(() => logRecordRepository.DeleteAsync(default(IList<Log>)).Wait());
      Assert.Throws<AggregateException>(() => logRecordRepository.DeleteAsync((Expression<Func<Log, bool>>)null).Wait());
   }

   [Test]
   public async Task CanSoftDelete()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
      var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
      var user = new User() { Email = "softdelete@user.com" };
      await userRepository.InsertAsync(user);
      await userRepository.DeleteAsync(user);
      var softDeletedUser = await dbContext.GetTable<User>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Email == "softdelete@user.com");
      softDeletedUser.Should().NotBeNull();  
   }

   [Test]
   public async Task CanSoftDeleteMany()
   {
      using var scope = GetService<IServiceProvider>().CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
      var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
      var user = new User() { Email = "softdelete@user.com" };
      await userRepository.InsertAsync(user);
      await userRepository.DeleteAsync(new List<User>() { user });
      var softDeletedUser = await dbContext.GetTable<User>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Email == "softdelete@user.com");
      softDeletedUser.Should().NotBeNull();
   }


   [Test]
   public async Task CanLoadOriginalCopy()
   {
      var userRepository = GetService<IRepository<User>>();
      var user = await userRepository.GetByIdAsync(1, _ => default);
      var userName = user.Username;
      user.Username = "test name";
      var userNew = await userRepository.GetByIdAsync(1, _ => default);
      var userOld = await userRepository.LoadOriginalCopyAsync(user);

      userOld.Username.Should().NotBeEquivalentTo(userNew.Username);
      user.Username = userName;
   }

   [Test]
   public async Task CanTruncate()
   {
      using (var scope = GetService<IServiceScopeFactory>().CreateScope())
      {
         var logRecordRepository = scope.ServiceProvider.GetService<IRepository<Log>>();

         await logRecordRepository.InsertAsync(new List<Log>
         {
             new() {FullMessage = "Test message 1", ShortMessage = "test log record short", LogLevel = LogLevel.Information},
             new() {FullMessage = "Test message 2", ShortMessage = "test log record short", LogLevel = LogLevel.Information},
             new() {FullMessage = "Test message 3", ShortMessage = "test log record short", LogLevel = LogLevel.Information},
             new() {FullMessage = "Test message 4", ShortMessage = "test log record short", LogLevel = LogLevel.Information},
             new() {FullMessage = "Test message 5", ShortMessage = "test log record short", LogLevel = LogLevel.Information}
         });

         var rezWithContent = await logRecordRepository.GetAllAsync(query => query);
         await logRecordRepository.TruncateAsync();
         var rezWithoutContent = await logRecordRepository.GetAllAsync(query => query);

         rezWithContent.Count.Should().Be(5);
         rezWithoutContent.Count.Should().Be(0);
      }

      using (var scope = GetService<IServiceScopeFactory>().CreateScope())
      {
         var logRecordRepository = scope.ServiceProvider.GetService<IRepository<Log>>();

         var logRecord1 = new Log { FullMessage = "Test message 1", ShortMessage = "test log record short", LogLevel = LogLevel.Information };

         await logRecordRepository.InsertAsync(logRecord1);
         await logRecordRepository.TruncateAsync(true);
         logRecord1.Id.Should().NotBe(1);
      }

      using (var scope = GetService<IServiceScopeFactory>().CreateScope())
      {
         var logRecordRepository = scope.ServiceProvider.GetService<IRepository<Log>>();

         var logRecord2 = new Log { FullMessage = "Test message 2", ShortMessage = "test log record short", LogLevel = LogLevel.Information };
         await logRecordRepository.InsertAsync(logRecord2);
         await logRecordRepository.TruncateAsync(true);

         logRecord2.Id.Should().Be(1);
      }
   }
}
