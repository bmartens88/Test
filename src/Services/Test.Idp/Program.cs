using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Test.Idp;
using Test.Idp.Components;
using Test.Idp.Components.Account;
using Test.Idp.Data;
using Test.Idp.Data.Models;
using Test.Idp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Blazor
builder.Services.AddRazorComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();

// Aspire
builder.AddServiceDefaults();

// EF
builder.Services.AddDbContextPool<ApplicationDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("idpDb")));
builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

// Identity
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// IdentityServer
builder.Services.AddIdentityServer()
    .AddInMemoryClients([
        new Client
        {
            ClientId = "web",
            ClientSecrets = [new Secret("secret".Sha256())],
            AllowedGrantTypes = GrantTypes.Code,
            AllowedScopes =
            [
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                "color"
            ],
            AllowOfflineAccess = true,
            RedirectUris = [$"{Environment.GetEnvironmentVariable("WEB_HTTPS")}/signin-oidc"],
            PostLogoutRedirectUris = [$"{Environment.GetEnvironmentVariable("WEB_HTTPS")}/signout-callback-oidc"],
            RequirePkce = true
        }
    ])
    .AddInMemoryIdentityResources([
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email(),
        new IdentityResource("color", ["favorite_color"])
    ])
    .AddAspNetIdentity<ApplicationUser>()
    .AddProfileService<CustomUserProfileService>();

// Authn & Authz
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = IdentityConstants.ApplicationScheme;
        opts.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
        };
    })
    .AddCookie(IdentityConstants.ExternalScheme, options =>
    {
        options.Cookie.Name = IdentityConstants.ExternalScheme;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    await app.MigrateDbAsync<ApplicationDbContext>();

app.UseIdentityServer();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>();

app.MapAdditionalIdentityEndpoints();
app.MapDefaultEndpoints();

app.Run();