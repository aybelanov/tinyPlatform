using System;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hub.Web.Framework.Mvc.ModelBinding.Binders;

/// <summary>
/// Represents model binder for numeric types
/// </summary>
public class InvariantNumberModelBinder : IModelBinder
{

   #region Fields

   private delegate bool TryParseNumber<T>(string value, NumberStyles styles, IFormatProvider provider, out T result) where T : struct;
   private readonly IModelBinder _globalizedBinder;
   private readonly NumberStyles _supportedStyles;

   #endregion

   #region Ctor

   /// <summary> Param Ctor </summary>
   public InvariantNumberModelBinder(NumberStyles supportedStyles, IModelBinder globalizedBinder)
   {
      _globalizedBinder = globalizedBinder ?? throw new ArgumentNullException(nameof(globalizedBinder));
      _supportedStyles = supportedStyles;
   }

   #endregion

   #region Utils

   private T? TryParse<T>(string value, TryParseNumber<T> handler) where T : struct
   {
      if (!string.IsNullOrWhiteSpace(value) && handler(value, _supportedStyles, CultureInfo.InvariantCulture, out var parseResult))
         return parseResult;

      return null;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Attempts to bind a model
   /// </summary>
   /// <param name="bindingContext">Model binding context</param>
   public Task BindModelAsync(ModelBindingContext bindingContext)
   {
      if (bindingContext is null)
         throw new ArgumentNullException(nameof(bindingContext));

      var modelName = bindingContext.ModelName;
      var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

      if (valueProviderResult == ValueProviderResult.None)
         return Task.CompletedTask;

      bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

      var value = valueProviderResult.FirstValue;

      if (string.IsNullOrEmpty(value))
         return Task.CompletedTask;

      value = value.Replace('/', '.'); //workaround for a kendonumeric input

      object model = bindingContext.ModelMetadata.UnderlyingOrModelType switch
      {
         Type t when t == typeof(float) => TryParse<float>(value, float.TryParse),
         Type t when t == typeof(decimal) => TryParse<decimal>(value, decimal.TryParse),
         Type t when t == typeof(double) => TryParse<double>(value, double.TryParse),
         Type t when t == typeof(int) => TryParse<int>(value, int.TryParse),
         Type t when t == typeof(long) => TryParse<long>(value, long.TryParse),
         Type t when t == typeof(short) => TryParse<short>(value, short.TryParse),
         Type t when t == typeof(sbyte) => TryParse<sbyte>(value, sbyte.TryParse),
         Type t when t == typeof(byte) => TryParse<byte>(value, byte.TryParse),
         Type t when t == typeof(ulong) => TryParse<ulong>(value, ulong.TryParse),
         Type t when t == typeof(ushort) => TryParse<ushort>(value, ushort.TryParse),
         Type t when t == typeof(uint) => TryParse<uint>(value, uint.TryParse),
         Type t when t == typeof(BigInteger) => TryParse<BigInteger>(value, BigInteger.TryParse),
         _ => null
      };

      if (model is null)
         return _globalizedBinder.BindModelAsync(bindingContext); //attempt to bind a model depending on the current culture

      bindingContext.Result = ModelBindingResult.Success(model);

      return Task.CompletedTask;
   }

   #endregion
}