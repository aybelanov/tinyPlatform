using System.Threading.Tasks;
using Hub.Core.Domain.Messages;
using Hub.Services.Messages;
using Hub.Tests;
using NUnit.Framework;

namespace Hub.Services.Tests.Messages
{
   [TestFixture]
    public class EmailAccountServiceTests : BaseAppTest
    {
        private IEmailAccountService _emailAccountService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _emailAccountService = GetService<IEmailAccountService>();
        }

        [Test]
        public async Task TestCrud()
        {
            var insertItem = new EmailAccount
            {
                Email = "test@test.com",
                DisplayName = "Test name",
                Host = "smtp.test.com",
                Port = 25,
                Username = "test_user",
                Password = "test_password",
                EnableSsl = false,
                UseDefaultCredentials = false
            };

            var updateItem = new EmailAccount
            {
                Email = "test@test.com",
                DisplayName = "Test name",
                Host = "smtp.test.com",
                Port = 430,
                Username = "test_user",
                Password = "test_password",
                EnableSsl = true,
                UseDefaultCredentials = true
            };

            await TestCrud(insertItem, updateItem, (item, other) => item.UseDefaultCredentials.Equals(other.UseDefaultCredentials) && item.Port.Equals(other.Port) && item.EnableSsl.Equals(other.EnableSsl));
        }

    }
}
