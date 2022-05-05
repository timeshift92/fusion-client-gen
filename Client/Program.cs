using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Stl.Fusion;
using Stl.Fusion.Blazor;
using Stl.Fusion.Client;
using Stl.Fusion.Extensions;
using Stl.Fusion.UI;
using Uztelecom.Template.Shared;
using Uztelecom.Template.Client;
using Uztelecom.Template.Client.Services;
using MudBlazor.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;
var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
var apiBaseUri = new Uri($"{baseUri}api");


// Fusion
var fusion = services.AddFusion();
var fusionClient = fusion.AddRestEaseClient(
    (c, o) => {
        o.BaseUri = baseUri;
        o.IsLoggingEnabled = true;
        o.IsMessageLoggingEnabled = false;
    }).ConfigureHttpClientFactory(
    (c, name, o) => {
        var isFusionClient = (name ?? "").StartsWith("Stl.Fusion");
        var clientBaseUri = isFusionClient ? baseUri : apiBaseUri;
        o.HttpClientActions.Add(client => client.BaseAddress = clientBaseUri);
    });
var fusionAuth = fusion.AddAuthentication().AddRestEaseClient();

// Fusion service clients
fusionClient.AddFusionClients();

services.AddMudServices();

SharedServices.Configure(builder.Services);

await builder.Build().RunAsync();
