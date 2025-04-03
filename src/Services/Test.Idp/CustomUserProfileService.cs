using System.Security.Claims;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Test.Idp.Data.Models;

namespace Test.Idp;

internal sealed class CustomUserProfileService(
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
    ILogger<ProfileService<ApplicationUser>> logger)
    : ProfileService<ApplicationUser>(userManager, claimsFactory, logger)
{
    protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user)
    {
        var principal = await GetUserClaimsAsync(user);
        var id = (ClaimsIdentity)principal.Identity!;
        if (!string.IsNullOrEmpty(user.FavoriteColor))
            id.AddClaim(new Claim("favorite_color", user.FavoriteColor));

        context.AddRequestedClaims(principal.Claims);
    }
}