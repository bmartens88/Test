using System.Net.Http.Headers;
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

// HTTP Client setup
builder.Services.AddHttpClient("sourceClient", client => { client.BaseAddress = new Uri("https://source"); });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", async (HttpContext context, IHttpClientFactory clientFactory, ClaimsPrincipal user) =>
    {
        var accessToken = await context.GetTokenAsync("access_token");
        var client = clientFactory.CreateClient("sourceClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.GetFromJsonAsync<IEnumerable<WeatherForecast>>("/");
        var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return TypedResults.Ok(new { claims, response });
    })
    .RequireAuthorization(); // trigger auth flow

app.MapDefaultEndpoints();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}