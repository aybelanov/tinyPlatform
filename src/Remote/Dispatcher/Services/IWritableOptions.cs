using Microsoft.Extensions.Options;
using System;

namespace Devices.Dispatcher.Services;

/// <summary>
/// App setting writer
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
{
   /// <summary>
   /// Updates appsettings.json
   /// </summary>
   /// <param name="applyChanges"></param>
   void Update(Action<T> applyChanges);
}
