using System.Threading.Tasks;
using FluentAssertions;
using Hub.Core.Domain.Users;
using Hub.Tests;
using Hub.Web.Factories;
using NUnit.Framework;

namespace Hub.Web.Tests.Public.Factories
{
    [TestFixture]
    public class NewsletterModelFactoryTests : BaseAppTest
    {
        private INewsletterModelFactory _newsletterModelFactory;

        [OneTimeSetUp]
        public void SetUp()
        {
            _newsletterModelFactory = GetService<INewsletterModelFactory>();
        }

        [Test]
        public async Task CanPrepareNewsletterBoxModel()
        {
            var model = await _newsletterModelFactory.PrepareNewsletterBoxModelAsync();

            model.AllowToUnsubscribe.Should().Be(GetService<UserSettings>().NewsletterBlockAllowToUnsubscribe);
        }

        [Test]
        public async Task CanPrepareSubscriptionActivationModel()
        {
            var activated = (await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(true)).Result;

            var deactivated = (await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(false)).Result;

            activated.Should().NotBe(deactivated);
        }
    }
}
