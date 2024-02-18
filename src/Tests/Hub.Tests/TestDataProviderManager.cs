using Hub.Core;
using Hub.Data.DataProviders;
using Hub.Core.Infrastructure;
using Hub.Data;
using Hub.Data.Configuration;

namespace Hub.Tests;

/// <summary>
/// Represents the data provider manager
/// </summary>
public partial class TestDataProviderManager : IDataProviderManager
{
   #region Properties

   /// <summary>
   /// Gets data provider
   /// </summary>
   public IAppDataProvider DataProvider
   {
      get
      {
         return Singleton<DataConfig>.Instance.DataProvider switch
         {
            DataProviderType.SqlServer => new MsSqlAppDataProvider(),
            DataProviderType.MySql => new MySqlAppDataProvider(),
            DataProviderType.PostgreSQL => new PostgreSqlDataProvider(),
            DataProviderType.Unknown => new SqLiteAppDataProvider(),
            _ => throw new AppException($"Unknown [{Singleton<DataConfig>.Instance.DataProvider}] DataProvider")
         };
      }
   }

   #endregion
}
