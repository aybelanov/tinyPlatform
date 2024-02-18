using System;
using System.Data.Common;
using System.Threading;
using Hub.Core;
using Hub.Core.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Npgsql;

namespace Hub.Data;

/// <summary>
/// Represents the data provider manager
/// </summary>
public static class DataProviderManager
{
   #region Utilities

   #region Connection string builders

   /// <summary>
   /// Build the connection string
   /// </summary>
   /// <param name="appConnectionString">Connection string info</param>
   /// <returns>Connection string</returns>
   private static string BuildSqlConnectionString(IAppConnectionStringInfo appConnectionString)
   {
      if (appConnectionString is null)
         throw new ArgumentNullException(nameof(appConnectionString));

      var builder = new SqlConnectionStringBuilder
      {
         DataSource = appConnectionString.ServerName,
         InitialCatalog = appConnectionString.DatabaseName,
         PersistSecurityInfo = false,
         IntegratedSecurity = appConnectionString.IntegratedSecurity,
         TrustServerCertificate = true
      };

      if (!appConnectionString.IntegratedSecurity)
      {
         builder.UserID = appConnectionString.Username;
         builder.Password = appConnectionString.Password;
      }

      return builder.ConnectionString;
   }

   /// <summary>
   /// Build the connection string
   /// </summary>
   /// <param name="appConnectionString">Connection string info</param>
   /// <returns>Connection string</returns>
   private static string BuildMySqlConnectionString(IAppConnectionStringInfo appConnectionString)
   {
      if (appConnectionString is null)
         throw new ArgumentNullException(nameof(appConnectionString));

      if (appConnectionString.IntegratedSecurity)
         throw new AppException("Data provider supports connection only with login and password");

      var builder = new MySqlConnectionStringBuilder
      {
         Server = appConnectionString.ServerName,
         //Cast DatabaseName to lowercase to avoid case-sensitivity problems
         Database = appConnectionString.DatabaseName.ToLowerInvariant(),
         AllowUserVariables = true,
         UserID = appConnectionString.Username,
         Password = appConnectionString.Password,
      };

      return builder.ConnectionString;
   }

   /// <summary>
   /// Build the connection string
   /// </summary>
   /// <param name="appConnectionString">Connection string info</param>
   /// <returns>Connection string</returns>
   private static string BuildPostgreSqlConnectionString(IAppConnectionStringInfo appConnectionString)
   {
      if (appConnectionString is null)
         throw new ArgumentNullException(nameof(appConnectionString));

      if (appConnectionString.IntegratedSecurity)
         throw new AppException("Data provider supports connection only with login and password");

      var builder = new NpgsqlConnectionStringBuilder
      {
         Host = appConnectionString.ServerName,
         //Cast DatabaseName to lowercase to avoid case-sensitivity problems
         Database = appConnectionString.DatabaseName.ToLowerInvariant(),
         Username = appConnectionString.Username,
         Password = appConnectionString.Password,
      };

      return builder.ConnectionString;
   }

   /// <summary>
   /// Build the connection string
   /// </summary>
   /// <param name="appConnectionString">Connection string info</param>
   /// <returns>Connection string</returns>
   private static string BuildSQLiteConnectionString(IAppConnectionStringInfo appConnectionString)
   {
      if (appConnectionString is null)
         throw new ArgumentNullException(nameof(appConnectionString));

      if (appConnectionString.IntegratedSecurity)
         throw new AppException("Data provider supports connection only with password");

      var builder = new SqliteConnectionStringBuilder
      {
         DataSource = CommonHelper.DefaultFileProvider.MapPath($"~/App_Data/{appConnectionString.DatabaseName}.sqlite"),
         Password = appConnectionString.Password,
         Mode = SqliteOpenMode.ReadWriteCreate,
         Cache = SqliteCacheMode.Default
      };

      return builder.ConnectionString;
   }

   #endregion

   #region Create Databases

