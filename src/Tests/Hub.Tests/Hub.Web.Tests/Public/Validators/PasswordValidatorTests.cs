using FluentValidation.TestHelper;
using Hub.Core.Domain.Users;
using Hub.Services.Directory;
using Hub.Services.Localization;
using Hub.Tests;
using Hub.Web.Framework.Validators;
using NUnit.Framework;

namespace Hub.Web.Tests.Public.Validators;

[TestFixture]
public class PasswordValidatorTests : BaseAppTest
{
   private Person _person;
   private ILocalizationService _localizationService;
   private IStateProvinceService _stateProvinceService;
   private UserSettings _userSettings;

   [OneTimeSetUp]
   public void Setup()
   {
      _userSettings = new UserSettings
      {
         PasswordMinLength = 8,
         PasswordRequireUppercase = true,
         PasswordRequireLowercase = true,
         PasswordRequireDigit = true,
         PasswordRequireNonAlphanumeric = true
      };

      _localizationService = GetService<ILocalizationService>();
      _stateProvinceService = GetService<IStateProvinceService>();

      _person = new Person();
   }

   [Test]
   public void IsValidTestsLowercase()
   {
      var validator = new TestValidator();

      var cs = new UserSettings
      {
         PasswordMinLength = 3,
         PasswordRequireLowercase = true
      };

      validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

      //ShouldHaveValidationError
      _person.Password = "APP123";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

      //ShouldNotHaveValidationError
      _person.Password = "app123";
      validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
   }

   [Test]
   public void IsValidTestsUppercase()
   {
      var validator = new TestValidator();

      var cs = new UserSettings
      {
         PasswordMinLength = 3,
         PasswordRequireUppercase = true
      };

      validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

      //ShouldHaveValidationError
      _person.Password = "app";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

      //ShouldNotHaveValidationError
      _person.Password = "App";
      validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
   }

   [Test]
   public void IsValidTestsDigit()
   {
      var validator = new TestValidator();

      var cs = new UserSettings
      {
         PasswordMinLength = 3,
         PasswordRequireDigit = true
      };

      validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

      //ShouldHaveValidationError
      _person.Password = "app";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

      //ShouldNotHaveValidationError
      _person.Password = "App1";
      validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
   }

   [Test]
   public void IsValidTestsNonAlphanumeric()
   {
      var validator = new TestValidator();

      var cs = new UserSettings
      {
         PasswordMinLength = 3,
         PasswordRequireNonAlphanumeric = true
      };

      validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

      //ShouldHaveValidationError
      _person.Password = "app";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

      //ShouldNotHaveValidationError
      _person.Password = "App#";
      validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
   }

   [Test]
   public void IsValidTestsAllRules()
   {
      var validator = new TestValidator();

      //Example:  (?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$
      validator.RuleFor(x => x.Password).IsPassword(_localizationService, _userSettings);

      //ShouldHaveValidationError
      _person.Password = string.Empty;
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
      _person.Password = "123";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
      _person.Password = "12345678";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
      _person.Password = "password";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
      _person.Password = "Password";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
      _person.Password = "password123";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
      _person.Password = "Password123";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
      _person.Password = "password123$";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
      _person.Password = "PASSWORD123$";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
      _person.Password = "pASSWORD123~";
      validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

      //ShouldNotHaveValidationError
      _person.Password = "Password123$";
      validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
   }
}
