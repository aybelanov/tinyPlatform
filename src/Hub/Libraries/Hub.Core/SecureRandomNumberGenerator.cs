using System;
using System.Security.Cryptography;

namespace Hub.Core;

/// <summary>
/// Represents the class implementation of cryptographic random number generator derive
/// </summary>
public partial class SecureRandomNumberGenerator : RandomNumberGenerator
{
   #region Field

   private bool _disposed = false;
   private readonly RandomNumberGenerator _rng;

   #endregion

   #region Ctor

   /// <summary>
   /// Default Ctor
   /// </summary>
   public SecureRandomNumberGenerator()
   {
      _rng = Create();
   }

   #endregion

   #region Methods

   /// <summary>
   /// Gets random int
   /// </summary>
   /// <returns>random int</returns>
   public int Next()
   {
      var data = new byte[sizeof(int)];
      _rng.GetBytes(data);
      return BitConverter.ToInt32(data, 0) & int.MaxValue - 1;
   }

   /// <summary>
   /// Gets random int
   /// </summary>
   /// <param name="maxValue">Max value</param>
   /// <returns>Random int</returns>
   public int Next(int maxValue)
   {
      return Next(0, maxValue);
   }

   /// <summary>
   /// Gets random int
   /// </summary>
   /// <param name="minValue">Min value</param>
   /// <param name="maxValue">Max value</param>
   /// <returns>Random int</returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public int Next(int minValue, int maxValue)
   {
      if (minValue > maxValue)
      {
         throw new ArgumentOutOfRangeException();
      }
      return (int)Math.Floor(minValue + ((double)maxValue - minValue) * NextDouble());
   }

   /// <summary>
   /// Gets random double
   /// </summary>
   /// <returns>random double</returns>
   public double NextDouble()
   {
      var data = new byte[sizeof(uint)];
      _rng.GetBytes(data);
      var randUint = BitConverter.ToUInt32(data, 0);
      return randUint / (uint.MaxValue + 1.0);
   }

   /// <inheritdoc cref="RandomNumberGenerator.GetBytes(byte[])"/>
   public override void GetBytes(byte[] data)
   {
      _rng.GetBytes(data);
   }

   /// <inheritdoc cref="RandomNumberGenerator.GetNonZeroBytes(byte[])"/>
   public override void GetNonZeroBytes(byte[] data)
   {
      _rng.GetNonZeroBytes(data);
   }

   /// <summary>
   /// Dispose secure random
   /// </summary>
   public new void Dispose()
   {
      Dispose(true);
      GC.SuppressFinalize(this);
   }

   /// <remarks> Protected implementation of Dispose pattern. </remarks>
   protected override void Dispose(bool disposing)
   {
      if (_disposed)
         return;

      if (disposing)
      {
         _rng?.Dispose();
      }

      _disposed = true;
   }

   #endregion
}
