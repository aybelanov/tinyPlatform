using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Hub.Core.Domain.Messages;
using Hub.Data;
using Hub.Services.Messages;
using Hub.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Services.Tests.Messages
{
   [TestFixture]
   public class QueuedEmailServiceTests : BaseAppTest
   {
      private IQueuedEmailService _queuedEmailService;
      private IRepository<QueuedEmail> _queuedEmailRepository;
      private List<QueuedEmail> _emails;
      private string _testEmail;

      [OneTimeSetUp]
      public async Task OneTimeSetUp()
      {
         _queuedEmailService = GetService<IQueuedEmailService>();
         _queuedEmailRepository = GetService<IRepository<QueuedEmail>>();

         await _queuedEmailRepository.TruncateAsync();
         _testEmail = "test@test.com";
      }

      [TearDown]
      public async Task TearDown()
      {
         await _queuedEmailRepository.TruncateAsync();
      }

      [SetUp]
      public async Task SetUp()
      {
         _emails = new List<QueuedEmail>
            {
                new() {From = AppTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1, SentTries = 5},
                new() {From = AppTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1},
                new() {From = AppTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1},
                new() {From = AppTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1, SentOnUtc = DateTime.UtcNow},
                new() {From = AppTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1, SentOnUtc = DateTime.UtcNow}
            };

         foreach (var queuedEmail in _emails)
            await _queuedEmailService.InsertQueuedEmailAsync(queuedEmail);
      }

      [Test]
      public async Task CanCRUD()
      {
         using var scope = GetService<IServiceProvider>().CreateScope();
         var queuedEmailRepository = scope.ServiceProvider.GetRequiredService<IRepository<QueuedEmail>>();
         var queuedEmailService = scope.ServiceProvider.GetRequiredService<IQueuedEmailService>();

         var queuedEmails = await queuedEmailRepository.GetAllAsync(query => query);
         queuedEmails.Count.Should().Be(_emails.Count);

         var email = await queuedEmailService.GetQueuedEmailByIdAsync(_emails[0].Id);
         email.Body = "test";

         await queuedEmailService.UpdateQueuedEmailAsync(email);
         (await queuedEmailRepository.GetByIdAsync(_emails[0].Id)).Body.Should().Be(email.Body);

         await queuedEmailService.DeleteQueuedEmailAsync(email);

         queuedEmails = await queuedEmailRepository.GetAllAsync(query => query);
         queuedEmails.Count.Should().Be(_emails.Count - 1);

         //await queuedEmailService.DeleteQueuedEmailsAsync(_emails.Take(3).ToList());
         await queuedEmailService.DeleteQueuedEmailsAsync(queuedEmails.Take(2).ToList());

         queuedEmails = await queuedEmailRepository.GetAllAsync(query => query);
         queuedEmails.Count.Should().Be(_emails.Count - 3);

         await queuedEmailService.DeleteAllEmailsAsync();

         queuedEmails = await queuedEmailRepository.GetAllAsync(query => query);
         queuedEmails.Count.Should().Be(0);
      }


      [Test]
      public async Task CanGetQueuedEmailsByIds()
      {
         var queuedEmails =
             await _queuedEmailService.GetQueuedEmailsByIdsAsync(_emails.Take(3).Select(e => e.Id).ToArray());
         queuedEmails.Count.Should().Be(3);
      }

      [Test]
      public async Task CanSearchEmails()
      {
         var loadNotSentItemsOnly = true;
         var loadOnlyItemsToBeSent = true;
         var maxSendTries = int.MaxValue;
         var loadNewest = false;

         async Task<int> getCountAsync()
         {
            var emails = await _queuedEmailService.SearchEmailsAsync(AppTestsDefaults.AdminEmail, _testEmail, null,
                null, loadNotSentItemsOnly, loadOnlyItemsToBeSent, maxSendTries, loadNewest);

            return emails.Count;
         }

          (await getCountAsync()).Should().Be(3);
         loadNotSentItemsOnly = false;
         (await getCountAsync()).Should().Be(5);
         loadOnlyItemsToBeSent = false;
         (await getCountAsync()).Should().Be(5);
         loadNotSentItemsOnly = true;
         (await getCountAsync()).Should().Be(3);
         loadNotSentItemsOnly = false;
         maxSendTries = 1;
         (await getCountAsync()).Should().Be(4);
      }

      [Test]
      public async Task CanDeleteAlreadySentEmails()
      {
         await _queuedEmailService.DeleteAlreadySentEmailsAsync(null, null);
         var queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
         queuedEmails.Count.Should().Be(_emails.Count(e => !e.SentOnUtc.HasValue));
      }
   }
}
