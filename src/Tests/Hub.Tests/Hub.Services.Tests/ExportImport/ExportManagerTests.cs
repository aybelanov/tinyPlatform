using ClosedXML.Excel;
using FluentAssertions;
using Hub.Core;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Users;
using Hub.Services.Common;
using Hub.Services.Directory;
using Hub.Services.ExportImport;
using Hub.Services.ExportImport.Help;
using Hub.Services.Users;
using Hub.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Tests.ExportImport;

[TestFixture]
public class ExportManagerTests : ServiceTest
{
   #region Fields

   private IAddressService _addressService;
   private ICountryService _countryService;
   private IUserService _userService;
   private IExportManager _exportManager;
   private IMeasureService _measureService;
   private CommonSettings _commonSettings;

   #endregion

   #region Setup

   [OneTimeSetUp]
   public async Task SetUp()
   {
      _addressService = GetService<IAddressService>();
      _countryService = GetService<ICountryService>();
      _userService = GetService<IUserService>();
      _exportManager = GetService<IExportManager>();
      _measureService = GetService<IMeasureService>();
      _commonSettings = GetService<CommonSettings>();

      await GetService<IGenericAttributeService>()
          .SaveAttributeAsync(await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail), "category-advanced-mode",
              true);
      await GetService<IGenericAttributeService>()
          .SaveAttributeAsync(await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
              true);
      await GetService<IGenericAttributeService>()
          .SaveAttributeAsync(await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail), "product-advanced-mode",
              true);
   }

   [OneTimeTearDown]
   public async Task TearDown()
   {
      await GetService<IGenericAttributeService>()
          .SaveAttributeAsync(await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail), "category-advanced-mode",
              false);
      await GetService<IGenericAttributeService>()
          .SaveAttributeAsync(await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
              false);
      await GetService<IGenericAttributeService>()
          .SaveAttributeAsync(await _userService.GetUserByEmailAsync(AppTestsDefaults.AdminEmail), "product-advanced-mode",
              false);
   }

   #endregion

   #region Utilities

   protected static T PropertiesShouldEqual<T, Tp>(T actual, PropertyManager<Tp> manager, IDictionary<string, string> replacePairs, params string[] filter)
   {
      var objectProperties = typeof(T).GetProperties();
      foreach (var property in manager.GetProperties)
      {
         if (filter.Contains(property.PropertyName))
            continue;

         var objectProperty = replacePairs.ContainsKey(property.PropertyName)
             ? objectProperties.FirstOrDefault(p => p.Name == replacePairs[property.PropertyName])
             : objectProperties.FirstOrDefault(p => p.Name == property.PropertyName);

         if (objectProperty == null)
            continue;

         var objectPropertyValue = objectProperty.GetValue(actual);

         if (objectProperty.PropertyType == typeof(Guid))
            objectPropertyValue = objectPropertyValue.ToString();

         if (objectProperty.PropertyType == typeof(string))
            objectPropertyValue = (property.PropertyValue?.ToString() == string.Empty && objectPropertyValue == null) ? string.Empty : objectPropertyValue;

         if (objectProperty.PropertyType.IsEnum)
            objectPropertyValue = (int)objectPropertyValue;

         //https://github.com/ClosedXML/ClosedXML/blob/develop/ClosedXML/Extensions/ObjectExtensions.cs#L61
         if (objectProperty.PropertyType == typeof(DateTime))
            objectPropertyValue = DateTime.FromOADate(double.Parse(((DateTime)objectPropertyValue).ToOADate().ToString("G15", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));

         if (objectProperty.PropertyType == typeof(DateTime?))
            objectPropertyValue = objectPropertyValue != null ? DateTime.FromOADate(double.Parse(((DateTime?)objectPropertyValue)?.ToOADate().ToString("G15", CultureInfo.InvariantCulture))) : null;

         //https://github.com/ClosedXML/ClosedXML/issues/544
         property.PropertyValue.ToString().Should().Be(objectPropertyValue?.ToString() ?? "", $"The property \"{typeof(T).Name}.{property.PropertyName}\" of these objects is not equal");
      }

      return actual;
   }

   protected PropertyManager<T> GetPropertyManager<T>(IXLWorksheet worksheet)
   {
      //the columns
      var properties = ImportManager.GetPropertiesByExcelCells<T>(worksheet);

      return new PropertyManager<T>(properties, _commonSettings);
   }

   protected IXLWorksheet GetWorksheets(byte[] excelData)
   {
      var stream = new MemoryStream(excelData);
      var workbook = new XLWorkbook(stream);

      // get the first worksheet in the workbook
      var worksheet = workbook.Worksheets.FirstOrDefault();
      if (worksheet == null)
         throw new AppException("No worksheet found");

      return worksheet;
   }

   protected T AreAllObjectPropertiesPresent<T>(T obj, PropertyManager<T> manager, params string[] filters)
   {
      foreach (var propertyInfo in typeof(T).GetProperties())
      {
         if (filters.Contains(propertyInfo.Name))
            continue;

         if (manager.GetProperties.Any(p => p.PropertyName == propertyInfo.Name))
            continue;

         Assert.Fail($"The property \"{typeof(T).Name}.{propertyInfo.Name}\" no present on excel file");
      }

      return obj;
   }

   #endregion

   #region Test export to excel

   [Test]
   public async Task CanExportUsersToXlsx()
   {
      var users = await _userService.GetAllUsersAsync();

      var excelData = await _exportManager.ExportUsersToXlsxAsync(users);
      var worksheet = GetWorksheets(excelData);
      var manager = GetPropertyManager<User>(worksheet);

      manager.ReadFromXlsx(worksheet, 2);
      var user = users.First();

      var ignore = new List<string> { "Id", "ExternalAuthenticationRecords", "UserRoles", "UserPassword", "ReturnRequests", "BillingAddress", "ShippingAddress", "Addresses",
         "Address", "AdminComment", "EmailToRevalidate", "RequireReLogin", "FailedLoginAttempts", "CannotLoginUntilDateUtc", "Deleted", "IsSystemAccount", "SystemName",
         "LastIpAddress", "LastLoginDateUtc", "LastActivityDateUtc", "RegisteredInStoreId", "BillingAddressId", "ShippingAddressId", "UserUserRoleMappings",
         "UserAddressMappings", "EntityCacheKey", "VendorId", "IsDeleted", "ExternalAuthenticationRecords", "AddressMapping", "RoleMapping", "LogRecords",
         "ActivityLogRecords", "UpdatedOnUtc", "IsActive", "LastLoginUtc", "LastActivityUtc", "DeviceCountLimit", "OnlineStatus"
      };

      AreAllObjectPropertiesPresent(user, manager, [.. ignore]);
      PropertiesShouldEqual(user, manager, new Dictionary<string, string>());
   }

   #endregion
}
