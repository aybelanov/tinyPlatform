using Hub.Core.Domain.Common;
using Hub.Core.Domain.Directory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Directory
{
   /// <summary>
   /// State province service interface
   /// </summary>
   public partial interface IStateProvinceService
   {
      /// <summary>
      /// Deletes a state/province
      /// </summary>
      /// <param name="stateProvince">The state/province</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteStateProvinceAsync(StateProvince stateProvince);

      /// <summary>
      /// Gets a state/province
      /// </summary>
      /// <param name="stateProvinceId">The state/province identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the state/province
      /// </returns>
      Task<StateProvince> GetStateProvinceByIdAsync(long stateProvinceId);

      /// <summary>
      /// Gets a state/province by abbreviation
      /// </summary>
      /// <param name="abbreviation">The state/province abbreviation</param>
      /// <param name="countryId">Country identifier; pass null to load the state regardless of a country</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the state/province
      /// </returns>
      Task<StateProvince> GetStateProvinceByAbbreviationAsync(string abbreviation, long? countryId = null);

      /// <summary>
      /// Gets a state/province by address 
      /// </summary>
      /// <param name="address">Address</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the country
      /// </returns>
      Task<StateProvince> GetStateProvinceByAddressAsync(Address address);

      /// <summary>
      /// Gets a state/province collection by country identifier
      /// </summary>
      /// <param name="countryId">Country identifier</param>
      /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
      /// <param name="showHidden">A value indicating whether to show hidden records</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the states
      /// </returns>
      Task<IList<StateProvince>> GetStateProvincesByCountryIdAsync(long countryId, long languageId = 0, bool showHidden = false);

      /// <summary>
      /// Gets all states/provinces
      /// </summary>
      /// <param name="showHidden">A value indicating whether to show hidden records</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the states
      /// </returns>
      Task<IList<StateProvince>> GetStateProvincesAsync(bool showHidden = false);

      /// <summary>
      /// Inserts a state/province
      /// </summary>
      /// <param name="stateProvince">State/province</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertStateProvinceAsync(StateProvince stateProvince);

      /// <summary>
      /// Updates a state/province
      /// </summary>
      /// <param name="stateProvince">State/province</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdateStateProvinceAsync(StateProvince stateProvince);
   }
}
