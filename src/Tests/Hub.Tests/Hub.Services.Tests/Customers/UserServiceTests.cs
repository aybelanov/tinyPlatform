using FluentAssertions;
using Hub.Services.Users;
using Hub.Tests;
using NUnit.Framework;
using Shared.Clients.Configuration;
using System.Threading.Tasks;

namespace Hub.Services.Tests.Users;

[TestFixture]
public class UserServiceTests : ServiceTest
{
   private IUserService _userService;

   [OneTimeSetUp]
   public async Task SetUp()
   {
      _userService = GetService<IUserService>();
      var moderator = await _userService.GetUserRoleBySystemNameAsync(UserDefaults.ForumModeratorsRoleName);
      moderator.Active = false;
      await _userService.UpdateUserRoleAsync(moderator);
   }

   [OneTimeTearDown]
   public async Task TearDown()
   {
      var moderator = await _userService.GetUserRoleBySystemNameAsync(UserDefaults.ForumModeratorsRoleName);
      moderator.Active = true;
      await _userService.UpdateUserRoleAsync(moderator);
   }

   [Test]
   public async Task CanCheckIsInUserRole()
   {
      var user = await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail);

      var isInUserRole = await _userService.IsInUserRoleAsync(user, UserDefaults.AdministratorsRoleName, false);
      isInUserRole.Should().BeTrue();
      isInUserRole = await _userService.IsInUserRoleAsync(user, UserDefaults.AdministratorsRoleName);
      isInUserRole.Should().BeTrue();

      isInUserRole = await _userService.IsInUserRoleAsync(user, UserDefaults.ForumModeratorsRoleName, false);
      isInUserRole.Should().BeTrue();
      isInUserRole = await _userService.IsInUserRoleAsync(user, UserDefaults.ForumModeratorsRoleName);
      isInUserRole.Should().BeFalse();

      isInUserRole = await _userService.IsInUserRoleAsync(user, UserDefaults.GuestsRoleName, false);
      isInUserRole.Should().BeFalse();
      isInUserRole = await _userService.IsInUserRoleAsync(user, UserDefaults.GuestsRoleName);
      isInUserRole.Should().BeFalse();
   }

   [Test]
   public async Task CanCheckWhetherUserIsAdmin()
   {
      var user = await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail);
      var isAdmin = await _userService.IsAdminAsync(user);
      isAdmin.Should().BeTrue();
   }

   [Test]
   public async Task CanCheckWhetherUserIsForumModerator()
   {
      var user = await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail);
      var isForumModerator = await _userService.IsForumModeratorAsync(user, false);
      isForumModerator.Should().BeTrue();
   }

   [Test]
   public async Task CanCheckWhetherUserIsGuest()
   {
      var user = await _userService.GetUserByEmailAsync("builtin@search_engine_record.com");
      var isGuest = await _userService.IsGuestAsync(user);
      isGuest.Should().BeTrue();
   }

   [Test]
   public async Task CanCheckWhetherUserIsRegistered()
   {
      var user = await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail);

      var isRegistered = await _userService.IsRegisteredAsync(user);
      isRegistered.Should().BeTrue();
   }

   [Test]
   public async Task CanRemoveAddressAssignedAsBillingAddress()
   {
      var user = await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail);
      var addresses = await _userService.GetAddressesByUserIdAsync(user.Id);

      addresses.Count.Should().Be(1);

      var address = addresses[0];

      await _userService.InsertUserAddressAsync(user, address);

      var addressesByUser = await _userService.GetAddressesByUserIdAsync(user.Id);
      addressesByUser.Count.Should().Be(1);

      var billingAddress = await _userService.GetUserBillingAddressAsync(user);
      billingAddress.Should().NotBeNull();

      billingAddress = await _userService.GetUserBillingAddressAsync(user);
      billingAddress.Id.Should().Be(address.Id);

      await _userService.RemoveUserAddressAsync(user, address);

      addressesByUser = await _userService.GetAddressesByUserIdAsync(user.Id);
      var countAddresses = addressesByUser.Count;

      var billingAddressId = user.BillingAddressId;

      await _userService.InsertUserAddressAsync(user, address);
      user.BillingAddressId = address.Id;

      countAddresses.Should().Be(0);
      billingAddressId.Should().BeNull();
   }
}
