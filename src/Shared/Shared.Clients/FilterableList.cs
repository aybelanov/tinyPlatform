using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Clients;

/// <summary>
/// Represents a filterable list class for dynamic LINQ
/// </summary>
/// <typeparam name="T"></typeparam>
public class FilterableList<T> : List<T>, IFilterableList<T> where T : class
{
   /// <inheritdoc/>
   public int TotalCount { get; set; }

   /// <summary>
   /// Default Ctor
   /// </summary>
   public FilterableList() : base()
   {
   }

   /// <summary>
   /// Ctor
   /// </summary>
   /// <param name="source">List source</param>
   /// <param name="totalCount">Total count</param>
   /// <exception cref="ArgumentNullException"></exception>
   public FilterableList(IEnumerable<T> source, int? totalCount)
   {
      ArgumentNullException.ThrowIfNull(source);

      AddRange(source);
      TotalCount = totalCount ?? source.Count();
   }
}
