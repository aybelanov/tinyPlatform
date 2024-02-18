using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace Clients.Dash.Services.Localization;

/// <summary>
/// Represents a localizer for static resource
/// </summary>
public class Localizer : IStringLocalizer<Localizer>
{
   #region fields

   private readonly IStringLocalizer _localizer;

   #endregion

   #region Ctors

   /// <summary>
   /// Creates a new <see cref="StringLocalizer{TResourceSource}"/>.
   /// </summary>
   /// <param name="factory">The <see cref="IStringLocalizerFactory"/> to use.</param>
   public Localizer(IStringLocalizerFactory factory)
   {
      if (factory == null)
         throw new ArgumentNullException(nameof(factory));


      _localizer = factory.Create(typeof(Localizer));
   }

   #endregion

   #region Indexers

   /// <inheritdoc />
   public virtual LocalizedString this[string name]
   {
      get
      {
         ArgumentNullException.ThrowIfNull(name);

         return _localizer[name];
      }
   }

   /// <inheritdoc />
   public virtual LocalizedString this[string name, params object[] arguments]
   {
      get
      {
         ArgumentNullException.ThrowIfNull(name);

         return _localizer[name, arguments];
      }
   }

   #endregion

   #region Methods

   /// <inheritdoc />
   public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);

   #endregion
}
