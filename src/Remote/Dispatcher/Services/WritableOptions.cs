using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Devices.Dispatcher.Services;

// https://stackoverflow.com/questions/40970944/how-to-update-values-into-appsetting-json
// https://learn.microsoft.com/en-us/answers/questions/609232/how-to-save-the-updates-i-made-to-appsettings-conf.html
/// <summary>
/// Represents a application setting writer implementation
/// </summary>
/// <typeparam name="T"></typeparam>
public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
   #region fields

   private readonly IWebHostEnvironment _environment;
   private readonly IOptionsMonitor<T> _options;
   private readonly IConfigurationRoot _configuration;
   private readonly string _section;
   private readonly string _file;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public WritableOptions(IWebHostEnvironment environment, IOptionsMonitor<T> options, IConfigurationRoot configuration, string section, string file)
   {
      _environment = environment;
      _options = options;
      _configuration = configuration;
      _section = section;
      _file = file;
   }

   #endregion

   /// <summary>
   /// Updates appsettings.json
   /// </summary>
   /// <param name="applyChanges">Changes delegate</param>
   public void Update(Action<T> applyChanges)
   {
      var fileProvider = _environment.ContentRootFileProvider;

      var fileInfo = fileProvider.GetFileInfo(_file);

      var physicalPath = fileInfo.PhysicalPath;

      var jObject = JsonSerializer.Deserialize<JsonObject>(File.ReadAllText(physicalPath));

      var sectionObject = jObject.TryGetPropertyValue(_section, out var section) ? JsonSerializer.Deserialize<T>(section.ToString()) : Value ?? new T();

      applyChanges(sectionObject);

      jObject[_section] = JsonNode.Parse(JsonSerializer.Serialize(sectionObject));

      File.WriteAllText(physicalPath, JsonSerializer.Serialize(jObject, new JsonSerializerOptions { WriteIndented = true }));

      _configuration.Reload();
   }

   /// <summary>
   /// option value
   /// </summary>
   public T Value => _options.CurrentValue;

   /// <summary>
   /// Gets an option object
   /// </summary>
   /// <param name="name">Option name</param>
   /// <returns></returns>
   public T Get(string name) => _options.Get(name);
}
