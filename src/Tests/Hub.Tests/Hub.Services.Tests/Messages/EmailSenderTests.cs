using System.Threading.Tasks;
using FluentAssertions;
using Hub.Core.Domain.Messages;
using Hub.Services.Messages;
using Hub.Tests;
using NUnit.Framework;

namespace Hub.Services.Tests.Messages
{
   [TestFixture]
    public class EmailSenderTests : BaseAppTest
    {
        private IEmailSender _emailSender;

        [OneTimeSetUp]
        public void SetUp()
        {
            _emailSender = GetService<IEmailSender>();
        }

        [Test]
        public async Task CanSendEmail()
        {
            TestSmtpBuilder.TestSmtpClient.MessageIsSent = false;

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

            var subject = "Test subject";
            var body = "Test body";
            var fromAddress = AppTestsDefaults.AdminEmail;
            var fromName = "From name";
            var toAddress = "test@test.com";
            var toName = "To name";
            var replyToAddress = AppTestsDefaults.AdminEmail;
            var replyToName = "Reply to name";
            var bcc = new[] {AppTestsDefaults.AdminEmail};
            var cc = new[] { AppTestsDefaults.AdminEmail };

            await _emailSender.SendEmailAsync(emailAccount, subject, body,
                fromAddress, fromName, toAddress, toName,
                replyToAddress, replyToName, bcc, cc);

            TestSmtpBuilder.TestSmtpClient.MessageIsSent.Should().BeTrue();
        }
    }
}
