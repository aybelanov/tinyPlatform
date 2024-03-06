using FluentAssertions;
using Hub.Core.Domain.Messages;
using Hub.Data;
using Hub.Services.Messages;
using Hub.Tests;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Tests.Messages
{
   [TestFixture]
   public class CampaignServiceTests : BaseAppTest
   {
      private ICampaignService _campaignService;
      private IRepository<Campaign> _campaignRepository;
      private IRepository<QueuedEmail> _cueuedEmailRepository;

      [OneTimeSetUp]
      public void SetUp()
      {
         _campaignService = GetService<ICampaignService>();
         _campaignRepository = GetService<IRepository<Campaign>>();
         _cueuedEmailRepository = GetService<IRepository<QueuedEmail>>();
      }

      [Test]
      public async Task TestCrud()
      {
         var insertItem = new Campaign
         {
            Name = "Test name",
            Subject = "Test subject",
            Body = "Test body",
            UserRoleId = 1
         };

         var updateItem = new Campaign
         {
            Name = "Test name",
            Subject = "Test subject",
            Body = "Test body",
            UserRoleId = 1
         };

         await TestCrud(insertItem, updateItem, (item, other) => item.Subject.Equals(other.Subject) && item.Body.Equals(other.Body) && item.Name.Equals(other.Name));
      }

      [Test]
      public async Task CanGetAllCampaigns()
      {
         var rez = await _campaignService.GetAllCampaignsAsync();
         rez.Count.Should().Be(0);
         await _campaignService.InsertCampaignAsync(new Campaign
         {
            Name = "Test name",
            Subject = "Test subject",
            Body = "Test body",
            UserRoleId = 1
         });
         await _campaignService.InsertCampaignAsync(new Campaign
         {
            Name = "Test name",
            Subject = "Test subject",
            Body = "Test body",
            UserRoleId = 1
         });
         await _campaignService.InsertCampaignAsync(new Campaign
         {
            Name = "Test name",
            Subject = "Test subject",
            Body = "Test body",
            UserRoleId = 1
         });

         rez = await _campaignService.GetAllCampaignsAsync();
         rez.Count.Should().Be(3);
         rez = await _campaignService.GetAllCampaignsAsync();
         rez.Count.Should().Be(3);
         rez = await _campaignService.GetAllCampaignsAsync();
         rez.Count.Should().Be(3);
      }

      [Test]
      public async Task CanSendCampaign()
      {
         await _campaignRepository.TruncateAsync();
         var campaign = new Campaign { Name = "Test name", Subject = "Test subject", Body = "Test body", UserRoleId = 1 };
         await _campaignService.InsertCampaignAsync(campaign);
         TestSmtpBuilder.TestSmtpClient.MessageIsSent = false;
         await _cueuedEmailRepository.TruncateAsync();

         var emailAccount = new EmailAccount
         {
            Id = 1,
            Email = AppTestsDefaults.AdminEmail,
            DisplayName = "Test name",
            Host = "smtp.test.com",
            Port = 25,
            Username = "test_user",
            Password = "test_password",
            EnableSsl = false,
            UseDefaultCredentials = false
         };

         var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com" };

         await _campaignService.SendCampaignAsync(campaign, emailAccount, new[] { subscription });
         _cueuedEmailRepository.Table.Count().Should().Be(1);

         await _campaignService.SendCampaignAsync(campaign, emailAccount, AppTestsDefaults.AdminEmail);
         TestSmtpBuilder.TestSmtpClient.MessageIsSent.Should().BeTrue();
      }
   }
}
