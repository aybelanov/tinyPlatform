using Hub.Core.Infrastructure;
using Hub.Data;
using Hub.Services.Clients;
using Hub.Web.Framework.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Hub.Web.Infrastructure;

/// <summary>
/// Represents the registering services on application startup
/// </summary>
public class AppStartup : IAppStartup
{
   /// <summary>
   /// Add and configure any of the middleware
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   /// <param name="configuration">Configuration of the application</param>
   public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
   {
      services.AddAppDbContext<AppDbContext>();
      services.AddResponseCompression();
      services.AddAppWebOptimizer();
      services.AddSwagerTools();
      services.AddOptions();
      services.AddDistributedCache();
      services.AddHttpSession();
      services.AddAppHttpClients();
      services.AddAntiForgery();
      services.AddThemes();
      services.AddAppRouting();
      services.AddAppRateLimiter();
      services.AddAppWebMarkupMin();
      services.AddAppMiniProfiler();
      services.AddAppDataProtection();
      services.AddAppCors();
      services.AddAppAuthentication();
      services.AddAppAuthorization();
      services.AddAppMvc();
      services.AddWebEncoders();
      services.AddAppRedirectResultExecutor();
      services.AddAppGrpc();
      services.AddAppSignalR();
   }


   /// <summary>
   /// Configure the using of added middleware
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public void Configure(IApplicationBuilder application)
   {
      application.UseAppProxy();
      application.UseAppResponseCompression();
      application.UseAppExceptionHandler();

      #region api

      application.MapWhen(IsApiRequest, app =>
      {
         app.UseInstallUrl();
         app.UseAppSwagger();
         app.UseAppRequestLocalization();
         app.UseMiniProfiler();
         app.UseRouting();
         app.UseGrpcWeb();
         app.UseAppCors();
         app.UseAppAuthentication();
         app.UseAuthorization();
         app.UseAppRateLimiter();
         app.UseDemoMode();
         app.UseSignalrEndpoinst();
         app.UseGrpcEndpoinst();
         app.UseApiEndpoinst();
      });

      #endregion

      #region ipcam streamiing

      application.MapWhen(IsVideoSegmentRequest, app =>
      {
         app.UseInstallUrl();
         app.UseAppCors();
         app.UseAppAuthentication();
         app.UseAuthorization();
         app.UseAppRateLimiter();
         app.UseMiniProfiler();
         app.UseStaticIpCamFiles();
      });

      #endregion

      #region mvc requests

      application.MapWhen(IsMvcRequest, app =>
      {
         app.UseBadRequestResult();
         app.UsePageNotFound();
         app.UseAppWebOptimizer();
         app.UseAppStaticFiles();
         app.UseKeepAlive();
         app.UseInstallUrl();
         app.UseAppSwagger();
         app.UseSession();
         app.UseAppRequestLocalization();
         app.UseAppWebMarkupMin();
         app.UseMiniProfiler();
         app.UseRouting();
         app.UseAppCors();
         app.UseAppAuthentication();
         app.UseAuthorization();
         app.UseAppRateLimiter();
         app.UseDemoMode();
         app.UseMvcEndpoints();
      });

      #endregion
   }

   /// <summary>
   /// Gets order of this startup configuration implementation
   /// </summary>
   public int Order => 0;

   #region Utilities

   private bool IsApiRequest(HttpContext context)
   {
      var res = DataSettingsManager.IsDatabaseInstalled() && (
                context.Request.Path.StartsWithSegments(new PathString(ClientDefaults.WebApiEndpoint))
             || context.Request.Path.StartsWithSegments(new PathString("/.well-known/openid-configuration"))
             || context.Request.Path.StartsWithSegments(new PathString("/connect"))
             || context.Request.Path.StartsWithSegments(new PathString("/swagger"))
             || context.Request.Path.StartsWithSegments(new PathString(ClientDefaults.SignalrEndpoint))
             || ClientDefaults.GrpcContracts.Any(x => context.Request.Path.ToString().Contains(x)));

      return res;
   }

   private bool IsVideoSegmentRequest(HttpContext context)
   {
      var res = context.Request.Path.StartsWithSegments(new PathString(ClientDefaults.IpCamEndpoint));
      return res;
   }

   private bool IsMvcRequest(HttpContext context)
   {
      var res = !IsApiRequest(context) && !IsVideoSegmentRequest(context);
      return res;
   }

   #endregion
}
