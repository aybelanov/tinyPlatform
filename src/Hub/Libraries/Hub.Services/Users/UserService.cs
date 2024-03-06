using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Users;
using Hub.Core.Infrastructure;
using Hub.Data;
using Hub.Services.Clients;
using Hub.Services.Common;
using Hub.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;
using Shared.Clients.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Users;

/// <summary>
/// User service
/// </summary>
public partial class UserService : IUserService
{
   #region Fields

   private readonly UserSettings _userSettings;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly AppDbContext _dataProvider;
   private readonly IRepository<Address> _userAddressRepository;
   private readonly IRepository<User> _userRepository;
   private readonly IRepository<UserAddress> _userAddressMappingRepository;
   private readonly IRepository<UserUserRole> _userUserRoleMappingRepository;
   private readonly IRepository<UserDevice> _userDeviceRepository;
   private readonly IRepository<Device> _deviceRepository;
   private readonly IRepository<UserPassword> _userPasswordRepository;
   private readonly IRepository<UserRole> _userRoleRepository;
   private readonly IRepository<GenericAttribute> _gaRepository;
   private readonly IRepository<UserMonitor> _userMonitorRepository;
   private readonly IRepository<Monitor> _monitorRepository;
   private readonly ICommunicator _communicator;
   private readonly IStaticCacheManager _staticCacheManager;

   #endregion

   #region Ctor

   /// <summary> IoC Ctor </summary>
   public UserService(UserSettings userSettings,
       IGenericAttributeService genericAttributeService,
       AppDbContext dataProvider,
       IRepository<Address> userAddressRepository,
       IRepository<User> userRepository,
       ICommunicator communicator,
       IRepository<UserAddress> userAddressMappingRepository,
       IRepository<UserUserRole> userUserRoleMappingRepository,
       IRepository<UserPassword> userPasswordRepository,
       IRepository<UserDevice> userDeviceRepository,
       IRepository<UserRole> userRoleRepository,
       IRepository<UserMonitor> userMonitorRepository,
       IRepository<Monitor> monitorRepository,
       IRepository<Device> deviceRepository,
       IRepository<GenericAttribute> gaRepository,
       IStaticCacheManager staticCacheManager)
   {
      _userSettings = userSettings;
      _genericAttributeService = genericAttributeService;
      _dataProvider = dataProvider;
      _userAddressRepository = userAddressRepository;
      _userRepository = userRepository;
      _userAddressMappingRepository = userAddressMappingRepository;
      _userDeviceRepository = userDeviceRepository;
      _userUserRoleMappingRepository = userUserRoleMappingRepository;
      _userPasswordRepository = userPasswordRepository;
      _deviceRepository = deviceRepository;
      _userRoleRepository = userRoleRepository;
      _gaRepository = gaRepository;
      _staticCacheManager = staticCacheManager;
      _communicator = communicator;
      _userMonitorRepository = userMonitorRepository;
      _monitorRepository = monitorRepository;
   }

   #endregion

   #region Methods

   #region Users

