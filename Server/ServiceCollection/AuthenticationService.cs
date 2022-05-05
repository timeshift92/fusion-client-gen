using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;

namespace Uztelecom.Template.Server.ServiceCollectionExtensions
{
    public static class AuthenticationService
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/signIn";
                options.LogoutPath = "/signOut";
                if (env.IsDevelopment())
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                // This controls the expiration time stored in the cookie itself
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                // And this controls when the browser forgets the cookie
                options.Events.OnSigningIn = ctx =>
                {
                    ctx.CookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(28);
                    return Task.CompletedTask;
                };
            }).AddMicrosoftAccount(options =>
            {
                options.ClientId = "6839dbf7-d1d3-4eb2-a7e1-ce8d48f34d00";
                options.ClientSecret = Encoding.UTF8.GetString(Convert.FromBase64String("REFYeH4yNTNfcVNWX2h0WkVoc1V6NHIueDN+LWRxUTA2Zw=="));
                // That's for personal account authentication flow
                options.AuthorizationEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
                options.TokenEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            });
            return services;
        }

        public static IApplicationBuilder UseHybridCookie(this IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                var hybridIdCookieName = "hybrid-instance-id";
                if (context.Request.Cookies.All(c => c.Key != hybridIdCookieName))
                {
                    var idCookieOptions = new CookieOptions
                    {
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
            return app;
        }
    }
}
