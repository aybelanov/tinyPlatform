using Microsoft.EntityFrameworkCore;
using Shared.Clients;
using Shared.Clients.Proto;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Auto = Hub.Core.Infrastructure.Mapper.AutoMapperConfiguration;

namespace Hub.Services.Clients;

/// <summary>
/// Extension methods for telemetry logic
/// </summary>
public static class Extensions
{
   /// <summary>
   /// Apply dynamic query from parsing query string
   /// </summary>
   /// <typeparam name="T">Type of collection</typeparam>
   /// <param name="query">Incomming query</param>
   /// <param name="filter">Dynamoc filter</param>
   /// <returns>Resulting query</returns>
   public static IQueryable<T> ApplyClientQuery<T>(this IQueryable<T> query, DynamicFilter filter) where T : BaseEntity
   {
      if (!string.IsNullOrWhiteSpace(filter.Query))
      {
         var lambda = DynamicExpressionParser.ParseLambda<IQueryable<T>, IQueryable<T>>(new ParsingConfig(), true, 
            filter.Query, filter.Ids, filter.UserIds, filter.DeviceIds, filter.SensorIds, filter.MonitorIds, filter.WidgetIds);

         var func = lambda.Compile();
         query = func(query);
      }

      return query;
   }

   /// <summary>
   /// Filters an exixting collection by the filter
   /// </summary>
   /// <typeparam name="T">Type of collection</typeparam>
   /// <param name="query">Incomming query</param>
   /// <param name="filter">Filter</param>
   /// <returns>Filtered collection</returns>
   public static IFilterableList<T> Filter<T>(this IQueryable<T> query, DynamicFilter filter) where T : BaseEntity
   {
      if (!string.IsNullOrWhiteSpace(filter.Filter))
         query = query.Where(filter.Filter);

      var totalCount = query.Count();

      if (filter.CountOnly)
      {
         var result = new FilterableList<T>();
         result.TotalCount = totalCount;
         return result;
      }

      if (!string.IsNullOrWhiteSpace(filter.OrderBy))
         query = query.OrderBy(filter.OrderBy);
      else
         query = query.OrderBy(x => x.Id);

      if (filter.Skip.HasValue)
         query = query.Skip(filter.Skip.Value);

      if (filter.Top.HasValue)
         query = query.Take(filter.Top.Value);

      return new FilterableList<T>(query, totalCount);
   }

   /// <summary>
   /// Filters an exixting collection by the dynamic filter
   /// </summary>
   /// <typeparam name="T">Type of collection</typeparam>
   /// <param name="query">Incomming query</param>
   /// <param name="filter">Filter</param>
   /// <param name="postQuery">query after Skip and take query (for performance)</param>
   /// <returns>Filtered collection</returns>
   public static async Task<IFilterableList<T>> FilterAsync<T>(this IQueryable<T> query, DynamicFilter filter, Func<IQueryable<T>, IQueryable<T>> postQuery = null)
      where T : BaseEntity
   {
      if (!string.IsNullOrWhiteSpace(filter.Filter))
         query = query.Where(filter.Filter);
      
      var totalCount = await query.CountAsync();

      if (filter.CountOnly || totalCount == 0)
      {
         var result = new FilterableList<T> { TotalCount = totalCount };
         return result;
      }

      if (!string.IsNullOrWhiteSpace(filter.OrderBy))
         query = query.OrderBy(filter.OrderBy);
      else
         query = query.OrderBy(x => x.Id);

      if (filter.Skip.HasValue)
         query = query.Skip(filter.Skip.Value);

      if (filter.Top.HasValue)
         query = query.Take(filter.Top.Value);

      if(postQuery != null)
         query = postQuery(query);  

      return new FilterableList<T>(query, totalCount);
   }

   /// <summary>
   /// Convert dynamic filter protobuf to DynamicFilter
   /// </summary>
   /// <param name="protoFilter"></param>
   /// <returns></returns>
   public static DynamicFilter ToFilter(this FilterProto protoFilter)
   {
      return Auto.Mapper.Map<DynamicFilter>(protoFilter);   
   }

   /// <summary>
   /// Gets date time intervals
   /// </summary>
   /// <param name="type">Datetime interval type</param>
   /// <param name="point">Reference point (for months, quarters, years)</param>
   /// <returns>INterval/subinterval tupple</returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public static (long Interval, long SubInterval) ToTimeIntervals(this TimeIntervalType? type, DateTime? point = null)
   {
      point ??= DateTime.UtcNow;
      var interval = type switch
      {
         TimeIntervalType.Second => TimeSpan.FromSeconds(1).Ticks,
         TimeIntervalType.Minute => TimeSpan.FromMinutes(1).Ticks,
         TimeIntervalType.Hour => TimeSpan.FromHours(1).Ticks,
         TimeIntervalType.Day => TimeSpan.FromDays(1).Ticks,
         TimeIntervalType.Week => TimeSpan.FromDays(7).Ticks,
         TimeIntervalType.Decade => TimeSpan.FromDays(10).Ticks,
         TimeIntervalType.Month => (point.Value - point.Value.AddMonths(-1)).Ticks,
         TimeIntervalType.Quarter => (point.Value - point.Value.AddMonths(-3)).Ticks,
         TimeIntervalType.Year => (point.Value - point.Value.AddYears(-1)).Ticks,
         _ => throw new ArgumentOutOfRangeException()
      };

      var subInterval = type switch
      {
         TimeIntervalType.Second => TimeSpan.FromMilliseconds(100).Ticks,
         TimeIntervalType.Minute => TimeSpan.FromSeconds(10).Ticks,
         TimeIntervalType.Hour => TimeSpan.FromMinutes(10).Ticks,
         TimeIntervalType.Day => TimeSpan.FromHours(1).Ticks,
         TimeIntervalType.Week => TimeSpan.FromDays(1).Ticks,
         TimeIntervalType.Decade => TimeSpan.FromDays(1).Ticks,
         TimeIntervalType.Month => TimeSpan.FromDays(1).Ticks,
         TimeIntervalType.Quarter => TimeSpan.FromDays(7).Ticks,//(point.Value - point.Value.AddMonths(-1)).Ticks,
         TimeIntervalType.Year => TimeSpan.FromDays(30).Ticks,//(point.Value - point.Value.AddMonths(-1)).Ticks,
         _ => throw new ArgumentOutOfRangeException()
      };

      return (interval, subInterval);
   }
}
