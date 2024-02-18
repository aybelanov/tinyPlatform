using Grpc.Core;
using Hub.Core.Configuration;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Services.Devices;
using Hub.Services.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Shared.Clients.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hub.Services.Authentication;

/// <summary>
/// Represents service using cookie middleware for the authentication
/// </summary>
public partial class MixedAuthenticationService : IAuthenticationService
{
   #region Fields

   private readonly UserSettings _userSettings;
   private readonly DeviceSettings _deviceSettings;
   private readonly IUserService _userService;
   private readonly IHubDeviceService _deviceService;
   private readonly IHttpContextAccessor _httpContextAccessor;
   private readonly string _claimIssuer;
 
   private User _cachedUser;
   private Device _cachedDevice;

   #endregion

   #region Ctor

   /// <summary>
   /// Ioc Ctor
   /// </summary>
   public MixedAuthenticationService(
      UserSettings userSettings,
       IUserService userService,
       DeviceSettings deviceSettings,
       AppSettings appSettings,
       IHubDeviceService deviceService,
       IHttpContextAccessor httpContextAccessor)
   {
      _userSettings = userSettings;
      _userService = userService;
      _deviceService = deviceService;
      _httpContextAccessor = httpContextAccessor;
      _deviceSettings = deviceSettings;
      _claimIssuer = appSettings.Get<HostingConfig>().HubHostUrl.Trim('/');
   }

   #endregion

   #region Methods

   /// <summary>
   /// Sign in
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SignInAsync(User user, bool isPersistent)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      //create claims for user's username and email
      var claims = new List<Claim>();

      if (!string.IsNullOrEmpty(user.Username))
         claims.Add(new Claim(ClaimTypes.Name, user.Username, ClaimValueTypes.String, _claimIssuer));

      if (!string.IsNullOrEmpty(user.Email))
         claims.Add(new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.Email, _claimIssuer));

      var roles = (await _userService.GetUserRolesAsync(user)).Where(x=>!x.SystemName.Equals(UserDefaults.RegisteredRoleName));
      foreach (var role in roles)
         claims.Add(new Claim(ClaimTypes.Role, role.SystemName, _claimIssuer));

      //create principal for the current authentication scheme
      var userIdentity = new ClaimsIdentity(claims, AuthDefaults.MixedScheme);
      var userPrincipal = new ClaimsPrincipal(userIdentity);

      //set value indicating whether session is persisted and the time at which the authentication was issued
      var authenticationProperties = new AuthenticationProperties
      {
         IsPersistent = isPersistent,
         IssuedUtc = DateTime.UtcNow
      };

      //sign in
      await _httpContextAccessor.HttpContext.SignInAsync(AuthDefaults.MixedScheme, userPrincipal, authenticationProperties);

      //cache authenticated user
      _cachedUser = user;
   }

   /// <summary>
   /// Sign out
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task SignOutAsync()
   {
      //reset cached user
      _cachedUser = null;

      //and sign out from the current authentication scheme
      await _httpContextAccessor.HttpContext.SignOutAsync(AuthDefaults.CookieAuthenticationScheme);
   }

   /// <summary>
   /// Get authenticated user
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user
   /// </returns>
   public virtual async Task<User> GetAuthenticatedUserAsync()
   {
      //whether there is a cached user
      if (_cachedUser != null)
         return _cachedUser;

      //try to get authenticated user identity
      var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(AuthDefaults.MixedScheme);
      if (!authenticateResult.Succeeded)
         return null;

      User user = null;      
      //try to get user by email
      var emailClaim = authenticateResult.Principal.FindFirst(claim => (claim.Type == JwtRegisteredClaimNames.Email || claim.Type == ClaimTypes.Email)
            && claim.Issuer.Equals(_claimIssuer, StringComparison.InvariantCultureIgnoreCase));
      if (emailClaim != null)
         user = await _userService.GetUserByEmailAsync(emailClaim.Value);
     
      //whether the found user is available
      if (user == null || !user.IsActive || user.RequireReLogin || user.IsDeleted || !await _userService.IsRegisteredAsync(user))
         return null;

      //cache authenticated user
      _cachedUser = user;

      return _cachedUser;
   }

   /// <summary>
   /// Get authenticated device
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the device
   /// </returns>
   public async Task<Device> GetAuthenticatedDeviceAsync()
   {
      //whether there is a cached user
      if (_cachedDevice != null)
         return _cachedDevice;

      // To comminaction with clients and devices we use gRPC. Unauhenticated and not allowed device cannot get access to hub functionality.
      // Thus we throw a gRPC exception to trace to a device log with an error data reason.

      // TODO refactor to user by user guid (and device by guid)

      //try to get authenticated user identity
      var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(AuthDefaults.MixedScheme);
      if (!authenticateResult.Succeeded)
         throw new RpcException(new Status(StatusCode.Unauthenticated, "The device is not authenticated"));

      // check if it's a user request
      var clientId = authenticateResult.Principal.FindFirst(claim => (claim.Type == "client_id"));
      if (clientId == null || !clientId.Value.Equals(AuthDefaults.DispatcherApp))
         return null;

      Device device = null;

      var deviceIdClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == JwtRegisteredClaimNames.Name
             && claim.Issuer.Equals(_claimIssuer, StringComparison.InvariantCultureIgnoreCase));

      if (deviceIdClaim != null && !string.IsNullOrEmpty(deviceIdClaim.Value))
         device = await _deviceService.GetDeviceBySystemNameAsync(deviceIdClaim.Value);

      // chek device
      if (device == null)
         throw new RpcException(new Status(StatusCode.NotFound, "The device is not found"));

      if (!device.IsActive)
         throw new RpcException(new Status(StatusCode.Aborted, "The device is not active or not approved."));

      if (!device.Enabled)
         throw new RpcException(new Status(StatusCode.Aborted, "The device is disabled."));

      if (device.IsDeleted)
         throw new RpcException(new Status(StatusCode.Aborted, "The device is deleted."));

      // checks user-owner
      var user = await _userService.GetUserByIdAsync(device.OwnerId)
         ?? throw new RpcException(new Status(StatusCode.Aborted, "The device owner is not found."));

      if (!user.IsActive && _deviceSettings.BlockDevicesIfOwnerNotActive)
         throw new RpcException(new Status(StatusCode.Aborted, "The device owner is not active (disabled or not approved)."));

      //if (user.RequireReLogin)
      //   throw new RpcException(new Status(StatusCode.Aborted, "The device owner must relogin."));

      if (user.IsDeleted)
         throw new RpcException(new Status(StatusCode.Aborted, "The device owner is deleted."));


      if (!await _userService.IsRegisteredAsync(user))
         throw new RpcException(new Status(StatusCode.Aborted, "The device owner is not registered."));

      //cache authenticated user
      _cachedDevice = device;

      return _cachedDevice;
   }

   #endregion
}