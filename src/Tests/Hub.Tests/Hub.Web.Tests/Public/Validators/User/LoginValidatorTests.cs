using FluentValidation.TestHelper;
using Hub.Tests;
using Hub.Core.Domain.Users;
using Hub.Services.Localization;
using Hub.Web.Models.User;
using Hub.Web.Validators.User;
using NUnit.Framework;

namespace Hub.Web.Tests.Public.Validators.Users
{
    [TestFixture]
    public class LoginValidatorTests : BaseAppTest
    {
        private LoginValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<LoginValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
        {
            var model = new LoginModel
            {
                Email = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
            model.Email = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsWrongFormat()
        {
            var model = new LoginModel
            {
                Email = "adminexample.com"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var model = new LoginModel
            {
                Email = "admin@example.com"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsNullButUsernamesAreEnabled()
        {
            var userSettings = new UserSettings
            {
                UsernamesEnabled = true
            };
            _validator = new LoginValidator(GetService<ILocalizationService>(), userSettings);

            var model = new LoginModel
            {
                Email = null
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}
