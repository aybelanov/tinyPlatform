using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Web.Models.Common;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Users;
using Hub.Core.Domain.Directory;

namespace Hub.Web.Factories
{
   /// <summary>
   /// Represents the interface of the address model factory
   /// </summary>
   public partial interface IAddressModelFactory
   {
      /// <summary>
      /// Prepare address model
      /// </summary>
      /// <param name="model">Address model</param>
      /// <param name="address">Address entity</param>
      /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
      /// <param name="addressSettings">Address settings</param>
      /// <param name="loadCountries">Countries loading function; pass null if countries do not need to load</param>
      /// <param name="prePopulateWithUserFields">Whether to populate model properties with the user fields (used with the user entity)</param>
      /// <param name="user">User entity; required if prePopulateWithUserFields is true</param>
      /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of the address entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task PrepareAddressModelAsync(AddressModel model,
          Address address, bool excludeProperties,
          AddressSettings addressSettings,
          Func<Task<IList<Country>>> loadCountries = null,
          bool prePopulateWithUserFields = false,
          User user = null,
          string overrideAttributesXml = "");
   }
}
