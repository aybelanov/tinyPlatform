using Shared.Common;

namespace Hub.Core.Domain.Localization;

/// <summary>
/// Represents a locale string resource
/// </summary>
public partial class LocaleStringResource : BaseEntity
{
   /// <summary>
   /// Gets or sets the language identifier
   /// </summary>
   public long LanguageId { get; set; }

   /// <summary>
   /// Gets or sets the resource name
   /// </summary>
   public string ResourceName { get; set; }

   /// <summary>
   /// Gets or sets the resource value
   /// </summary>
   public string ResourceValue { get; set; }

   //   #region Navigation
   //#pragma warning disable CS1591

   //   public Language Language { get; set; }

   //#pragma warning restore CS1591
   //   #endregion
}
