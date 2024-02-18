using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Data;
using Devices.Dispatcher.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSystemd();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(Defaults.ConnectionString));
builder.Services.AddConnectConfiguration(builder.Configuration);
builder.Services.AddHubGrpcClient(builder.Configuration);
builder.Services.AddAutoMapper();
builder.Services.AddSwagerTools();
builder.Services.AddAppLogicServices();
builder.Services.AddOptions();   
builder.Services.AddControllers();
var app = builder.Build();

app.UseAppSwagger();
app.MapControllers();

app.StartAplicationAsync();  

app.Run();