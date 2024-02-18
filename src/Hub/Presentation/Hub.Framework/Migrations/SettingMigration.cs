using Hub.Data.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hub.Web.Framework.Migrations;

/// <summary>
/// Setting migration
/// </summary>
public class SettingMigration : Migration
{
   /// <summary>Collect the UP migration expressions</summary>
   protected override void Up(MigrationBuilder migrationBuilder)
   {

   }

   /// <summary>Collect the DOWN migration expressions</summary>
   protected override void Down(MigrationBuilder migrationBuilder)
   {
      //add the downgrade logic if necessary 
   }
}