   private static void CreateSQLiteDatabase(string collation, int triesToConnect)
   {
      if (DatabaseExists())
         return;

      var settings = DataSettingsManager.LoadSettings();

      if (string.IsNullOrEmpty(settings.ConnectionString))
         throw new ArgumentNullException(nameof(settings.ConnectionString));

      var builder = new SqliteConnectionStringBuilder(settings.ConnectionString);

      using var connection = new SqliteConnection(builder.ConnectionString);
      connection.Open();

      using var command = connection.CreateCommand();
      command.CommandText = "PRAGMA journal_mode=WAL;";

      using var transaction = connection.BeginTransaction();
      command.ExecuteNonQuery();
      transaction.Commit();

      //sometimes on slow servers (hosting) there could be situations when database requires some time to be created.
      //but we have already started creation of tables and sample data.
      //as a result there is an exception thrown and the installation process cannot continue.
      //that's why we are in a cycle of "triesToConnect" times trying to connect to a database with a delay of one second.
      for (var i = 0; i <= triesToConnect; i++)
      {
         if (i == triesToConnect)
            throw new Exception("Unable to connect to the new database. Please try one more time");

         if (!DatabaseExists())
            Thread.Sleep(1000);
         else
            break;
      }
   }


   private static void CreatePostgreSqlDatabase(string collation, int triesToConnect)
   {
      if (DatabaseExists())
         return;

      var settings = DataSettingsManager.LoadSettings();

      if (string.IsNullOrEmpty(settings.ConnectionString))
         throw new ArgumentNullException(nameof(settings.ConnectionString));

      var builder = new NpgsqlConnectionStringBuilder(settings.ConnectionString);

      //gets database name
      var databaseName = builder.Database;

      //now create connection string to 'postgres' - default administrative connection database.
      builder.Database = "postgres";

      using (var connection = new NpgsqlConnection(builder.ConnectionString))
      {
         var query = $"CREATE DATABASE \"{databaseName}\" WITH OWNER = '{builder.Username}'";
         if (!string.IsNullOrWhiteSpace(collation))
            query = $"{query} LC_COLLATE = '{collation}'";

         var command = connection.CreateCommand();
         command.CommandText = query;
         command.Connection.Open();

         command.ExecuteNonQuery();
      }

      //try connect
      if (triesToConnect <= 0)
         return;

      //sometimes on slow servers (hosting) there could be situations when database requires some time to be created.
      //but we have already started creation of tables and sample data.
      //as a result there is an exception thrown and the installation process cannot continue.
      //that's why we are in a cycle of "triesToConnect" times trying to connect to a database with a delay of one second.
      for (var i = 0; i <= triesToConnect; i++)
      {
         if (i == triesToConnect)
            throw new Exception("Unable to connect to the new database. Please try one more time");

         if (!DatabaseExists())
         {
            Thread.Sleep(1000);
         }
         else
         {
            builder.Database = databaseName;
            using var connection = new NpgsqlConnection(builder.ConnectionString);
            var command = connection.CreateCommand();
            command.CommandText = "CREATE EXTENSION IF NOT EXISTS citext; CREATE EXTENSION IF NOT EXISTS pgcrypto;";
            command.Connection.Open();
            command.ExecuteNonQuery();
            connection.ReloadTypes();

            break;
         }
      }
   }

   private static void CreateMySqlDatabase(string collation, int triesToConnect)
   {
      if (DatabaseExists())
         return;

      var settings = DataSettingsManager.LoadSettings();

      if (string.IsNullOrEmpty(settings.ConnectionString))
         throw new ArgumentNullException(nameof(settings.ConnectionString));

      var builder = new MySqlConnectionStringBuilder(settings.ConnectionString);

      //gets database name
      var databaseName = builder.Database;

      //now create connection string to 'master' database. It always exists.
      builder.Database = null;

      using (var connection = new MySqlConnection(builder.ConnectionString))
      {
         var query = $"CREATE DATABASE IF NOT EXISTS {databaseName}";
         if (!string.IsNullOrWhiteSpace(collation))
            query = $"{query} COLLATE {collation}";

         var command = connection.CreateCommand();
         command.CommandText = query;
         command.Connection.Open();

         command.ExecuteNonQuery();
      }

      //try connect
      if (triesToConnect <= 0)
         return;

      //sometimes on slow servers (hosting) there could be situations when database requires some time to be created.
      //but we have already started creation of tables and sample data.
      //as a result there is an exception thrown and the installation process cannot continue.
      //that's why we are in a cycle of "triesToConnect" times trying to connect to a database with a delay of one second.
      for (var i = 0; i <= triesToConnect; i++)
      {
         if (i == triesToConnect)
            throw new Exception("Unable to connect to the new database. Please try one more time");

         if (!DatabaseExists())
            Thread.Sleep(1000);
         else
            break;
      }
   }

