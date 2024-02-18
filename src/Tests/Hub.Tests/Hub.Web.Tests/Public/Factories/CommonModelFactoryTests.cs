using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Hub.Core;
using Hub.Core.Domain;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Users;
using Hub.Tests;
using Hub.Web.Factories;
using Hub.Web.Models.Common;
using NUnit.Framework;

namespace Hub.Web.Tests.Public.Factories
{
   [TestFixture]
   public class CommonModelFactoryTests : BaseAppTest
   {
      private ICommonModelFactory _commonModelFactory;
      private LocalizationSettings _localizationSettings;
      private IWorkContext _workContext;
      private UserSettings _userSettings;
      private DisplayDefaultFooterItemSettings _displayDefaultFooterItemSettings;
      private CommonSettings _commonSettings;
      private AppInfoSettings _appInformationSettings;


      [OneTimeSetUp]
      public async Task SetUp()
      {
         _commonModelFactory = GetService<ICommonModelFactory>();
         _localizationSettings = GetService<LocalizationSettings>();
         _workContext = GetService<IWorkContext>();
         _userSettings = GetService<UserSettings>();
         _commonSettings = GetService<CommonSettings>();
         _displayDefaultFooterItemSettings = GetService<DisplayDefaultFooterItemSettings>();
         _appInformationSettings = GetService<AppInfoSettings>();

         await Task.CompletedTask;

      }

      [Test]
      public async Task CanPrepareLogoModel()
      {
         var model = await _commonModelFactory.PrepareLogoModelAsync();
         //model.AppName.Should().NotBeNullOrEmpty();
         //model.AppName.Should().Be("Your store name");
         model.LogoPath.Should().NotBeNullOrEmpty();
         model.LogoPath.Should()
             .Be($"http://{AppTestsDefaults.HostIpAddress}/Themes/DefaultTheme/Content/images/logo.svg");
      }

      [Test]
      public async Task CanPrepareLanguageSelectorModel()
      {
         var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

         model.CurrentLanguageId.Should().Be(1);
         model.UseImages.Should().Be(_localizationSettings.UseImagesForLanguageSelection);

         model.AvailableLanguages.Should().NotBeNullOrEmpty();
         var lang = model.AvailableLanguages.FirstOrDefault();
         lang.Should().NotBeNull();
         lang?.Name.Should().Be("EN");
         lang?.FlagImageFileName.Should().Be("us.png");
      }

      [Test]
      public async Task CanPrepareCurrencySelectorModel()
      {
         var model = await _commonModelFactory.PrepareCurrencySelectorModelAsync();
         model.CurrentCurrencyId.Should().Be(1);
         model.AvailableCurrencies.Should().NotBeNullOrEmpty();
         model.AvailableCurrencies.Count.Should().Be(1);
      }

      [Test]
      public async Task CanPrepareHeaderLinksModel()
      {
         var model = await _commonModelFactory.PrepareHeaderLinksModelAsync();

         model.RegistrationType.Should().Be(_userSettings.UserRegistrationType);
         model.IsAuthenticated.Should().BeTrue();
         model.UserName.Should().Be("John");
      }

      [Test]
      public async Task CanPrepareAdminHeaderLinksModel()
      {
         var model = await _commonModelFactory.PrepareAdminHeaderLinksModelAsync();
         model.ImpersonatedUserName.Should().Be("John");
         model.IsUserImpersonated.Should().BeFalse();
         model.DisplayAdminLink.Should().BeTrue();
         model.EditPageUrl.Should().BeNull();
      }

      [Test]
      public async Task CanPrepareSocialModel()
      {
         var model = await _commonModelFactory.PrepareSocialModelAsync();

         model.FacebookLink.Should().Be(_appInformationSettings.FacebookLink);
         model.TwitterLink.Should().Be(_appInformationSettings.TwitterLink);
         model.YoutubeLink.Should().Be(_appInformationSettings.YoutubeLink);
         model.WorkingLanguageId.Should().Be(1);
      }

      [Test]
      public async Task CanPrepareFooterModel()
      {
         var model = await _commonModelFactory.PrepareFooterModelAsync();

         //model.AppName.Should().Be("Your app name");
         model.SitemapEnabled.Should().BeTrue();
         model.WorkingLanguageId.Should().Be(1);
         model.DisplaySitemapFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplaySitemapFooterItem);
         model.DisplayContactUsFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayContactUsFooterItem);
         model.DisplayUserInfoFooterItem.Should()
             .Be(_displayDefaultFooterItemSettings.DisplayUserInfoFooterItem);
         model.DisplayUserAddressesFooterItem.Should()
             .Be(_displayDefaultFooterItemSettings.DisplayUserAddressesFooterItem);
      }

      [Test]
      public async Task CanPrepareContactUsModel()
      {
         var model = new ContactUsModel();
         model = await _commonModelFactory.PrepareContactUsModelAsync(model, true);

         model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
         model.DisplayCaptcha.Should().BeFalse();
         model.Email.Should().BeNullOrEmpty();
         model.FullName.Should().BeNullOrEmpty();

         model = await _commonModelFactory.PrepareContactUsModelAsync(model, false);
         model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
         model.DisplayCaptcha.Should().BeFalse();
         model.Email.Should().Be(AppTestsDefaults.AdminEmail);
         model.FullName.Should().Be("John Smith");
      }

      [Test]
      public void PrepareContactUsModelShouldRaiseExceptionIfModelIsNull()
      {
         Assert.Throws<AggregateException>(() =>
             _commonModelFactory.PrepareContactUsModelAsync(null, true).Wait());

         Assert.Throws<AggregateException>(() =>
             _commonModelFactory.PrepareContactUsModelAsync(null, false).Wait());
      }

      [Test]
      public async Task CanPrepareSitemapModel()
      {
         var model = await _commonModelFactory.PrepareSitemapModelAsync(new SitemapPageModel());
         model.Items.Should().NotBeNullOrEmpty();
         model.Items.Count.Should().Be(4);
      }

      [Test]
      public void PrepareSitemapModelShouldRaiseExceptionIfModelIsNull()
      {
         Assert.Throws<AggregateException>(() =>
             _commonModelFactory.PrepareSitemapModelAsync(null).Wait());
      }

      [Test]
      public async Task CanPrepareStoreThemeSelectorModel()
      {
         var model = await _commonModelFactory.PreparePlatformThemeSelectorModelAsync();
         model.CurrentHubTheme.Should().NotBeNull();
         model.CurrentHubTheme.Name.Should().Be("DefaultTheme");
         model.CurrentHubTheme.Title.Should().Be("Default clean");
         model.AvailableHubThemes.Should().NotBeNull();
         model.AvailableHubThemes.Count.Should().BeGreaterThan(0);
      }

      [Test]
      public async Task CanPrepareFaviconAndAppIconsModel()
      {
         var model = await _commonModelFactory.PrepareFaviconAndAppIconsModelAsync();
         model.HeadCode.Should().Be(_commonSettings.FaviconAndAppIconsHeadCode);
      }

      [Test]
      public async Task CanPrepareRobotsTextFile()
      {
         var model = await _commonModelFactory.PrepareRobotsTextFileAsync();
         model.Should().NotBeNullOrEmpty();
         model.Trim().Split("\r\n").Length.Should().Be(74);
      }
   }
}
