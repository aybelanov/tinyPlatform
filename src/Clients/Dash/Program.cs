using Clients.Dash;
using Clients.Dash.Infrastructure;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("browser")]

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.PrepareAppConfiguration();
builder.Services.AddAuthorizedHttpClient();
builder.Services.AddGrpcClients();
builder.Services.AddSignalrClientConnection();
builder.Services.AddAuthentication();
builder.Services.AddCaching();
builder.Services.AddRadzenServices();
builder.Services.AddApplicationServices();
builder.Services.AddAppLocalization();
builder.Services.AddAutoMapper();

var host = builder.Build();

host.InitializeGlobalVariables();

await host.RunAsync();