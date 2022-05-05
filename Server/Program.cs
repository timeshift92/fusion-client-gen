using Stl.Fusion.Server;
using Uztelecom.Template.Client;
using Uztelecom.Template.Server.Blazor;
using MudBlazor.Services;
using Uztelecom.Template.Server.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var cfg = builder.Configuration;
var env = builder.Environment;

// Options
services.Configure<BlazorHybridOptions>(cfg.GetSection(nameof(BlazorHybridOptions)));

services.AddAuth(env);

services.AddFusionServices();

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
app.UseWebSockets(new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(30),
});
app.UseFusionSession();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseHybridCookie();

app.MapFusionWebSocketServer();
app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
