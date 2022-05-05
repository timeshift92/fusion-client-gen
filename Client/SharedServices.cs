using Microsoft.AspNetCore.Components.Authorization;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.Blazor;
using Stl.Fusion.Extensions;
using Stl.Fusion.UI;

namespace Uztelecom.Template.Client;

public static class SharedServices
{
    public static void Configure(IServiceCollection services)
    {
        var fusion = services.AddFusion();
        var fusionAuth = fusion.AddAuthentication().AddBlazor();

        fusion.AddBlazorUIServices();
        fusion.AddFusionTime();

        services.AddScoped(async c =>
        {
            var auth = c.GetRequiredService<IAuth>();
            var sessionResolver = c.GetRequiredService<ISessionResolver>();
            var session = await sessionResolver.GetSession(CancellationToken.None).ConfigureAwait(false);
            var user = await auth.GetUser(session).ConfigureAwait(false);
            return user;
        });

        services.AddScoped(async c =>
        {
            var asp = c.GetRequiredService<AuthStateProvider>();
            return await asp.GetAuthenticationStateAsync().ConfigureAwait(false);
        });
        services.AddTransient(async c =>
        {
            var asp = c.GetRequiredService<AuthStateProvider>();
            return (AuthState)await asp.GetAuthenticationStateAsync().ConfigureAwait(false);
        });

        services.AddTransient<IUpdateDelayer>(c => new UpdateDelayer(c.GetRequiredService<IUICommandTracker>(), 0.1));
    }
}
