using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clients.Dash.Services.Authentication;

//https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/additional-scenarios?view=aspnetcore-6.0
/// <summary>
/// Represents a custom implementation of the user factory 
/// </summary>
public class CustomUserFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="accessor"></param>
   public CustomUserFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
   {
   }

   /// <inheritdoc/>
   public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
   {
      var user = await base.CreateUserAsync(account, options);

      if (user.Identity.IsAuthenticated)
      {
         var identity = (ClaimsIdentity)user.Identity;
         var roleClaims = identity.FindAll(identity.RoleClaimType).ToArray();

         if (roleClaims.Any())
         {
            foreach (var existingClaim in roleClaims)
               identity.RemoveClaim(existingClaim);

            var rolesElem = account.AdditionalProperties[identity.RoleClaimType];

            if (rolesElem is JsonElement roles)
               if (roles.ValueKind == JsonValueKind.Array)
                  foreach (var role in roles.EnumerateArray())
                     identity.AddClaim(new Claim(options.RoleClaim, role.GetString()));
               else
                  identity.AddClaim(new Claim(options.RoleClaim, roles.GetString()));
         }

         var scopeClaims = identity.FindAll(options.ScopeClaim).ToArray();

         if (scopeClaims.Any())
         {
            foreach (var scopeClaim in scopeClaims)
               identity.RemoveClaim(scopeClaim);

            var scopeElem = account.AdditionalProperties[options.ScopeClaim];

            if (scopeElem is JsonElement scopes)
               if (scopes.ValueKind == JsonValueKind.Array)
                  foreach (var scope in scopes.EnumerateArray())
                     identity.AddClaim(new Claim(options.ScopeClaim, scope.GetString()));
               else
                  identity.AddClaim(new Claim(options.ScopeClaim, scopes.GetString()));
         }
      }

      return user;
   }
}