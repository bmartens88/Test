using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Test.Web;
using Test.Web.Client.Weather;
using Test.Web.Components;
using Yarp.ReverseProxy.Transforms;
using _Imports = Test.Web.Client._Imports;

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
        opts.TokenValidationParameters.NameClaimType = "name";
        opts.ClaimActions.MapUniqueJsonKey("favorite_color", "favorite_color");
    })
    .AddCookie();

// Cookie with OIDC refresh
builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme,
    OpenIdConnectDefaults.AuthenticationScheme);

builder.Services.AddAuthorization();

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();
builder.Services.AddCascadingAuthenticationState();

// HTTP forwarding
builder.Services.AddHttpForwarderWithServiceDiscovery();
builder.Services.AddHttpContextAccessor();
// When interacting with the api, use this code
builder.Services.AddHttpClient<IWeatherForecaster, ServerWeatherForecaster>(httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly);

// When calling from the webassembly part, forward using this forwarding
app.MapForwarder("/weather-forecast", "https://api", transformBuilder =>
{
    transformBuilder.AddRequestTransform(async transformContext =>
    {
        var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");
        transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    });
}).RequireAuthorization();

app.MapLoginLogout();

app.MapGroup("/authentication").MapLoginLogout();

app.Run();