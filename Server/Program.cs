using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Stl.Fusion;
using Stl.Fusion.Blazor;
using Stl.Fusion.Extensions;
using Stl.Fusion.Server;
using Stl.Fusion.Server.Authentication;
using Stl.Fusion.Server.Controllers;
using Uztelecom.Template.Client;
using Uztelecom.Template.Server.Blazor;
using Uztelecom.Template.Server.Services;
using Uztelecom.Template.Shared;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var cfg = builder.Configuration;
var env = builder.Environment;

// Options
services.Configure<BlazorHybridOptions>(cfg.GetSection(nameof(BlazorHybridOptions)));
services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options => {
    options.LoginPath = "/signIn";
    options.LogoutPath = "/signOut";
    if (env.IsDevelopment())
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    // This controls the expiration time stored in the cookie itself
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    // And this controls when the browser forgets the cookie
    options.Events.OnSigningIn = ctx => {
        ctx.CookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(28);
        return Task.CompletedTask;
    };
}).AddMicrosoftAccount(options => {
    options.ClientId = "6839dbf7-d1d3-4eb2-a7e1-ce8d48f34d00";
    options.ClientSecret = Encoding.UTF8.GetString(Convert.FromBase64String("REFYeH4yNTNfcVNWX2h0WkVoc1V6NHIueDN+LWRxUTA2Zw=="));
    // That's for personal account authentication flow
    options.AuthorizationEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
    options.TokenEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
});

// Fusion
var fusion = services.AddFusion();
var fusionServer = fusion.AddWebServer();
var fusionAuth = fusion.AddAuthentication();
fusionAuth.AddAuthBackend().AddServer(
    signInControllerSettingsFactory: _ => SignInController.DefaultSettings with {
        DefaultScheme = MicrosoftAccountDefaults.AuthenticationScheme,
        SignInPropertiesBuilder = (_, properties) => {
            properties.IsPersistent = true;
        }
    },
    serverAuthHelperSettingsFactory: _ => ServerAuthHelper.DefaultSettings with {
        NameClaimKeys = Array.Empty<string>(),
    });

// Fusion services
fusion.AddFusionTime(); // IFusionTime is one of built-in compute services you can use
fusion.AddComputeService<ICounterService, CounterService>();
fusion.AddComputeService<IWeatherForecastService, WeatherForecastService>();

services.AddControllersWithViews();
services.AddRazorPages();
services.AddServerSideBlazor(o => o.DetailedErrors = true);

SharedServices.Configure(services); // Must follow services.AddServerSideBlazor()!

services.AddEndpointsApiExplorer();
services.AddSwaggerDocument();
services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
    app.UseOpenApi();
    app.UseSwaggerUi3();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseWebSockets(new WebSocketOptions() {
    KeepAliveInterval = TimeSpan.FromSeconds(30),
});
app.UseFusionSession();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Use((context, next) => {
    var hybridIdCookieName = "hybrid-instance-id";
    if (context.Request.Cookies.All(c => c.Key != hybridIdCookieName)) {
        var idCookieOptions = new CookieOptions {
            Path = "/",
            Secure = true,
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddYears(100),
        };
        context.Response.Cookies.Append(
            key: hybridIdCookieName,
            value: Guid.NewGuid().ToString(),
            options: idCookieOptions);
        return next();
    }
    return next();
});

app.MapFusionWebSocketServer();
app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
