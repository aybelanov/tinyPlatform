using System.Collections.Generic;

namespace Shared.Clients;

/// <summary>
/// Filterable list interface
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFilterableList<T> : IList<T>
{
   /// <summary>
   /// Total count of the collection
   /// </summary>
   int TotalCount { get; set; }
}