   private static void CreateMsSqlDatabase(string collation, int triesToConnect)
   {
      if (DatabaseExists())
         return;

      var settings = DataSettingsManager.LoadSettings();

      if (string.IsNullOrEmpty(settings.ConnectionString))
         throw new ArgumentNullException(nameof(settings.ConnectionString));

      var builder = new SqlConnectionStringBuilder(settings.ConnectionString);

      //gets database name
      var databaseName = builder.InitialCatalog;

      //now create connection string to 'master' dabatase. It always exists.
      builder.InitialCatalog = "master";

      using (var connection = new SqlConnection(builder.ConnectionString))
      {
         var query = $"CREATE DATABASE [{databaseName}]";
         if (!string.IsNullOrWhiteSpace(collation))
            query = $"{query} COLLATE {collation}";

         var command = connection.CreateCommand();
         command.CommandText = query;
         command.Connection.Open();

         command.ExecuteNonQuery();
      }

      //try connect
      if (triesToConnect <= 0)
         return;

      //sometimes on slow servers (hosting) there could be situations when database requires some time to be created.
      //but we have already started creation of tables and sample data.
      //as a result there is an exception thrown and the installation process cannot continue.
      //that's why we are in a cycle of "triesToConnect" times trying to connect to a database with a delay of one second.
      for (var i = 0; i <= triesToConnect; i++)
      {
         if (i == triesToConnect)
            throw new Exception("Unable to connect to the new database. Please try one more time");

         if (!DatabaseExists())
            Thread.Sleep(1000);
         else
            break;
      }
   }

   #endregion


   #endregion

   #region Methods

   /// <summary>
   /// Build the connection string
   /// </summary>
   /// <param name="appConnectionString">Connection string info</param>
   /// <param name="dataProviderType">Data provider type</param>
   /// <returns>Connection string</returns>
   public static string BuildConnectionString(DataProviderType dataProviderType, IAppConnectionStringInfo appConnectionString) => (dataProviderType) switch
   {
      DataProviderType.SqlServer => BuildSqlConnectionString(appConnectionString),
      DataProviderType.MySql => BuildMySqlConnectionString(appConnectionString),
      DataProviderType.PostgreSQL => BuildPostgreSqlConnectionString(appConnectionString),
      DataProviderType.SQLite => BuildSQLiteConnectionString(appConnectionString),
      _ => throw new AppException($"Not supported data provider name: '{dataProviderType}'")
   };

   /// <summary>
   /// Create the database
   /// </summary>
   /// <param name="collation">Collation</param>
   /// <param name="dataProviderType">Data provider type</param>
   /// <param name="triesToConnect">Count of tries to connect to the database after creating; set 0 if no need to connect after creating</param>
   public static void CreateDatabase(DataProviderType dataProviderType, string collation, int triesToConnect = 10)
   {
      switch (dataProviderType)
      {
         case DataProviderType.SqlServer:
            CreateMsSqlDatabase(collation, triesToConnect);
            break;

         case DataProviderType.MySql:
            CreateMySqlDatabase(collation, triesToConnect);
            break;

         case DataProviderType.PostgreSQL:
            CreatePostgreSqlDatabase(collation, triesToConnect);
            break;

         case DataProviderType.SQLite:
            CreateSQLiteDatabase(collation, triesToConnect);
            break;

         default:
            throw new AppException($"Not supported data provider name: '{dataProviderType}'");
      }
   }

   /// <summary>
   /// Initialize database
   /// </summary>
   public static void InitializeDatabase()
   {
      using var scope = EngineContext.Current.Resolve<IServiceScopeFactory>().CreateScope();
      using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); 
      dbContext.Database.Migrate();
   }

   /// <summary>
   /// Checks if the specified database exists, returns true if database exists
   /// </summary>
   /// <returns>Returns true if the database exists.</returns>
   public static bool DatabaseExists()
   {
      try
      {
         var settings = DataSettingsManager.LoadSettings();
        
         using DbConnection connection = (settings.DataProvider) switch
         {
            DataProviderType.SqlServer => new SqlConnection(settings.ConnectionString),
            DataProviderType.MySql => new MySqlConnection(settings.ConnectionString),
            DataProviderType.PostgreSQL => new NpgsqlConnection(settings.ConnectionString),
            DataProviderType.SQLite => new SqliteConnection(settings.ConnectionString),
            _ => throw new AppException($"Not supported data provider name: '{settings.DataProvider}'")
         };

         //just try to connect
         connection.Open();
         
         return true;
      }
      catch
      {
         return false;
      }
   }

   #endregion
}
