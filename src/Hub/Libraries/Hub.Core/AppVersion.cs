namespace Hub.Core
{
   /// <summary>
   /// Represents application version
   /// </summary>
   public static class AppVersion
   {
      /// <summary>
      /// Gets the major platform version
      /// </summary>
      public const string CURRENT_VERSION = "0.20";

      /// <summary>
      /// Gets the minor platform version
      /// </summary>
      public const string MINOR_VERSION = "01-beta";

      /// <summary>
      /// Gets the full platform version
      /// </summary>
      public const string FULL_VERSION = CURRENT_VERSION + "." + MINOR_VERSION;
   }
}