using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Devices.Dispatcher.Infrastructure;

/// <summary>
/// Application builder extensions
/// </summary>
public static class ApplicationBuilderExtensions
{
   /// <summary>
   /// Some required or optional procedures before running the appliaction
   /// </summary>
   /// <param name="application"></param>
   public static void StartAplicationAsync(this IApplicationBuilder application)
   {
      var services = application.ApplicationServices;
      var scopeProvider = services.GetRequiredService<IServiceScopeFactory>();

      // Ensures the database is created
      using var scope = scopeProvider.CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
      // TODO migration inplementation if it needed
      dbContext.Database.EnsureCreated();

      // Ensure the video file directory is created
      //var contentRoot = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>().ContentRootPath;
      var config = application.ApplicationServices.GetRequiredService<IConfiguration>();
      Defaults.VideoSegmentsPath = Path.Combine(Path.GetTempPath(), config["HubConnections:SystemName"], "VideoSegments");
      if (!Directory.Exists(Defaults.VideoSegmentsPath)) Directory.CreateDirectory(Defaults.VideoSegmentsPath);

      var sentDir = Path.Combine(Defaults.VideoSegmentsPath, "sent");
      if (!Directory.Exists(sentDir)) Directory.CreateDirectory(sentDir);
   }

   /// <summary>
   /// Configure Swagger middleware
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppSwagger(this IApplicationBuilder application)
   {
      application.UseSwagger();
      application.UseSwaggerUI();
   }
}
