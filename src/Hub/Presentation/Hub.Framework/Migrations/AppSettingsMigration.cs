using Hub.Core.Configuration;
using Hub.Core.Infrastructure;
using Hub.Web.Framework.Configuration;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;

namespace Hub.Web.Framework.Migrations;

#pragma warning disable CS1591
public class AppSettingsMigration : Migration
{
   protected override void Up(MigrationBuilder migrationBuilder)
   {
      var fileProvider = EngineContext.Current.Resolve<IAppFileProvider>();

      var rootDir = fileProvider.MapPath("~/");

      var config = new WebOptimizerConfig
      {
         EnableTagHelperBundling = false,
         EnableCaching = true,
         EnableDiskCache = true,
         AllowEmptyBundle = true,
         CacheDirectory = fileProvider.Combine(rootDir, @"wwwroot\bundles")
      };

      AppSettingsHelper.SaveAppSettings(new List<IConfig> { config }, fileProvider);
   }

   protected override void Down(MigrationBuilder migrationBuilder)
   {
      //add the downgrade logic if necessary 
   }
}
#pragma warning restore CS1591