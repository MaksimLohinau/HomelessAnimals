using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.Models;
using HomelessAnimals.Shared.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace HomelessAnimals.Policies
{
    public class CookieValidationPolicy(IAccountService accountService) : CookieAuthenticationEvents
    {
        private readonly IAccountService _accountService = accountService;

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var claims = context.Principal.Claims.ToList();

            var lastValidated = claims.FirstOrDefault(c => c.Type == "LastValidated");

            if (lastValidated != null &&
                DateTime.Parse(lastValidated.Value) > DateTime.UtcNow.Subtract(TimeSpan.FromHours(1)))
                return;

            var userId = int.Parse(claims.FirstOrDefault(c => c.Type == nameof(AuthenticationInfo.VolunteerId)).Value);

            var permissionsInfo = await _accountService.GetPermissions(userId);

            var permissionsClaims = permissionsInfo.Permissions.Select(p => new Claim(nameof(Permissions), p));
            claims.RemoveAll(c => c.Type == nameof(Permissions));
            claims.AddRange(permissionsClaims);

            var scopesClaims = permissionsInfo.Scopes.Select(s => new Claim(nameof(AuthenticationInfo.Scopes), s.ToString()));
            claims.RemoveAll(c => c.Type == nameof(AuthenticationInfo.Scopes));
            claims.AddRange(scopesClaims);

            claims.Remove(lastValidated);
            claims.Add(new("LastValidated", DateTime.UtcNow.ToString()));

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(claimsIdentity);

            context.ReplacePrincipal(principal);
            context.ShouldRenew = true;
        }
    }
}
