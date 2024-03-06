using FluentAssertions;
using Hub.Core.Domain.Users;
using Hub.Web.Framework.Validators;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Hub.Web.Tests.Public.Validators
{
   [TestFixture]
   public class UsernameValidatorTests
   {
      private TestValidator _validator;
      private UserSettings _userSettings;

      [OneTimeSetUp]
      public void Setup()
      {
         _userSettings = new UserSettings
         {
            UsernameValidationRule = "^a.*1$",
            UsernameValidationEnabled = true,
            UsernameValidationUseRegex = false
         };

         _validator = new TestValidator { v => v.RuleFor(x => x.Username).IsUsername(_userSettings) };
      }

      [Test]
      public async Task IsValidTests()
      {
         //optional value is not valid
         var result = await _validator.ValidateAsync(new Person { Username = null });
         result.IsValid.Should().BeFalse();
         result = await _validator.ValidateAsync(new Person { Username = string.Empty });
         result.IsValid.Should().BeFalse();

         //validation without regex
         result = await _validator.ValidateAsync(new Person { Username = "test_user" });
         result.IsValid.Should().BeFalse();
         result = await _validator.ValidateAsync(new Person { Username = "a*1^" });
         result.IsValid.Should().BeTrue();

         //validation with regex
         _userSettings.UsernameValidationUseRegex = true;
         result = await _validator.ValidateAsync(new Person { Username = "test_user" });
         result.IsValid.Should().BeFalse();
         result = await _validator.ValidateAsync(new Person { Username = "a*1^" });
         result.IsValid.Should().BeFalse();
         result = await _validator.ValidateAsync(new Person { Username = "a1" });
         result.IsValid.Should().BeTrue();
         result = await _validator.ValidateAsync(new Person { Username = "a_test_user_name_1" });
         result.IsValid.Should().BeTrue();
         result = await _validator.ValidateAsync(new Person { Username = "b_test_user_name_1" });
         result.IsValid.Should().BeFalse();
      }
   }
}
