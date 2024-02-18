﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Hub.Core.Infrastructure;

namespace Hub.Services.Plugins
{
   /// <summary>
   /// Represents an information about plugins
   /// </summary>
   public partial class PluginsInfo : IPluginsInfo
   {
      #region Fields

      private const string OBSOLETE_FIELD = "Obsolete field, using only for compatibility";
      private List<string> _installedPluginNames = new();
      private IList<PluginDescriptorBaseInfo> _installedPlugins = new List<PluginDescriptorBaseInfo>();

      /// <summary> Application file provider </summary>
      protected readonly IAppFileProvider _fileProvider;

      #endregion

      #region Utilities

      /// <summary>
      /// Get system names of installed plugins from obsolete file
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the list of plugin system names
      /// </returns>
      protected virtual async Task<IList<string>> GetObsoleteInstalledPluginNamesAsync()
      {
         //check whether file exists
         var filePath = _fileProvider.MapPath(AppPluginDefaults.InstalledPluginsFilePath);
         if (!_fileProvider.FileExists(filePath))
         {
            //if not, try to parse the file that was used in previous application versions
            filePath = _fileProvider.MapPath(AppPluginDefaults.ObsoleteInstalledPluginsFilePath);
            if (!_fileProvider.FileExists(filePath))
               return new List<string>();

            //get plugin system names from the old txt file
            var pluginSystemNames = new List<string>();
            using (var reader = new StringReader(await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8)))
            {
               string pluginName;
               while ((pluginName = await reader.ReadLineAsync()) != null)
                  if (!string.IsNullOrWhiteSpace(pluginName))
                     pluginSystemNames.Add(pluginName.Trim());
            }

            //and delete the old one
            _fileProvider.DeleteFile(filePath);

            return pluginSystemNames;
         }

         var text = await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
         if (string.IsNullOrEmpty(text))
            return new List<string>();

         //delete the old file
         _fileProvider.DeleteFile(filePath);

         //get plugin system names from the JSON file
         return JsonConvert.DeserializeObject<IList<string>>(text);
      }

      /// <summary>
      /// Deserialize PluginInfo from json
      /// </summary>
      /// <param name="json">Json data of PluginInfo</param>
      /// <returns>True if data are loaded, otherwise False</returns>
      protected virtual bool DeserializePluginInfo(string json)
      {
         var pluginsInfo = JsonConvert.DeserializeObject<PluginsInfo>(json);

         InstalledPluginNames = pluginsInfo.InstalledPluginNames;
         InstalledPlugins = pluginsInfo.InstalledPlugins;
         PluginNamesToUninstall = pluginsInfo.PluginNamesToUninstall;
         PluginNamesToDelete = pluginsInfo.PluginNamesToDelete;
         PluginNamesToInstall = pluginsInfo.PluginNamesToInstall;

         return InstalledPlugins.Any() || PluginNamesToUninstall.Any() || PluginNamesToDelete.Any() ||
                PluginNamesToInstall.Any();
      }

      #endregion

      #region Ctor

      /// <summary>
      /// IoC Ctor
      /// </summary>
      public PluginsInfo(IAppFileProvider fileProvider)
      {
         _fileProvider = fileProvider;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Save plugins info to the file
      /// </summary>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task SaveAsync()
      {
         //save the file
         var filePath = _fileProvider.MapPath(AppPluginDefaults.PluginsInfoFilePath);
         var text = JsonConvert.SerializeObject(this, Formatting.Indented);
         await _fileProvider.WriteAllTextAsync(filePath, text, Encoding.UTF8);
      }

      /// <summary>
      /// Save plugins info to the file
      /// </summary>
      public virtual void Save()
      {
         //save the file
         var filePath = _fileProvider.MapPath(AppPluginDefaults.PluginsInfoFilePath);
         var text = JsonConvert.SerializeObject(this, Formatting.Indented);
         _fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
      }

      /// <summary>
      /// Get plugins info
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the rue if data are loaded, otherwise False
      /// </returns>
      public virtual async Task<bool> LoadPluginInfoAsync()
      {
         //check whether plugins info file exists
         var filePath = _fileProvider.MapPath(AppPluginDefaults.PluginsInfoFilePath);
         if (!_fileProvider.FileExists(filePath))
         {
            //file doesn't exist, so try to get only installed plugin names from the obsolete file
            _installedPluginNames.AddRange(await GetObsoleteInstalledPluginNamesAsync());

            //and save info into a new file if need
            if (_installedPluginNames.Any())
               await SaveAsync();
         }

         //try to get plugin info from the JSON file
         var text = _fileProvider.FileExists(filePath)
             ? await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8)
             : string.Empty;
         return !string.IsNullOrEmpty(text) && DeserializePluginInfo(text);
      }