   /// <summary>
   /// Gets all users
   /// </summary>
   /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
   /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
   /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
   /// <param name="deviceIds">Device identifiers</param>
   /// <param name="email">Email; null to load all users</param>
   /// <param name="username">Username; null to load all users</param>
   /// <param name="firstName">First name; null to load all users</param>
   /// <param name="lastName">Last name; null to load all users</param>
   /// <param name="dayOfBirth">Day of birth; 0 to load all users</param>
   /// <param name="monthOfBirth">Month of birth; 0 to load all users</param>
   /// <param name="company">Company; null to load all users</param>
   /// <param name="phone">Phone; null to load all users</param>
   /// <param name="zipPostalCode">Phone; null to load all users</param>
   /// <param name="ipAddress">IP address; null to load all users</param>
   /// <param name="lastActivityFromUtc">Last activity datetime from</param>
   /// <param name="lastActivityToUtc">Last activity datetime to</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. GetTable to "true" if you don't want to load data from database</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the users
   /// </returns>
   public virtual async Task<IPagedList<User>> GetAllUsersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null, long[] userRoleIds = null,
      long[] deviceIds = null, string email = null, string username = null, string firstName = null, string lastName = null, int dayOfBirth = 0, int monthOfBirth = 0,
      string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null, DateTime? lastActivityFromUtc = null,
      DateTime? lastActivityToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
   {
      var users = await _userRepository.GetAllPagedAsync(query =>
      {
         if (createdFromUtc.HasValue)
            query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);

         if (createdToUtc.HasValue)
            query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);

         query = query.Where(c => !c.IsDeleted);

         if (userRoleIds != null && userRoleIds.Length > 0)
         {
            query = query.Join(_userUserRoleMappingRepository.Table, x => x.Id, y => y.UserId,
                       (x, y) => new { User = x, Mapping = y })
                   .Where(z => userRoleIds.Contains(z.Mapping.UserRoleId))
                   .Select(z => z.User)
                   .Distinct();
         }

         if (deviceIds is not null)
         {
            query = from u in query
                    join ud in _userDeviceRepository.Table on u.Id equals ud.UserId
                    where deviceIds.Contains(ud.DeviceId)
                    select u;
         }

         if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(c => c.Email.Contains(email));

         if (!string.IsNullOrWhiteSpace(username))
            query = query.Where(c => c.Username.Contains(username));

         if (!string.IsNullOrWhiteSpace(firstName))
         {
            query = query
                   .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                       (x, y) => new { User = x, Attribute = y })
                   .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                               z.Attribute.Key == AppUserDefaults.FirstNameAttribute &&
                               z.Attribute.Value.Contains(firstName))
                   .Select(z => z.User);
         }

         if (!string.IsNullOrWhiteSpace(lastName))
         {
            query = query
                   .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                       (x, y) => new { User = x, Attribute = y })
                   .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                               z.Attribute.Key == AppUserDefaults.LastNameAttribute &&
                               z.Attribute.Value.Contains(lastName))
                   .Select(z => z.User);
         }

         //date of birth is stored as a string into database.
         //we also know that date of birth is stored in the following format YYYY-MM-DD (for example, 1983-02-18).
         //so let's search it as a string
         if (dayOfBirth > 0 && monthOfBirth > 0)
         {
            //both are specified
            var dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" +
                                    dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

            //z.Attribute.Value.Length - dateOfBirthStr.Length = 5
            //dateOfBirthStr.Length = 5
            query = query
                   .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                       (x, y) => new { User = x, Attribute = y })
                   .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                               z.Attribute.Key == AppUserDefaults.DateOfBirthAttribute &&
                               z.Attribute.Value.Substring(5, 5) == dateOfBirthStr)
                   .Select(z => z.User);
         }
         else if (dayOfBirth > 0)
         {
            //only day is specified
            var dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

            //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
            //dateOfBirthStr.Length = 2
            query = query
                   .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                       (x, y) => new { User = x, Attribute = y })
                   .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                               z.Attribute.Key == AppUserDefaults.DateOfBirthAttribute &&
                               z.Attribute.Value.Substring(8, 2) == dateOfBirthStr)
                   .Select(z => z.User);
         }
         else if (monthOfBirth > 0)
         {
            //only month is specified
            var dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
            query = query
                   .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                       (x, y) => new { User = x, Attribute = y })
                   .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                               z.Attribute.Key == AppUserDefaults.DateOfBirthAttribute &&
                               z.Attribute.Value.Contains(dateOfBirthStr))
                   .Select(z => z.User);
         }

         //search by company
         if (!string.IsNullOrWhiteSpace(company))
         {
            query = query
                   .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                       (x, y) => new { User = x, Attribute = y })
                   .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                               z.Attribute.Key == AppUserDefaults.CompanyAttribute &&
                               z.Attribute.Value.Contains(company))
                   .Select(z => z.User);
         }

         //search by phone
         if (!string.IsNullOrWhiteSpace(phone))
         {
            query = query
                   .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                       (x, y) => new { User = x, Attribute = y })
                   .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                               z.Attribute.Key == AppUserDefaults.PhoneAttribute &&
                               z.Attribute.Value.Contains(phone))
                   .Select(z => z.User);
         }

         //search by zip
         if (!string.IsNullOrWhiteSpace(zipPostalCode))
         {
            query = query
                   .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                       (x, y) => new { User = x, Attribute = y })
                   .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                               z.Attribute.Key == AppUserDefaults.ZipPostalCodeAttribute &&
                               z.Attribute.Value.Contains(zipPostalCode))
                   .Select(z => z.User);
         }

         //search by IpAddress
         if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
         {
            query = query.Where(w => w.LastIpAddress == ipAddress);
         }

         // search last activity
         if (lastActivityFromUtc.HasValue)
            query = query.Where(x => x.LastActivityUtc >= lastActivityFromUtc.Value);

         if (lastActivityToUtc.HasValue)
            query = query.Where(x => x.LastActivityUtc <= lastActivityToUtc.Value);

         query = query.OrderByDescending(c => c.CreatedOnUtc);

         return query;
      }, pageIndex, pageSize, getOnlyTotalCount);

      return users;
   }

   /// <summary>
   /// Gets online users
   /// </summary>
   /// <param name="lastActivityFromUtc">User last activity date (from)</param>
   /// <param name="lastActivityToUtc">User frist activity date (to)</param>
   /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
   /// <param name="email">Email; null to load all users</param>
   /// <param name="company">Company; null to load all users</param>
   /// <param name="ipAddress">IP address; null to load all users</param>
   /// <param name="onlineIds">Online user identifiers</param>
   /// <param name="online">Show online devices</param>
   /// <param name="beenRecently">Show been recently users</param>
   /// <param name="offline">Show offline users</param>
   /// <param name="utcNow">Request date time</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the users
   /// </returns>
   public virtual async Task<IPagedList<User>> GetOnlineUsersAsync(long[] onlineIds, DateTime? lastActivityFromUtc = null, DateTime? lastActivityToUtc = null,
      long[] userRoleIds = null, string email = null, string company = null, string ipAddress = null, bool online = false, bool beenRecently = false, bool offline = false,
      DateTime? utcNow = null, int pageIndex = 0, int pageSize = int.MaxValue)
   {
      var now = utcNow ?? DateTime.UtcNow;

      var beenRecentlyLimit = now.AddMinutes(-_userSettings.BeenRecentlyMinutes);

      // https://stackoverflow.com/questions/2691392/enumerable-emptyt-equivalent-for-iqueryable
      // https://stackoverflow.com/questions/11067226/how-should-i-initialize-iqueryable-variables-before-use-a-union-expression
      var query = _userRepository.Table.AsNoTracking().Take(0);
      // var query = _userRepository.Table.Where(_ => false);

      // include offline users
      if (offline)
      {
         var offlineQuery = _userRepository.Table.AsNoTracking()
            .Where(x => x.LastActivityUtc < beenRecentlyLimit && !onlineIds.Contains(x.Id));

         if (lastActivityFromUtc.HasValue)
            offlineQuery = offlineQuery.Where(x => x.LastActivityUtc >= lastActivityFromUtc.Value);

         if (lastActivityToUtc.HasValue)
            offlineQuery = offlineQuery.Where(x => x.LastActivityUtc <= lastActivityToUtc.Value);

         query = query.Union(offlineQuery);
      }

      // include been recently users
      if (beenRecently)
      {
         query = (from u in _userRepository.Table.AsNoTracking()
                  where u.LastActivityUtc >= beenRecentlyLimit && !onlineIds.Contains(u.Id)
                  select u).Union(query);
      }

      // include online users
      if (online)
      {
         query = (from u in _userRepository.Table.AsNoTracking()
                  where onlineIds.Contains(u.Id)
                  select u).Union(query);
      }

      // search by user role
      if (userRoleIds != null && userRoleIds.Any())
      {
         query = from u in query
                 join ur in _userUserRoleMappingRepository.Table.AsNoTracking() on u.Id equals ur.UserId
                 where userRoleIds.Contains(ur.UserRoleId)
                 select u;
      }

      if (!string.IsNullOrWhiteSpace(email))
         query = query.Where(x => x.Email.Contains(email));

      // search by company
      if (company != null)
      {
         query = from u in query
                 join ga in _gaRepository.Table.AsNoTracking() on new { u.Id, KeyGroup = nameof(User) } equals new { Id = ga.EntityId, ga.KeyGroup }
                 where ga.Key == AppUserDefaults.CompanyAttribute && ga.Value.Contains(company)
                 select u;
      }

      //search by IpAddress
      if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
         query = query.Where(w => w.LastIpAddress == ipAddress);


      query = query.OrderByDescending(c => c.LastActivityUtc);
      var users = await Hub.Data.Extensions.AsyncIQueryableExtensions.ToPagedListAsync(query, pageIndex, pageSize);
      //var users = await query.ToPagedListAsync(pageIndex, pageSize);

      return users;
   }

   /// <summary>
   /// Delete a user
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteUserAsync(User user)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      if (user.IsSystemAccount)
         throw new AppException($"System user account ({user.SystemName}) could not be deleted");

      user.IsDeleted = true;

      if (_userSettings.SuffixDeletedUsers)
      {
         if (!string.IsNullOrEmpty(user.Email))
            user.Email += "-DELETED";
         if (!string.IsNullOrEmpty(user.Username))
            user.Username += "-DELETED";
      }

      await _userRepository.UpdateAsync(user, false);
      await _userRepository.DeleteAsync(user);
   }

   /// <summary>
   /// Gets a user
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains a user
   /// </returns>
   public virtual async Task<User> GetUserByIdAsync(long userId)
   {
      return await _userRepository.GetByIdAsync(userId,
         cache => cache.PrepareKeyForShortTermCache(AppEntityCacheDefaults<User>.ByIdCacheKey, userId));
   }

   /// <summary>
   /// Get users by identifiers
   /// </summary>
   /// <param name="userIds">User identifiers</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the users
   /// </returns>
   public virtual async Task<IList<User>> GetUsersByIdsAsync(long[] userIds)
   {
      return await _userRepository.GetByIdsAsync(userIds);
   }

   /// <summary>
   /// Gets a user by GUID
   /// </summary>
   /// <param name="userGuid">User GUID</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains a user
   /// </returns>
   public virtual async Task<User> GetUserByGuidAsync(Guid userGuid)
   {
      if (userGuid == Guid.Empty)
         return null;

      var query = from u in _userRepository.Table
                  where u.UserGuid == userGuid
                  orderby u.Id
                  select u;

      var key = _staticCacheManager.PrepareKeyForShortTermCache(AppUserServicesDefaults.UserByGuidCacheKey, userGuid);
      return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
   }

   /// <summary>
   /// Get user by email
   /// </summary>
   /// <param name="email">Email</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user
   /// </returns>
   public virtual async Task<User> GetUserByEmailAsync(string email)
   {
      if (string.IsNullOrWhiteSpace(email))
         return null;

      var query = from u in _userRepository.Table
                  orderby u.Id
                  where u.Email == email
                  select u;

      var key = _staticCacheManager.PrepareKeyForShortTermCache(AppUserServicesDefaults.UserByEmailCacheKey, email);
      return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
   }


   /// <summary>
   /// Get user by username
   /// </summary>
   /// <param name="username">Username</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user
   /// </returns>
   public virtual async Task<User> GetUserByUsernameAsync(string username)
   {
      if (string.IsNullOrWhiteSpace(username))
         return null;

      var query = from u in _userRepository.Table
                  orderby u.Id
                  where u.Username == username
                  select u;

      var key = _staticCacheManager.PrepareKeyForShortTermCache(AppUserServicesDefaults.UserByUsernameCacheKey, username);
      return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
   }


   /// <summary>
   /// Get user by system name
   /// </summary>
   /// <param name="systemName">System name</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user
   /// </returns>
   public virtual async Task<User> GetUserBySystemNameAsync(string systemName)
   {
      if (string.IsNullOrWhiteSpace(systemName))
         return null;

      var query = from u in _userRepository.Table
                  orderby u.Id
                  where u.SystemName == systemName
                  select u;

      var key = _staticCacheManager.PrepareKeyForDefaultCache(AppUserServicesDefaults.UserBySystemNameCacheKey, systemName);
      return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
   }

   /// <summary>
   /// Gets built-in system record used for background tasks
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains a user object
   /// </returns>
   public virtual async Task<User> GetOrCreateBackgroundTaskUserAsync()
   {
      var backgroundTaskUser = await GetUserBySystemNameAsync(AppUserDefaults.BackgroundTaskUserName);

      if (backgroundTaskUser is null)
      {
         //If for any reason the system user isn't in the database, then we add it
         backgroundTaskUser = new User
         {
            Email = "builtin@background-task-record.com",
            UserGuid = Guid.NewGuid(),
            AdminComment = "Built-in system record used for background tasks.",
            IsActive = true,
            IsSystemAccount = true,
            SystemName = AppUserDefaults.BackgroundTaskUserName,
            CreatedOnUtc = DateTime.UtcNow,
            LastActivityUtc = DateTime.UtcNow,
         };

         await InsertUserAsync(backgroundTaskUser);

         var guestRole = await GetUserRoleBySystemNameAsync(UserDefaults.GuestsRoleName);

         if (guestRole is null)
            throw new AppException("'Guests' role could not be loaded");

         await AddUserRoleMappingAsync(new UserUserRole { UserRoleId = guestRole.Id, UserId = backgroundTaskUser.Id });
      }

      return backgroundTaskUser;
   }

   /// <summary>
   /// Gets built-in system guest record used for requests from search engines
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains a user object
   /// </returns>
   public virtual async Task<User> GetOrCreateSearchEngineUserAsync()
   {
      var searchEngineUser = await GetUserBySystemNameAsync(AppUserDefaults.SearchEngineUserName);

      if (searchEngineUser is null)
      {
         //If for any reason the system user isn't in the database, then we add it
         searchEngineUser = new User
         {
            Email = "builtin@search_engine_record.com",
            UserGuid = Guid.NewGuid(),
            AdminComment = "Built-in system guest record used for requests from search engines.",
            IsActive = true,
            IsSystemAccount = true,
            SystemName = AppUserDefaults.SearchEngineUserName,
            CreatedOnUtc = DateTime.UtcNow,
            LastActivityUtc = DateTime.UtcNow,
         };

         await InsertUserAsync(searchEngineUser);

         var guestRole = await GetUserRoleBySystemNameAsync(UserDefaults.GuestsRoleName);

         if (guestRole is null)
            throw new AppException("'Guests' role could not be loaded");

         await AddUserRoleMappingAsync(new UserUserRole { UserRoleId = guestRole.Id, UserId = searchEngineUser.Id });
      }

      return searchEngineUser;
   }

   /// <summary>
   /// Insert a guest user
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user
   /// </returns>
   public virtual async Task<User> InsertGuestUserAsync()
   {
      var user = new User
      {
         UserGuid = Guid.NewGuid(),
         IsActive = true,
         CreatedOnUtc = DateTime.UtcNow,
         LastActivityUtc = DateTime.UtcNow,
      };

      //add to 'Guests' role
      var guestRole = await GetUserRoleBySystemNameAsync(UserDefaults.GuestsRoleName) ?? throw new AppException("'Guests' role could not be loaded");

      await _userRepository.InsertAsync(user);

      await AddUserRoleMappingAsync(new UserUserRole { UserId = user.Id, UserRoleId = guestRole.Id });

      return user;
   }

   /// <summary>
   /// Insert a user
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertUserAsync(User user)
   {
      await _userRepository.InsertAsync(user);
   }

   /// <summary>
   /// Updates the user
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateUserAsync(User user)
   {
      await _userRepository.UpdateAsync(user);
   }

   /// <summary>
   /// Delete guest user records
   /// </summary>
   /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
   /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the number of deleted users
   /// </returns>
   public virtual async Task<int> DeleteGuestUsersAsync(DateTime? createdFromUtc, DateTime? createdToUtc)
   {
      var guestRole = await GetUserRoleBySystemNameAsync(UserDefaults.GuestsRoleName);

      var guestsToDelete =
          from u in _userRepository.Table
          join uur in _userUserRoleMappingRepository.Table on u.Id equals uur.UserId into rolegr
          from g in rolegr.DefaultIfEmpty()
          where g == default || (g.UserId == u.Id && g.UserRoleId == guestRole.Id && !u.IsSystemAccount && (createdFromUtc == null || (u.CreatedOnUtc > createdFromUtc && u.CreatedOnUtc < createdToUtc)))
          select u;

      var gaRecordsToDelete =
         from ga in _gaRepository.Table
         join u in guestsToDelete on ga.EntityId equals u.Id
         where ga.KeyGroup == nameof(User)
         select ga;

      var addressesToDelete =
         from a in _userAddressRepository.Table
         join ua in _userAddressMappingRepository.Table on a.Id equals ua.AddressId
         join u in guestsToDelete on ua.UserId equals u.Id
         select a;

      var totalRecordsDeleted = await guestsToDelete.ExecuteDeleteAsync();
      await gaRecordsToDelete.ExecuteDeleteAsync();
      await addressesToDelete.ExecuteDeleteAsync();

      return totalRecordsDeleted;
   }

   /// <summary>
   /// Get full name
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user full name
   /// </returns>
   public virtual async Task<string> GetUserFullNameAsync(User user)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      var firstName = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.FirstNameAttribute);
      var lastName = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.LastNameAttribute);

      var fullName = string.Empty;
      if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
         fullName = $"{firstName} {lastName}";
      else
      {
         if (!string.IsNullOrWhiteSpace(firstName))
            fullName = firstName;

         if (!string.IsNullOrWhiteSpace(lastName))
            fullName = lastName;
      }

      return fullName;
   }

   /// <summary>
   /// Formats the user name
   /// </summary>
   /// <param name="user">Source</param>
   /// <param name="stripTooLong">Strip too long user name</param>
   /// <param name="maxLength">Maximum user name length</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the formatted text
   /// </returns>
   public virtual async Task<string> FormatUsernameAsync(User user, bool stripTooLong = false, int maxLength = 0)
   {
      if (user == null)
         return string.Empty;

      if (await IsGuestAsync(user))
         //do not inject ILocalizationService via constructor because it'll cause circular references
         return await EngineContext.Current.Resolve<ILocalizationService>().GetResourceAsync("User.Guest");

      var result = string.Empty;
      switch (_userSettings.UserNameFormat)
      {
         case UserNameFormat.ShowEmails:
            result = user.Email;
            break;
         case UserNameFormat.ShowUsernames:
            result = user.Username;
            break;
         case UserNameFormat.ShowFullNames:
            result = await GetUserFullNameAsync(user);
            break;
         case UserNameFormat.ShowFirstName:
            result = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.FirstNameAttribute);
            break;
         default:
            break;
      }

      if (stripTooLong && maxLength > 0)
         result = CommonHelper.EnsureMaximumLength(result, maxLength);

      return result;
   }

   #endregion

   #region User roles

   /// <summary>
   /// Gets only registered user identifierss from user identifiers
   /// </summary>
   /// <param name="userIds">Init user identifiers</param>
   /// <returns>Registered user identifier collection</returns>
   public async Task<IList<long>> GetOnlyRegisteredUserIdsAsync(IEnumerable<long> userIds)
   {
      ArgumentNullException.ThrowIfNull(userIds);

      if (!userIds.Any())
         return new List<long>();

      var query = from u in _userRepository.Table
                  join ur in _userUserRoleMappingRepository.Table on u.Id equals ur.UserId
                  join r in _userRoleRepository.Table on ur.UserRoleId equals r.Id
                  where userIds.Contains(u.Id) && r.SystemName == UserDefaults.RegisteredRoleName
                  select u.Id;

      return await query.ToListAsync();
   }


   /// <summary>
   /// Add a user-user role mapping
   /// </summary>
   /// <param name="roleMapping">User-user role mapping</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public async Task AddUserRoleMappingAsync(UserUserRole roleMapping)
   {
      await _userUserRoleMappingRepository.InsertAsync(roleMapping);
   }

   /// <summary>
   /// Remove a user-user role mapping
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="role">User role</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public async Task RemoveUserRoleMappingAsync(User user, UserRole role)
   {
      if (user is null)
         throw new ArgumentNullException(nameof(user));

      if (role is null)
         throw new ArgumentNullException(nameof(role));

      var mapping = await _userUserRoleMappingRepository.Table
          .SingleOrDefaultAsync(ccrm => ccrm.UserId == user.Id && ccrm.UserRoleId == role.Id);

      if (mapping != null)
         await _userUserRoleMappingRepository.DeleteAsync(mapping);
   }

   /// <summary>
   /// Delete a user role
   /// </summary>
   /// <param name="userRole">User role</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteUserRoleAsync(UserRole userRole)
   {
      if (userRole == null)
         throw new ArgumentNullException(nameof(userRole));

      if (userRole.IsSystemRole)
         throw new AppException("System role could not be deleted");

      await _userRoleRepository.DeleteAsync(userRole);
   }

   /// <summary>
   /// Gets a user role
   /// </summary>
   /// <param name="userRoleId">User role identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user role
   /// </returns>
   public virtual async Task<UserRole> GetUserRoleByIdAsync(long userRoleId)
   {
      return await _userRoleRepository.GetByIdAsync(userRoleId, cache => default);
   }

   /// <summary>
   /// Gets a user role
   /// </summary>
   /// <param name="systemName">User role system name</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user role
   /// </returns>
   public virtual async Task<UserRole> GetUserRoleBySystemNameAsync(string systemName)
   {
      if (string.IsNullOrWhiteSpace(systemName))
         return null;

      var key = _staticCacheManager.PrepareKeyForDefaultCache(AppUserServicesDefaults.UserRolesBySystemNameCacheKey, systemName);

      var query = from cr in _userRoleRepository.Table
                  orderby cr.Id
                  where cr.SystemName == systemName
                  select cr;

      var userRole = await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());

      return userRole;
   }

   /// <summary>
   /// Get user role identifiers
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="showHidden">A value indicating whether to load hidden records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user role identifiers
   /// </returns>
   public virtual async Task<long[]> GetUserRoleIdsAsync(User user, bool showHidden = false)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      var query = from cr in _userRoleRepository.Table
                  join crm in _userUserRoleMappingRepository.Table on cr.Id equals crm.UserRoleId
                  where crm.UserId == user.Id &&
                  (showHidden || cr.Active)
                  select cr.Id;

      var key = _staticCacheManager.PrepareKeyForShortTermCache(AppUserServicesDefaults.UserRoleIdsCacheKey, user, showHidden);

      return await _staticCacheManager.GetAsync(key, () => query.ToArray());
   }

   /// <summary>
   /// Gets list of user roles
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="showHidden">A value indicating whether to load hidden records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<IList<UserRole>> GetUserRolesAsync(User user, bool showHidden = false)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      return await _userRoleRepository.GetAllAsync(query =>
      {
         return from cr in query
                join crm in _userUserRoleMappingRepository.Table on cr.Id equals crm.UserRoleId
                where crm.UserId == user.Id &&
                         (showHidden || cr.Active)
                select cr;

      }, cache => cache.PrepareKeyForShortTermCache(AppUserServicesDefaults.UserRolesCacheKey, user, showHidden));

   }

   /// <summary>
   /// Gets all user roles
   /// </summary>
   /// <param name="showHidden">A value indicating whether to show hidden records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user roles
   /// </returns>
   public virtual async Task<IList<UserRole>> GetAllUserRolesAsync(bool showHidden = false)
   {
      var key = _staticCacheManager.PrepareKeyForDefaultCache(AppUserServicesDefaults.UserRolesAllCacheKey, showHidden);

      var query = from cr in _userRoleRepository.Table
                  orderby cr.Name
                  where showHidden || cr.Active
                  select cr;

      var userRoles = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

      return userRoles;
   }

   /// <summary>
   /// Inserts a user role
   /// </summary>
   /// <param name="userRole">User role</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertUserRoleAsync(UserRole userRole)
   {
      await _userRoleRepository.InsertAsync(userRole);
   }

   /// <summary>
   /// Gets a value indicating whether user is in a certain user role
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="userRoleSystemName">User role system name</param>
   /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsInUserRoleAsync(User user,
       string userRoleSystemName, bool onlyActiveUserRoles = true)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      if (string.IsNullOrEmpty(userRoleSystemName))
         throw new ArgumentNullException(nameof(userRoleSystemName));

      var userRoles = await GetUserRolesAsync(user, !onlyActiveUserRoles);

      return userRoles?.Any(cr => cr.SystemName == userRoleSystemName) ?? false;
   }

   /// <summary>
   /// Gets a value indicating whether user is administrator
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsAdminAsync(User user, bool onlyActiveUserRoles = true)
   {
      return await IsInUserRoleAsync(user, UserDefaults.AdministratorsRoleName, onlyActiveUserRoles);
   }

   /// <summary>
   /// Gets a value indicating whether user is a forum moderator
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsForumModeratorAsync(User user, bool onlyActiveUserRoles = true)
   {
      return await IsInUserRoleAsync(user, UserDefaults.ForumModeratorsRoleName, onlyActiveUserRoles);
   }

   /// <summary>
   /// Gets a value indicating whether user is registered
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsRegisteredAsync(User user, bool onlyActiveUserRoles = true)
   {
      return await IsInUserRoleAsync(user, UserDefaults.RegisteredRoleName, onlyActiveUserRoles);
   }

   /// <summary>
   /// Gets a value indicating whether user is guest
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsGuestAsync(User user, bool onlyActiveUserRoles = true)
   {
      return await IsInUserRoleAsync(user, UserDefaults.GuestsRoleName, onlyActiveUserRoles);
   }

   /// <summary>
   /// Gets a value indicating whether user is vendor
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsOwnerAsync(User user, bool onlyActiveUserRoles = true)
   {
      return await IsInUserRoleAsync(user, UserDefaults.OwnersRoleName, onlyActiveUserRoles);
   }

   /// <summary>
   /// Gets a value indicating whether user is vendor
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsOperatorAsync(User user, bool onlyActiveUserRoles = true)
   {
      return await IsInUserRoleAsync(user, UserDefaults.OperatorsRoleName, onlyActiveUserRoles);
   }

   /// <summary>
   /// Gets a value indicating whether user is vendor
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsDemoUserAsync(User user, bool onlyActiveUserRoles = true)
   {
      return await IsInUserRoleAsync(user, UserDefaults.DemoRoleName, onlyActiveUserRoles);
   }

   /// <summary>
   /// Gets a value indicating whether user is a device
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsDeviceUserAsync(User user, bool onlyActiveUserRoles = true)
   {
      return await IsInUserRoleAsync(user, UserDefaults.DevicesRoleName, onlyActiveUserRoles);
   }

   /// <summary>
   /// Updates the user role
   /// </summary>
   /// <param name="userRole">User role</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateUserRoleAsync(UserRole userRole)
   {
      await _userRoleRepository.UpdateAsync(userRole);
   }

   #endregion

   #region User passwords

   /// <summary>
   /// Gets user passwords
   /// </summary>
   /// <param name="userId">User identifier; pass null to load all records</param>
   /// <param name="passwordFormat">Password format; pass null to load all records</param>
   /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of user passwords
   /// </returns>
   public virtual async Task<IList<UserPassword>> GetUserPasswordsAsync(long? userId = null,
       PasswordFormat? passwordFormat = null, int? passwordsToReturn = null)
   {
      var query = _userPasswordRepository.Table;

      //filter by user
      if (userId.HasValue)
         query = query.Where(password => password.UserId == userId.Value);

      //filter by password format
      if (passwordFormat.HasValue)
         query = query.Where(password => password.PasswordFormatId == (int)passwordFormat.Value);

      //get the latest passwords
      if (passwordsToReturn.HasValue)
         query = query.OrderByDescending(password => password.CreatedOnUtc).Take(passwordsToReturn.Value);

      return await query.ToListAsync();
   }

   /// <summary>
   /// Get current user password
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user password
   /// </returns>
   public virtual async Task<UserPassword> GetCurrentPasswordAsync(long userId)
   {
      if (userId == 0)
         return null;

      //return the latest password
      return (await GetUserPasswordsAsync(userId, passwordsToReturn: 1)).FirstOrDefault();
   }

   /// <summary>
   /// Insert a user password
   /// </summary>
   /// <param name="userPassword">User password</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertUserPasswordAsync(UserPassword userPassword)
   {
      await _userPasswordRepository.InsertAsync(userPassword);
   }

   /// <summary>
   /// Update a user password
   /// </summary>
   /// <param name="userPassword">User password</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateUserPasswordAsync(UserPassword userPassword)
   {
      await _userPasswordRepository.UpdateAsync(userPassword);
   }

   /// <summary>
   /// Check whether password recovery token is valid
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="token">Token to validate</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsPasswordRecoveryTokenValidAsync(User user, string token)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      var cPrt = await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.PasswordRecoveryTokenAttribute);
      if (string.IsNullOrEmpty(cPrt))
         return false;

      if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
         return false;

      return true;
   }

   /// <summary>
   /// Check whether password recovery link is expired
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsPasswordRecoveryLinkExpiredAsync(User user)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      if (_userSettings.PasswordRecoveryLinkDaysValid == 0)
         return false;

      var generatedDate = await _genericAttributeService.GetAttributeAsync<DateTime?>(user, AppUserDefaults.PasswordRecoveryTokenDateGeneratedAttribute);
      if (!generatedDate.HasValue)
         return false;

      var daysPassed = (DateTime.UtcNow - generatedDate.Value).TotalDays;
      if (daysPassed > _userSettings.PasswordRecoveryLinkDaysValid)
         return true;

      return false;
   }

   /// <summary>
   /// Check whether user password is expired 
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue if password is expired; otherwise false
   /// </returns>
   public virtual async Task<bool> IsPasswordExpiredAsync(User user)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      //the guests don't have a password
      if (await IsGuestAsync(user))
         return false;

      //password lifetime is disabled for user
      if (!(await GetUserRolesAsync(user)).Any(role => role.Active && role.EnablePasswordLifetime))
         return false;

      //setting disabled for all
      if (_userSettings.PasswordLifetime == 0)
         return false;

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(AppUserServicesDefaults.UserPasswordLifetimeCacheKey, user);

      //get current password usage time
      var currentLifetime = await _staticCacheManager.GetAsync(cacheKey, async () =>
      {
         var userPassword = await GetCurrentPasswordAsync(user.Id);
         //password is not found, so return max value to force user to change password
         if (userPassword == null)
            return int.MaxValue;

         return (DateTime.UtcNow - userPassword.CreatedOnUtc).Days;
      });

      return currentLifetime >= _userSettings.PasswordLifetime;
   }

   #endregion

   #region User address mapping

   /// <summary>
   /// Remove a user-address mapping record
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="address">Address</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task RemoveUserAddressAsync(User user, Address address)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      if (await _userAddressMappingRepository.Table
          .FirstOrDefaultAsync(m => m.AddressId == address.Id && m.UserId == user.Id)
          is UserAddress mapping)
      {
         if (user.BillingAddressId == address.Id)
            user.BillingAddressId = null;
         if (user.ShippingAddressId == address.Id)
            user.ShippingAddressId = null;

         await _userAddressMappingRepository.DeleteAsync(mapping);
      }
   }

   /// <summary>
   /// Inserts a user-address mapping record
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="address">Address</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertUserAddressAsync(User user, Address address)
   {
      if (user is null)
         throw new ArgumentNullException(nameof(user));

      if (address is null)
         throw new ArgumentNullException(nameof(address));

      if (await _userAddressMappingRepository.Table
          .FirstOrDefaultAsync(m => m.AddressId == address.Id && m.UserId == user.Id)
          is null)
      {
         var mapping = new UserAddress
         {
            AddressId = address.Id,
            UserId = user.Id
         };

         await _userAddressMappingRepository.InsertAsync(mapping);
      }
   }



   /// <summary>
   /// Gets a list of addresses mapped to user
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<IList<Address>> GetAddressesByUserIdAsync(long userId)
   {
      var query = from address in _userAddressRepository.Table
                  join cam in _userAddressMappingRepository.Table on address.Id equals cam.AddressId
                  where cam.UserId == userId
                  select address;

      var key = _staticCacheManager.PrepareKeyForShortTermCache(AppUserServicesDefaults.UserAddressesCacheKey, userId);

      return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
   }

   /// <summary>
   /// Gets a address mapped to user
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="addressId">Address identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<Address> GetUserAddressAsync(long userId, long addressId)
   {
      if (userId == 0 || addressId == 0)
         return null;

      var query = from address in _userAddressRepository.Table
                  join cam in _userAddressMappingRepository.Table on address.Id equals cam.AddressId
                  where cam.UserId == userId && address.Id == addressId
                  select address;

      var key = _staticCacheManager.PrepareKeyForShortTermCache(AppUserServicesDefaults.UserAddressCacheKey, userId, addressId);

      return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
   }

   /// <summary>
   /// Gets a user billing address
   /// </summary>
   /// <param name="user">User identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<Address> GetUserBillingAddressAsync(User user)
   {
      if (user is null)
         throw new ArgumentNullException(nameof(user));

      return await GetUserAddressAsync(user.Id, user.BillingAddressId ?? 0);
   }

   /// <summary>
   /// Gets a user shipping address
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<Address> GetUserShippingAddressAsync(User user)
   {
      if (user is null)
         throw new ArgumentNullException(nameof(user));

      return await GetUserAddressAsync(user.Id, user.ShippingAddressId ?? 0);
   }

   #endregion

   #region Clients

   /// <summary>
   /// Gets shared device's users by device identifier
   /// </summary>
   /// <param name="deviceId">Device identifier</param>
   /// <returns>User collection</returns>
   public async Task<IList<User>> GetUsersByDeviceIdAsync(long deviceId)
   {
      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(AppUserServicesDefaults.UserDeviceCacheKey, deviceId, 0);

      var query = from u in _userRepository.Table
                  join ud in _userDeviceRepository.Table on u.Id equals ud.UserId
                  where ud.DeviceId == deviceId
                  select u;

      return await _staticCacheManager.GetAsync(cacheKey, async () => await query.ToListAsync());
   }

   /// <summary>
   /// Gets all users for dasboard clients (using by admins only)
   /// </summary>
   /// <param name="filter">Dymamic filter</param>
   /// <returns>Filterd user collection</returns>
   public async Task<IFilterableList<User>> GetUsersByFilterAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      if (!filter.ConnectionStatuses.Any())
         return new FilterableList<User>();

      var query = _userRepository.Table.AsNoTracking();

      var onlineUserIds = await _communicator.GetOnlineUserIdsAsync();
      var beenRecenlyLimit = DateTime.UtcNow.AddMinutes(-_userSettings.BeenRecentlyMinutes);

      // projection with required fields
      query = from u in query
              select new User()
              {
                 Id = u.Id,
                 LastActivityUtc = u.LastActivityUtc,
                 IsActive = u.IsActive,
                 Username = _userSettings.UsernamesEnabled ? u.Username : u.Email,
                 AvatarPictureId = u.AvatarPictureId,
                 LastIpAddress = u.LastIpAddress,
                 OnlineStatus = onlineUserIds.Contains(u.Id)
                 ? OnlineStatus.Online
                 : u.LastActivityUtc >= beenRecenlyLimit
                    ? OnlineStatus.BeenRecently
                    : OnlineStatus.Offline,
              };

      // get only telemetry roles by the weakest role
      //var dashboardRoles = UserDefaults.TelemetryRoles.Split(',').Select(x=> x.Trim());

      query =
          from u in query
          join ur in _userUserRoleMappingRepository.Table.AsNoTracking() on u.Id equals ur.UserId
          join r in _userRoleRepository.Table.AsNoTracking() on ur.UserRoleId equals r.Id
          where r.SystemName == UserDefaults.OperatorsRoleName
          select u;

      // online status
      query =
      from u in query
      where filter.ConnectionStatuses.Contains(u.OnlineStatus)
      select u;

      if (filter.DeviceId.HasValue)
      {
         var deviceQuery = _deviceRepository.Table.AsNoTracking();

         if (filter.UserId.HasValue)
         {
            deviceQuery =
            from d in deviceQuery
            join ud in _userDeviceRepository.Table.AsNoTracking() on d.Id equals ud.DeviceId into udgroup
            from g in udgroup.DefaultIfEmpty()
            where (g != default && g.UserId == filter.UserId) || d.OwnerId == filter.UserId
            select d;
         }

         query =
         from u in query
         join ud in _userDeviceRepository.Table.AsNoTracking() on u.Id equals ud.UserId
         join d in deviceQuery on ud.DeviceId equals d.Id
         where ud.DeviceId == filter.DeviceId
         select u;
      }

      if (filter.MonitorId.HasValue)
      {
         var monitorQuery = _monitorRepository.Table.AsNoTracking();

         if (filter.UserId.HasValue)
         {
            monitorQuery =
               from m in monitorQuery
               join um in _userMonitorRepository.Table.AsNoTracking() on m.Id equals um.MonitorId into umgroup
               from g in umgroup.DefaultIfEmpty()
               where (g != default && g.UserId == filter.UserId) || m.OwnerId == filter.UserId
               select m;
         }

         query =
         from u in query
         join um in _userMonitorRepository.Table.AsNoTracking() on u.Id equals um.UserId
         join m in monitorQuery on um.MonitorId equals m.Id
         where um.MonitorId == filter.MonitorId
         select u;
      }

      query = query.ApplyClientQuery(filter);
      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets user select items for dasboard clients (using by admins only)
   /// </summary>
   /// <param name="filter">Dymamic filter</param>
   /// <returns>Filterd user select item collection</returns>
   public async Task<IFilterableList<UserSelectItem>> GetUserSelectItemsByFilterAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);


      // get only telemetry roles
      //var dashboardRoles = UserDefaults.TelemetryRoles.Split(',').Select(x => x.Trim());

      var userQuery =
          from u in _userRepository.Table.AsNoTracking()
          join ur in _userUserRoleMappingRepository.Table.AsNoTracking() on u.Id equals ur.UserId
          join r in _userRoleRepository.Table.AsNoTracking() on ur.UserRoleId equals r.Id
          where r.SystemName == UserDefaults.OperatorsRoleName
          select u;

      // projection with required fileds
      var query =
      from u in userQuery
      select new UserSelectItem()
      {
         Id = u.Id,
         Username = _userSettings.UsernamesEnabled ? u.Username : u.Email,
         AvatarPictureId = u.AvatarPictureId,
      };

      query = query.ApplyClientQuery(filter);
      var result = await query.FilterAsync(filter);
      return result;
   }


   #endregion

   #endregion
}