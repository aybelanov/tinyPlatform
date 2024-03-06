using FluentAssertions;
using Hub.Services.Localization;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Tests.Localization
{
   [TestFixture]
   public class LanguageServiceTests : ServiceTest
   {
      private ILanguageService _languageService;

      [OneTimeSetUp]
      public void SetUp()
      {
         _languageService = GetService<ILanguageService>();
      }

      [Test]
      public async Task CanGetAllLanguages()
      {
         var languages = await _languageService.GetAllLanguagesAsync();
         languages.Should().NotBeNull();
         languages.Any().Should().BeTrue();
      }
   }
}
