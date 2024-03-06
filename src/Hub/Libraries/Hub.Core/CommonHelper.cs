using Hub.Core.Infrastructure;

namespace Hub.Core
{
   /// <summary>
   /// Represents a common helper
   /// </summary>
   public partial class CommonHelper : Shared.Common.Helpers.CommonHelper
   {
      #region Methods

      /// <summary>
      /// Generate random digit code
      /// </summary>
      /// <param name="length">Length</param>
      /// <returns>Result string</returns>
      public static string GenerateRandomDigitCode(int length)
      {
         using var random = new SecureRandomNumberGenerator();
         var str = string.Empty;
         for (var i = 0; i < length; i++)
            str = string.Concat(str, random.Next(10).ToString());
         return str;
      }

      /// <summary>
      /// Returns an random integer number within a specified rage
      /// </summary>
      /// <param name="min">Minimum number</param>
      /// <param name="max">Maximum number</param>
      /// <returns>Result</returns>
      public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
      {
         using var random = new SecureRandomNumberGenerator();
         return random.Next(min, max);
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the default file provider
      /// </summary>
      public static IAppFileProvider DefaultFileProvider { get; set; }

      #endregion
   }
}
