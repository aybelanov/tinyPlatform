using System.Linq;
using LinqToDB;
using LinqToDB.Data;

namespace Hub.Data;

/// <summary>
/// Represents temporary storage
/// </summary>
/// <typeparam name="T">Storage record mapping class</typeparam>
public class TempSqlDataStorage<T> : TempTable<T>, ITempDataStorage<T> where T : class
{
   #region Ctor

   /// <summary>
   /// Ctor
   /// </summary>
   /// <param name="storageName"></param>
   /// <param name="query"></param>
   /// <param name="dataConnection"></param>
   public TempSqlDataStorage(string storageName, IQueryable<T> query, IDataContext dataConnection)
       : base(dataConnection, storageName, query, tableOptions: TableOptions.NotSet | TableOptions.DropIfExists)
   {
      dataConnection.CloseAfterUse = true;
   }

   #endregion
}