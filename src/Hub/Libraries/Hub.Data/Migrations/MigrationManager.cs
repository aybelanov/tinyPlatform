using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hub.Core;

namespace Hub.Data.Migrations
{
   /// <summary>
   /// Represents the migration manager
   /// </summary>
   public partial class MigrationManager : IMigrationManager
   {
      #region Fields

      private readonly AppDbContext _dataProvider;

      #endregion

      #region Ctor

      /// <summary>
      /// IoC Ctor
      /// </summary>
      public MigrationManager(AppDbContext dataProvider)
      {
         _dataProvider = dataProvider;
      }

      #endregion

      #region Utils

      ///// <summary>
      ///// Returns the instances for found types implementing FluentMigrator.IMigration which ready to Up process
      ///// </summary>
      ///// <param name="assembly">Assembly to find migrations</param>
      ///// <param name="migrationProcessType">Type of migration process; pass MigrationProcessType.NoMatter to load all migrations</param>
      ///// <returns>The instances for found types implementing FluentMigrator.IMigration</returns>
      //protected virtual IEnumerable<IMigrationInfo> GetUpMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.NoMatter)
      //{
      //   var migrations = _filteringMigrationSource
      //       .GetMigrations(t =>
      //       {
      //          var migrationAttribute = t.GetCustomAttribute<AppMigrationAttribute>();

      //          if (migrationAttribute is null || _versionLoader.Value.VersionInfo.HasAppliedMigration(migrationAttribute.Version))
      //             return false;

      //          if (migrationAttribute.TargetMigrationProcess != MigrationProcessType.NoMatter &&
      //                 migrationProcessType != MigrationProcessType.NoMatter &&
      //                 migrationProcessType != migrationAttribute.TargetMigrationProcess)
      //             return false;

      //          return assembly == null || t.Assembly == assembly;

      //       }) ?? Enumerable.Empty<IMigration>();

      //   return migrations
      //       .Select(m => _migrationRunnerConventions.GetMigrationInfoForMigration(m))
      //       .OrderBy(migration => migration.Version);
      //}

      ///// <summary>
      ///// Returns the instances for found types implementing FluentMigrator.IMigration which ready to Down process
      ///// </summary>
      ///// <param name="assembly">Assembly to find migrations</param>
      ///// <param name="migrationProcessType">Type of migration process; pass MigrationProcessType.NoMatter to load all migrations</param>
      ///// <returns>The instances for found types implementing FluentMigrator.IMigration</returns>
      //protected virtual IEnumerable<IMigrationInfo> GetDownMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.NoMatter)
      //{
      //   var migrations = _filteringMigrationSource
      //       .GetMigrations(t =>
      //       {
      //          var migrationAttribute = t.GetCustomAttribute<AppMigrationAttribute>();

      //          if (migrationAttribute is null || !_versionLoader.Value.VersionInfo.HasAppliedMigration(migrationAttribute.Version))
      //             return false;

      //          if (migrationAttribute.TargetMigrationProcess != MigrationProcessType.NoMatter &&
      //                 migrationProcessType != MigrationProcessType.NoMatter &&
      //                 migrationProcessType != migrationAttribute.TargetMigrationProcess)
      //             return false;

      //          return assembly == null || t.Assembly == assembly;
      //       }) ?? Enumerable.Empty<IMigration>();

      //   return migrations
      //       .Select(m => _migrationRunnerConventions.GetMigrationInfoForMigration(m))
      //       .OrderBy(migration => migration.Version);
      //}

      #endregion

      #region Methods

      /// <summary>
      /// Executes an Up for all found unapplied migrations
      /// </summary>
      /// <param name="assembly">Assembly to find migrations</param>
      /// <param name="migrationProcessType">Type of migration process</param>
      public virtual void ApplyUpMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.Installation)
      {
         throw new NotImplementedException();

//         if (assembly is null)
//            throw new ArgumentNullException(nameof(assembly));

//         foreach (var migrationInfo in GetUpMigrations(assembly, migrationProcessType))
//         {
//            _migrationRunner.Up(migrationInfo.Migration);

//#if DEBUG
//            if (!string.IsNullOrEmpty(migrationInfo.Description) &&
//                migrationInfo.Description.StartsWith(string.Format(AppMigrationDefaults.UpdateMigrationDescriptionPrefix, AppVersion.FULL_VERSION)))
//               continue;
//#endif
//            _versionLoader.Value
//                .UpdateVersionInfo(migrationInfo.Version, migrationInfo.Description ?? migrationInfo.Migration.GetType().Name);
//         }
      }

      /// <summary>
      /// Executes all found (and applied) migrations
      /// </summary>
      /// <param name="assembly">Assembly to find the migration</param>
      public void ApplyDownMigrations(Assembly assembly)
      {
         throw new NotImplementedException();
         //if (assembly is null)
         //   throw new ArgumentNullException(nameof(assembly));

         //foreach (var migrationInfo in GetDownMigrations(assembly).Reverse())
         //{
         //   _migrationRunner.Down(migrationInfo.Migration);
         //   _versionLoader.Value.DeleteVersion(migrationInfo.Version);
         //}
      }

      #endregion
   }
}