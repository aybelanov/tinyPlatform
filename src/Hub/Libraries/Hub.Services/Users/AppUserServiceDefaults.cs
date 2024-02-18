using Hub.Core.Caching;
using Hub.Core.Domain.Users;

namespace Hub.Services.Users
{
   /// <summary>
   /// Represents default values related to user services
   /// </summary>
   public static partial class AppUserServicesDefaults
   {
      /// <summary>
      /// Gets a password salt key size
      /// </summary>
      public static int PasswordSaltKeySize => 5;

      /// <summary>
      /// Gets a max username length
      /// </summary>
      public static int UserUsernameLength => 100;

      /// <summary>
      /// Gets a default hash format for user password
      /// </summary>
      public static string DefaultHashedPasswordFormat => "SHA512";

      /// <summary>
      /// Gets default prefix for user
      /// </summary>
      public static string UserAttributePrefix => "user_attribute_";

      #region Caching defaults

      #region User

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : system name
      /// </remarks>
      public static CacheKey UserBySystemNameCacheKey => new("App.user.bysystemname.{0}");

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : user GUID
      /// </remarks>
      public static CacheKey UserByGuidCacheKey => new("App.user.byguid.{0}");

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : user email
      /// </remarks>
      public static CacheKey UserByEmailCacheKey => new("App.user.byemail.{0}");

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : user username
      /// </remarks>
      public static CacheKey UserByUsernameCacheKey => new("App.user.username.{0}");

      #endregion

      #region User attributes

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : user attribute ID
      /// </remarks>
      public static CacheKey UserAttributeValuesByAttributeCacheKey => new("App.userattributevalue.byattribute.{0}");

      #endregion

      #region User roles

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : show hidden records?
      /// </remarks>
      public static CacheKey UserRolesAllCacheKey => new("App.userrole.all.{0}", AppEntityCacheDefaults<UserRole>.AllPrefix);

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : system name
      /// </remarks>
      public static CacheKey UserRolesBySystemNameCacheKey => new("App.userrole.bysystemname.{0}", UserRolesBySystemNamePrefix);

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      public static string UserRolesBySystemNamePrefix => "App.userrole.bysystemname.";

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : user identifier
      /// {1} : show hidden
      /// </remarks>
      public static CacheKey UserRoleIdsCacheKey => new("App.user.userrole.ids.{0}-{1}", UserUserRolesPrefix);

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : user identifier
      /// {1} : show hidden
      /// </remarks>
      public static CacheKey UserRolesCacheKey => new("App.user.userrole.{0}-{1}", UserUserRolesByUserPrefix, UserUserRolesPrefix);

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      public static string UserUserRolesPrefix => "App.user.userrole.";

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      /// <remarks>
      /// {0} : user identifier
      /// </remarks>
      public static string UserUserRolesByUserPrefix => "App.user.userrole.{0}";

      #endregion

      #region Addresses

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : user identifier
      /// </remarks>
      public static CacheKey UserAddressesCacheKey => new("App.user.addresses.{0}", UserAddressesPrefix);

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : user identifier
      /// {1} : address identifier
      /// </remarks>
      public static CacheKey UserAddressCacheKey => new("App.user.addresses.address.{0}-{1}", UserAddressesByUserPrefix, UserAddressesPrefix);

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      public static string UserAddressesPrefix => "App.user.addresses.";

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      /// <remarks>
      /// {0} : user identifier
      /// </remarks>
      public static string UserAddressesByUserPrefix => "App.user.addresses.{0}";

      #endregion

      #region User password

      /// <summary>
      /// Gets a key for caching current user password lifetime
      /// </summary>
      /// <remarks>
      /// {0} : user identifier
      /// </remarks>
      public static CacheKey UserPasswordLifetimeCacheKey => new("App.userpassword.lifetime.{0}");

      #endregion

      #region Telemetry

      /// <summary>
      /// Gets a key for caching
      /// </summary>
      /// <remarks>
      /// {0} : device id
      /// {0} : user id
      /// </remarks>
      public static CacheKey UserDeviceCacheKey => new("App.user.device.{0}-{1}", UserDevicePrefix);

      /// <summary>
      /// Gets a key pattern to clear cache
      /// </summary>
      public static string UserDevicePrefix => "App.user.device.";

      #endregion

      #endregion

   }
}