using Shared.Common;

namespace Hub.Core.Domain.Localization;

/// <summary>
/// Represents a localized property
/// </summary>
public partial class LocalizedProperty : BaseEntity
{
   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   public long EntityId { get; set; }

   /// <summary>
   /// Gets or sets the language identifier
   /// </summary>
   public long LanguageId { get; set; }

   /// <summary>
   /// Gets or sets the locale key group
   /// </summary>
   public string LocaleKeyGroup { get; set; }

   /// <summary>
   /// Gets or sets the locale key
   /// </summary>
   public string LocaleKey { get; set; }

   /// <summary>
   /// Gets or sets the locale value
   /// </summary>
   public string LocaleValue { get; set; }
}