      /// <summary>
      /// Create copy from another instance of IPluginsInfo interface
      /// </summary>
      /// <param name="pluginsInfo">Plugins info</param>
      public virtual void CopyFrom(IPluginsInfo pluginsInfo)
      {
         InstalledPlugins = pluginsInfo.InstalledPlugins?.ToList() ?? new List<PluginDescriptorBaseInfo>();
         PluginNamesToUninstall = pluginsInfo.PluginNamesToUninstall?.ToList() ?? new List<string>();
         PluginNamesToDelete = pluginsInfo.PluginNamesToDelete?.ToList() ?? new List<string>();
         PluginNamesToInstall = pluginsInfo.PluginNamesToInstall?.ToList() ??
                                new List<(string SystemName, Guid? UserGuid)>();
         AssemblyLoadedCollision = pluginsInfo.AssemblyLoadedCollision?.ToList();
         PluginDescriptors = pluginsInfo.PluginDescriptors?.ToList();
         IncompatiblePlugins = pluginsInfo.IncompatiblePlugins?.ToList();
      }

      #endregion

      #region Properties

      // TODO remove obsolete plugin support 
      /// <summary>
      /// Gets or sets the list of all installed plugin names
      /// </summary>
      public virtual IList<string> InstalledPluginNames
      {
         get
         {
            if (_installedPlugins.Any())
               _installedPluginNames.Clear();

            return _installedPluginNames.Any() ? _installedPluginNames : new List<string> { OBSOLETE_FIELD };
         }
         set
         {
            if (value?.Any() ?? false)
               _installedPluginNames = value.ToList();
         }
      }

      /// <summary>
      /// Gets or sets the list of all installed plugin
      /// </summary>
      public virtual IList<PluginDescriptorBaseInfo> InstalledPlugins
      {
         get
         {
            if ((_installedPlugins?.Any() ?? false) || !_installedPluginNames.Any())
               return _installedPlugins;

            if (PluginDescriptors?.Any() ?? false)
               _installedPlugins = PluginDescriptors
                   .Where(pd => _installedPluginNames.Any(pn =>
                       pn.Equals(pd.SystemName, StringComparison.InvariantCultureIgnoreCase)))
                   .Select(pd => pd as PluginDescriptorBaseInfo).ToList();
            else
               return _installedPluginNames
                   .Where(name => !name.Equals(OBSOLETE_FIELD, StringComparison.InvariantCultureIgnoreCase))
                   .Select(systemName => new PluginDescriptorBaseInfo { SystemName = systemName }).ToList();

            return _installedPlugins;
         }
         set => _installedPlugins = value;
      }

      /// <summary>
      /// Gets or sets the list of plugin names which will be uninstalled
      /// </summary>
      public virtual IList<string> PluginNamesToUninstall { get; set; } = new List<string>();

      /// <summary>
      /// Gets or sets the list of plugin names which will be deleted
      /// </summary>
      public virtual IList<string> PluginNamesToDelete { get; set; } = new List<string>();

      /// <summary>
      /// Gets or sets the list of plugin names which will be installed
      /// </summary>
      public virtual IList<(string SystemName, Guid? UserGuid)> PluginNamesToInstall { get; set; } =
          new List<(string SystemName, Guid? UserGuid)>();

      /// <summary>
      /// Gets or sets the list of plugin names which are not compatible with the current version
      /// </summary>
      [JsonIgnore]
      public virtual IList<string> IncompatiblePlugins { get; set; }

      /// <summary>
      /// Gets or sets the list of assembly loaded collisions
      /// </summary>
      [JsonIgnore]
      public virtual IList<PluginLoadedAssemblyInfo> AssemblyLoadedCollision { get; set; }

      /// <summary>
      /// Gets or sets a collection of plugin descriptors of all deployed plugins
      /// </summary>
      [JsonIgnore]
      public virtual IList<PluginDescriptor> PluginDescriptors { get; set; }

      #endregion
   }
}