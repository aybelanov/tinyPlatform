using FluentAssertions;
using Hub.Core.Domain.Messages;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Messages;
using Hub.Services.Users;
using Hub.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Tests.Messages
{
   [TestFixture]
   public class WorkflowMessageServiceTests : ServiceTest
   {
      private readonly IWorkflowMessageService _workflowMessageService;

      private readonly List<long> _notActiveTempletes = [];
      private readonly IMessageTemplateService _messageTemplateService;
      private User _user;
      private readonly IRepository<QueuedEmail> _queuedEmailRepository;
      private IList<MessageTemplate> _allMessageTemplates;
      private NewsLetterSubscription _subscription;

      public WorkflowMessageServiceTests()
      {
         _workflowMessageService = GetService<IWorkflowMessageService>();
         _messageTemplateService = GetService<IMessageTemplateService>();
         _queuedEmailRepository = GetService<IRepository<QueuedEmail>>();
      }

      [OneTimeSetUp]
      public async Task OneTimeSetUp()
      {
         var userService = GetService<IUserService>();
         _user = await userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail);
         _subscription = new NewsLetterSubscription { Active = true, Email = AppTestsDefaults.AdminEmail };

         _allMessageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync();

         foreach (var template in _allMessageTemplates.Where(t => !t.IsActive))
         {
            template.IsActive = true;
            _notActiveTempletes.Add(template.Id);
            await _messageTemplateService.UpdateMessageTemplateAsync(template);
         }
      }

      [OneTimeTearDown]
      public async Task OneTimeTearDown()
      {
         foreach (var template in _allMessageTemplates.Where(t => _notActiveTempletes.Contains(t.Id)))
         {
            template.IsActive = false;
            await _messageTemplateService.UpdateMessageTemplateAsync(template);
         }
      }

      [SetUp]
      public async Task SetUp()
      {
         await _queuedEmailRepository.TruncateAsync();
      }

      protected async Task CheckData(Func<Task<IList<long>>> func)
      {
         var queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
         queuedEmails.Count.Should().Be(0);

         var emailIds = await func();

         emailIds.Count.Should().BeGreaterThan(0);

         queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
         queuedEmails.Count.Should().Be(emailIds.Count);
      }

      #region User workflow

      [Test]
      public async Task CanSendUserRegisteredNotificationMessage()
      {
         await CheckData(async () =>
             await _workflowMessageService.SendUserRegisteredNotificationMessageAsync(_user, 1));
      }

      [Test]
      public async Task CanSendUserWelcomeMessage()
      {
         await CheckData(async () =>
             await _workflowMessageService.SendUserWelcomeMessageAsync(_user, 1));
      }

      [Test]
      public async Task CanSendUserEmailValidationMessage()
      {
         await CheckData(async () =>
             await _workflowMessageService.SendUserEmailValidationMessageAsync(_user, 1));
      }

      [Test]
      public async Task CanSendUserEmailRevalidationMessage()
      {
         _user.EmailToRevalidate = AppTestsDefaults.AdminEmail;
         await CheckData(async () =>
             await _workflowMessageService.SendUserEmailRevalidationMessageAsync(_user, 1));
      }

      [Test]
      public async Task CanSendUserPasswordRecoveryMessage()
      {
         await CheckData(async () =>
             await _workflowMessageService.SendUserPasswordRecoveryMessageAsync(_user, 1));
      }

      #endregion

      #region Newsletter workflow

      [Test]
      public async Task CanSendNewsLetterSubscriptionActivationMessage()
      {
         await CheckData(async () =>
             await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(_subscription, 1));
      }

      [Test]
      public async Task CanSendNewsLetterSubscriptionDeactivationMessage()
      {
         await CheckData(async () =>
             await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(_subscription, 1));
      }

      #endregion

      #region Misc

      [Test]
      public async Task CanSendNewVatSubmittedStoreOwnerNotification()
      {
         await CheckData(async () =>
             await _workflowMessageService.SendNewVatSubmittedApplicationOwnerNotificationAsync(_user, "vat name", "vat address", 1));
      }

      [Test]
      public async Task CanSendContactUsMessage()
      {
         await CheckData(async () =>
             await _workflowMessageService.SendContactUsMessageAsync(1, AppTestsDefaults.AdminEmail, "sender name", "subject", "body"));
      }

      
      #endregion
   }
}
