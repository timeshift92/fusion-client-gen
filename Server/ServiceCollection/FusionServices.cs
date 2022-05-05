using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Stl.Fusion;
using Stl.Fusion.Extensions;
using Stl.Fusion.Server;
using Stl.Fusion.Server.Authentication;
using Stl.Fusion.Server.Controllers;
using Uztelecom.Template.Server.Services;
using Uztelecom.Template.Shared;

namespace Uztelecom.Template.Server.ServiceCollectionExtensions
{
    public static class FusionServices
    {
        public static IServiceCollection AddFusionServices(this IServiceCollection services)
        {
            // Fusion
            var fusion = services.AddFusion();
            var fusionServer = fusion.AddWebServer();
            var fusionAuth = fusion.AddAuthentication();
            fusionAuth.AddAuthBackend().AddServer(
                signInControllerSettingsFactory: _ => SignInController.DefaultSettings with
                {
                    DefaultScheme = MicrosoftAccountDefaults.AuthenticationScheme,
                    SignInPropertiesBuilder = (_, properties) => {
                        properties.IsPersistent = true;
                    }
                },
                serverAuthHelperSettingsFactory: _ => ServerAuthHelper.DefaultSettings with
                {
                    NameClaimKeys = Array.Empty<string>(),
                });

            // Fusion services
            fusion.AddFusionTime(); // IFusionTime is one of built-in compute services you can use
            fusion.AddComputeService<ICounterService, CounterService>();
            fusion.AddComputeService<IWeatherForecastService, WeatherForecastService>();

            return services;
        }
    }
}
