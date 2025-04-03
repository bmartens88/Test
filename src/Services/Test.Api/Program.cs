using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Aspire
builder.AddServiceDefaults();

// Authn && Authz
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddOpenIdConnect(opts =>
    {
        opts.Authority = Environment.GetEnvironmentVariable("IDP_HTTPS");
        opts.ClientId = "web";
        opts.ClientSecret = "secret";
        opts.ResponseType = OpenIdConnectResponseType.Code;
        opts.Scope.Add(OpenIdConnectScope.Email);
        opts.Scope.Add(OpenIdConnectScope.OfflineAccess);
        opts.Scope.Add("color");
        opts.UsePkce = true;
        opts.MapInboundClaims = false;
        opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.SaveTokens = true;
        opts.GetClaimsFromUserInfoEndpoint = true;
        opts.ClaimActions.MapUniqueJsonKey("favorite_color", "favorite_color");
    })
    .AddCookie();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (ClaimsPrincipal user) =>
    {
        var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return TypedResults.Ok(claims);
    })
    .RequireAuthorization(); // trigger auth flow

app.MapDefaultEndpoints();

app.Run();