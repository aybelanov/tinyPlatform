using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Shared.Clients;
using Shared.Clients.Proto;
using System;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;


namespace Clients.Dash.Services.EntityServices.Grpc;

/// <summary>
/// Represents a common service implementation
/// </summary>
public class CommonGrpcService : ICommonService
{
   #region fields

   private readonly CommonRpc.CommonRpcClient _grpcClient;
   private readonly IStaticCacheManager _staticCacheManager;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public CommonGrpcService(CommonRpc.CommonRpcClient grpcClient, IStaticCacheManager staticCacheManager)
   {
      _grpcClient = grpcClient;
      _staticCacheManager = staticCacheManager;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Gets user activity log for a user or a device 
   /// </summary>
   /// <param name="filter"></param>
   /// <returns>Activity log record collection</returns>
   public async Task<IFilterableList<ActivityLogRecord>> GetUserActivityLogAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var filterProto = Auto.Mapper.Map<FilterProto>(filter);
      var recordsProto = await _grpcClient.GetUserActivityLogAsync(filterProto);
      var records = Auto.Mapper.Map<FilterableList<ActivityLogRecord>>(recordsProto.Records);
      records.TotalCount = recordsProto.TotalCount ?? 0;

      return records;
   }


   /// <summary>
   /// Gets device activity log for a user or a device 
   /// </summary>
   /// <param name="filter"></param>
   /// <returns>Activity log record collection</returns>
   public async Task<IFilterableList<ActivityLogRecord>> GetDeviceActivityLogAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var filterProto = Auto.Mapper.Map<FilterProto>(filter);
      var recordsProto = await _grpcClient.GetDeviceActivityLogAsync(filterProto);
      var records = Auto.Mapper.Map<FilterableList<ActivityLogRecord>>(recordsProto.Records);
      records.TotalCount = recordsProto.TotalCount ?? 0;

      return records;
   }

   /// <summary>
   /// Gets users by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamoc filter</param>
   /// <returns>User collection</returns>
   public Task<IFilterableList<User>> GetUsersAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<User>.ByDynamicFilterCacheKey, "user", filter);
      return _staticCacheManager.Get(cacheKey, acquire);

      async Task<IFilterableList<User>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var userProtos = await _grpcClient.GetUsersAsync(filterProto);
         var users = Auto.Mapper.Map<FilterableList<User>>(userProtos.Users);
         users.TotalCount = userProtos.TotalCount ?? 0;

         return users;
      }
   }

   /// <summary>
   /// Gets user select items by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamoc filter</param>
   /// <returns>User select item collection</returns>
   public Task<IFilterableList<UserSelectItem>> GetUserSelecItemsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<UserSelectItem>.ByDynamicFilterCacheKey, "all", filter);
      return _staticCacheManager.Get(cacheKey, acquire);

      async Task<IFilterableList<UserSelectItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var userProtos = await _grpcClient.GetUserSelectItemsAsync(filterProto);
         var users = Auto.Mapper.Map<FilterableList<UserSelectItem>>(userProtos.Users);
         users.TotalCount = userProtos.TotalCount ?? 0;

         return users;
      }
   }

   /// <summary>
   /// Checks username availability
   /// </summary>
   /// <param name="userName">Username</param>
   /// <returns>Check result</returns>
   public async Task<CommonResponse> CheckUserNameAvailabilityAsync(string userName)
   {
      var response = await _grpcClient.CheckUserNameAvalabilityAsync(new() { SystemName = userName });
      return response;
   }

   #endregion
}
