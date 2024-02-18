namespace Hub.Services.Installation
{
   /// <summary>
   /// Represents default values related to installation services
   /// </summary>
   public static partial class AppInstallationDefaults
   {
      /// <summary>
      /// Gets a request path to the install URL
      /// </summary>
      public static string InstallPath => "install";

      /// <summary>
      /// Gets a path to the localization resources file
      /// </summary>
      public static string LocalizationResourcesPath => "~/App_Data/Localization/";

      /// <summary>
      /// Gets a path to the installation sample images
      /// </summary>
      public static string SampleImagesPath => "images\\samples\\";

      /// <summary>
      /// Default localization file pattern
      /// </summary>
      public static string DefaultLocalizationFilePattern => @"^defaultLocale\.([a-z]{2}\-[A-Z]{2})\.xml$";

      /// <summary>
      /// Localization file pattern
      /// </summary>
      public static string LocalizationFilePattern => @"^locale\.([a-z]{2}\-[A-Z]{2})\.xml$";


   }
}