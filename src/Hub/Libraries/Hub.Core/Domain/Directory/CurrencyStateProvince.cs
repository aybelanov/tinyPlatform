using Shared.Common;

namespace Hub.Core.Domain.Directory;

/// <summary>
/// Represents a currency-to-stateprovince map class
/// </summary>
public class CurrencyStateProvince : BaseEntity
{
   /// <summary>
   /// Curency identifier
   /// </summary>
   public long CurrencyId { get; set; }

   /// <summary>
   /// State province identifier
   /// </summary>
   public long StateProvinceId { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public Currency Currency { get; set; }
   //   public StateProvince StateProvince { get; set; }

   //#pragma warning restore CS1591
   //   #endregion
}
