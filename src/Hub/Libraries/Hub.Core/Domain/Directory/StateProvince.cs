using Hub.Core.Domain.Common;
using Hub.Core.Domain.Localization;
using Shared.Common;
using System.Collections.Generic;

namespace Hub.Core.Domain.Directory;

/// <summary>
/// Represents a state/province
/// </summary>
public partial class StateProvince : BaseEntity, ILocalizedEntity
{
   /// <summary>
   /// Gets or sets the country identifier
   /// </summary>
   public long CountryId { get; set; }

   /// <summary>
   /// Gets or sets the name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the abbreviation
   /// </summary>
   public string Abbreviation { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the entity is published
   /// </summary>
   public bool Published { get; set; }

   /// <summary>
   /// Gets or sets the display order
   /// </summary>
   public int DisplayOrder { get; set; }

//   #region Navigation
//#pragma warning disable CS1591

//   public Country Country { get; set; }

//   public List<Address> Addresses { get; set; } = new();

//   public List<Currency> Currencies { get; set; } = new();

//   public List<CurrencyStateProvince> CurrencyStateProvinces { get; set; } = new();

//#pragma warning restore CS1591
//   #endregion
}
