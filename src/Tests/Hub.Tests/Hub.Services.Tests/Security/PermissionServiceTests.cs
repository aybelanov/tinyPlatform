using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Hub.Core.Domain.Users;
using Hub.Core.Domain.Security;
using Hub.Data;
using Hub.Services.Security;
using Shared.Clients.Configuration;

namespace Hub.Services.Tests.Security
{
   [TestFixture]
    public class PermissionServiceTests : ServiceTest
    {
        private IPermissionService _permissionService;
        private IRepository<PermissionRecord> _permissionRecordRepository;

        [OneTimeSetUp]
        public void SetUp()
        {
            _permissionService = GetService<IPermissionService>();
            _permissionRecordRepository = GetService<IRepository<PermissionRecord>>();
        }

        [Test]
        public async Task TestCrud()
        {
            var insertItem = new PermissionRecord
            {
                Name = "Test name",
                SystemName = "Test system name",
                Category = "Test category"
            };

            var updateItem = new PermissionRecord
            {
                Name = "Test name 1",
                SystemName = "Test system name",
                Category = "Test category"
            };

            await TestCrud(insertItem, updateItem, (item, other) => item.Name.Equals(other.Name));
        }

        [Test]
        public async Task CanInstalUninstallPermissions()
        {
            async Task<IList<PermissionRecord>> getRecordsAsync()
            {
                return await _permissionRecordRepository.GetAllAsync(query =>
                    query.Where(p => p.SystemName.Equals("Test permission record system name")));
            }

            var records = await getRecordsAsync();

            records.Count.Should().Be(0);

            await _permissionService.InstallPermissionsAsync(new TestPermissionProvider());
            records = await getRecordsAsync();
            records.Count.Should().Be(1);
            await _permissionService.UninstallPermissionsAsync(new TestPermissionProvider());
            records = await getRecordsAsync();
            records.Count.Should().Be(0);
        }

        public class TestPermissionProvider : IPermissionProvider
        {
            private readonly PermissionRecord _permissionRecord = new()
            {
                Name = "Test name", SystemName = "Test permission record system name", Category = "Test category"
            };

            public IEnumerable<PermissionRecord> GetPermissions()
            {
                return [_permissionRecord];
            }

            public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
            {
                return [(UserDefaults.AdministratorsRoleName, new[] {_permissionRecord})];
            }
        }
    }
}
