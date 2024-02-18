﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core.Domain.Common;

namespace Hub.Services.Common
{
   /// <summary>
   /// Address service interface
   /// </summary>
   public partial interface IAddressService
   {
      /// <summary>
      /// Deletes an address
      /// </summary>
      /// <param name="address">Address</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteAddressAsync(Address address);

      /// <summary>
      /// Gets total number of addresses by country identifier
      /// </summary>
      /// <param name="countryId">Country identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the number of addresses
      /// </returns>
      Task<int> GetAddressTotalByCountryIdAsync(long countryId);

      /// <summary>
      /// Gets total number of addresses by state/province identifier
      /// </summary>
      /// <param name="stateProvinceId">State/province identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the number of addresses
      /// </returns>
      Task<int> GetAddressTotalByStateProvinceIdAsync(long stateProvinceId);

      /// <summary>
      /// Gets an address by address identifier
      /// </summary>
      /// <param name="addressId">Address identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the address
      /// </returns>
      Task<Address> GetAddressByIdAsync(long addressId);

      /// <summary>
      /// Inserts an address
      /// </summary>
      /// <param name="address">Address</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertAddressAsync(Address address);

      /// <summary>
      /// Updates the address
      /// </summary>
      /// <param name="address">Address</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdateAddressAsync(Address address);

      /// <summary>
      /// Gets a value indicating whether address is valid (can be saved)
      /// </summary>
      /// <param name="address">Address to validate</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the result
      /// </returns>
      Task<bool> IsAddressValidAsync(Address address);

      /// <summary>
      /// Find an address
      /// </summary>
      /// <param name="source">Source</param>
      /// <param name="firstName">First name</param>
      /// <param name="lastName">Last name</param>
      /// <param name="phoneNumber">Phone number</param>
      /// <param name="email">Email</param>
      /// <param name="faxNumber">Fax number</param>
      /// <param name="company">Company</param>
      /// <param name="address1">Address 1</param>
      /// <param name="address2">Address 2</param>
      /// <param name="city">City</param>
      /// <param name="county">County</param>
      /// <param name="stateProvinceId">State/province identifier</param>
      /// <param name="zipPostalCode">Zip postal code</param>
      /// <param name="countryId">Country identifier</param>
      /// <param name="customAttributes">Custom address attributes (XML format)</param>
      /// <returns>Address</returns>
      Address FindAddress(List<Address> source, string firstName, string lastName, string phoneNumber, string email,
          string faxNumber, string company, string address1, string address2, string city, string county, int? stateProvinceId,
          string zipPostalCode, long? countryId, string customAttributes);

      /// <summary>
      /// Clone address
      /// </summary>
      /// <returns>A deep copy of address</returns>
      Address CloneAddress(Address address);
   }
}