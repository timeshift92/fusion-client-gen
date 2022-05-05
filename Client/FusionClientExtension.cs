
using Stl.Fusion.Client;
using Uztelecom.Template.Client.Services;
using Uztelecom.Template.Shared;
    

namespace Uztelecom.Template.Client;

public static class FusionClientExtension
{
    public static FusionRestEaseClientBuilder AddFusionClients(this FusionRestEaseClientBuilder fusionClient)
    {
        fusionClient.AddReplicaService<IWeatherForecastService, IWeatherForecastClientDef>();
fusionClient.AddReplicaService<ICounterService, ICounterClientDef>();


        return fusionClient;
    }
}