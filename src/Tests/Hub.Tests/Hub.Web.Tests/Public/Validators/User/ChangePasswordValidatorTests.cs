using FluentValidation.TestHelper;
using Hub.Tests;
using Hub.Web.Models.User;
using Hub.Web.Validators.User;
using NUnit.Framework;

namespace Hub.Web.Tests.Public.Validators.User
{
   [TestFixture]
   public class ChangePasswordValidatorTests : BaseAppTest
   {
      private ChangePasswordValidator _validator;

      [OneTimeSetUp]
      public void Setup()
      {
         _validator = GetService<ChangePasswordValidator>();
      }

      [Test]
      public void ShouldHaveErrorWhenOldPasswordIsNullOrEmpty()
      {
         var model = new ChangePasswordModel
         {
            OldPassword = null
         };
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.OldPassword);
         model.OldPassword = string.Empty;
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.OldPassword);
      }

      [Test]
      public void ShouldNotHaveErrorWhenOldPasswordIsSpecified()
      {
         var model = new ChangePasswordModel
         {
            OldPassword = "old password"
         };
         _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.OldPassword);
      }

      [Test]
      public void ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()
      {
         var model = new ChangePasswordModel
         {
            NewPassword = null
         };
         //we know that new password should equal confirmation password
         model.ConfirmNewPassword = model.NewPassword;
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
         model.NewPassword = string.Empty;
         //we know that new password should equal confirmation password
         model.ConfirmNewPassword = model.NewPassword;
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
      }

      [Test]
      public void ShouldNotHaveErrorWhenNewPasswordIsSpecified()
      {
         var model = new ChangePasswordModel
         {
            NewPassword = "Mnbvcxz1!"
         };
         //we know that new password should equal confirmation password
         model.ConfirmNewPassword = model.NewPassword;
         _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
      }

      [Test]
      public void ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty()
      {
         var model = new ChangePasswordModel
         {
            ConfirmNewPassword = null
         };
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
         model.ConfirmNewPassword = string.Empty;
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
      }

      [Test]
      public void ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified()
      {
         var model = new ChangePasswordModel
         {
            ConfirmNewPassword = "some password"
         };
         //we know that new password should equal confirmation password
         model.NewPassword = model.ConfirmNewPassword;
         _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ConfirmNewPassword);
      }

      [Test]
      public void ShouldHaveErrorWhenNewPasswordDoesNotEqualConfirmationPassword()
      {
         var model = new ChangePasswordModel
         {
            NewPassword = "some password",
            ConfirmNewPassword = "another password"
         };
         _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
      }

      [Test]
      public void ShouldNotHaveErrorWhenNewPasswordEqualsConfirmationPassword()
      {
         var model = new ChangePasswordModel
         {
            NewPassword = "Mnbvcxz1!",
            ConfirmNewPassword = "Mnbvcxz1!"
         };
         _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
      }
   }
}
