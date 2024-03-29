﻿using FluentValidation.TestHelper;
using Hub.Core.Domain.Users;
using Hub.Services.Directory;
using Hub.Services.Localization;
using Hub.Tests;
using Hub.Web.Models.User;
using Hub.Web.Validators.User;
using NUnit.Framework;

namespace Hub.Web.Tests.Public.Validators.User
{
   [TestFixture]
   public class RegisterValidatorTests : BaseAppTest
   {
      private RegisterValidator _validator;

      [OneTimeSetUp]
      public void SetUp()
      {
         _validator = GetService<RegisterValidator>();
      }

      [Test]
      public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
      {
         var model = new RegisterModel
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
         var model = new RegisterModel
         {
            Email = "adminexample.com"
         };
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
      }

      [Test]
      public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
      {
         var model = new RegisterModel
         {
            Email = "admin@example.com"
         };
         _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
      }

      [Test]
      public void ShouldHaveErrorWhenFirstnameIsNullOrEmpty()
      {
         var userSettings = new UserSettings
         {
            FirstNameEnabled = true,
            FirstNameRequired = true
         };

         var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), userSettings);
         var model = new RegisterModel
         {
            FirstName = null
         };
         validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
         model.FirstName = string.Empty;
         validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
      }

      [Test]
      public void ShouldNotHaveErrorWhenFirstnameIsSpecified()
      {
         var userSettings = new UserSettings
         {
            FirstNameEnabled = true
         };

         var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), userSettings);

         var model = new RegisterModel
         {
            FirstName = "John"
         };
         validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FirstName);
      }

      [Test]
      public void ShouldHaveErrorWhenLastNameIsNullOrEmpty()
      {
         var userSettings = new UserSettings
         {
            LastNameEnabled = true,
            LastNameRequired = true
         };

         var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), userSettings);

         var model = new RegisterModel
         {
            LastName = null
         };

         validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
         model.LastName = string.Empty;
         validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
      }

      [Test]
      public void ShouldNotHaveErrorWhenLastNameIsSpecified()
      {
         var userSettings = new UserSettings
         {
            LastNameEnabled = true
         };

         var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), userSettings);

         var model = new RegisterModel
         {
            LastName = "Smith"
         };
         validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
      }

      [Test]
      public void ShouldHaveErrorWhenPasswordIsNullOrEmpty()
      {
         var model = new RegisterModel
         {
            Password = null
         };
         //we know that password should equal confirmation password
         model.ConfirmPassword = model.Password;
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
         model.Password = string.Empty;
         //we know that password should equal confirmation password
         model.ConfirmPassword = model.Password;
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
      }

      [Test]
      public void ShouldNotHaveErrorWhenPasswordIsSpecified()
      {
         var model = new RegisterModel
         {
            Password = "Mnbvcxz1!"
         };
         //we know that password should equal confirmation password
         model.ConfirmPassword = model.Password;
         _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Password);
      }

      [Test]
      public void ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()
      {
         var model = new RegisterModel
         {
            ConfirmPassword = null
         };
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
         model.ConfirmPassword = string.Empty;
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
      }

      [Test]
      public void ShouldNotHaveErrorWhenConfirmPasswordIsSpecified()
      {
         var model = new RegisterModel
         {
            ConfirmPassword = "some password"
         };
         //we know that new password should equal confirmation password
         model.Password = model.ConfirmPassword;
         _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword);
      }

      [Test]
      public void ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()
      {
         var model = new RegisterModel
         {
            Password = "some password",
            ConfirmPassword = "another password"
         };
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
      }

      [Test]
      public void ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()
      {
         var model = new RegisterModel
         {
            Password = "Mnbvcxz1!",
            ConfirmPassword = "Mnbvcxz1!"
         };
         _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Password);
      }
   }
}
