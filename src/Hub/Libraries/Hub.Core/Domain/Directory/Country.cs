using Hub.Core.Domain.Localization;
using Shared.Common;

namespace Hub.Core.Domain.Directory;

/// <summary>
/// Represents a country
/// </summary>
public partial class Country : BaseEntity, ILocalizedEntity
{
   /// <summary>
   /// Gets or sets the name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether billing is allowed to this country
   /// </summary>
   public bool AllowsBilling { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether shipping is allowed to this country
   /// </summary>
   public bool AllowsShipping { get; set; }

   /// <summary>
   /// Gets or sets the two letter ISO code
   /// </summary>
   public string TwoLetterIsoCode { get; set; }

   /// <summary>
   /// Gets or sets the three letter ISO code
   /// </summary>
   public string ThreeLetterIsoCode { get; set; }

   /// <summary>
   /// Gets or sets the numeric ISO code
   /// </summary>
   public int NumericIsoCode { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether users in this country must be charged EU VAT
   /// </summary>
   public bool SubjectToVat { get; set; }

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

   //   public List<StateProvince> StateProvinces { get; set; } = new();

   //#pragma warning restore CS1591
   //   #endregion
}