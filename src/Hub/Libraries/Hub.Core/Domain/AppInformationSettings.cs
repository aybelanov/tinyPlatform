using Hub.Core.Configuration;

namespace Hub.Core.Domain;

/// <summary>
/// Application information settings
/// </summary>
public class AppInfoSettings : ISettings
{
   /// <summary>
   /// Application (service) full name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Service (solution) owner company full name
   /// </summary>
   public string CompanyName { get; set; }

   /// <summary>
   /// Service (solution) owner company vat number
   /// </summary>
   public string CompanyVat { get; set; }

   /// <summary>
   /// Service (solution) owner company official web site adress
   /// </summary>
   public string CompanyWebsiteUrl { get; set; }

   /// <summary>
   /// Service (solution) owner company address
   /// </summary>
   public string CompanyAddress { get; set; }

   /// <summary>
   /// Service (solution) owner company phone number
   /// </summary>
   public string CompanyPhoneNumber { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether SSL is enabled
   /// </summary>
   public bool SslEnabled { get; set; }

   /// <summary>
   /// Gets or sets the comma separated list of possible HTTP_HOST values
   /// </summary>
   public string Hosts { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether server is closed
   /// </summary>
   public bool PlatformClosed { get; set; }

   /// <summary>
   /// Gets or sets a picture identifier of the logo. If 0, then the default one will be used
   /// </summary>
   public long LogoPictureId { get; set; }

   /// <summary>
   /// Gets or sets a default app theme
   /// </summary>
   public string DefaultAppTheme { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether users are allowed to select a theme
   /// </summary>
   public bool AllowUserToSelectTheme { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether we should display warnings about the new EU cookie law
   /// </summary>
   public bool DisplayEuCookieLawWarning { get; set; }

   /// <summary>
   /// Gets or sets a value of Facebook page URL of the site
   /// </summary>
   public string FacebookLink { get; set; }

   /// <summary>
   /// Gets or sets a value of Twitter page URL of the site
   /// </summary>
   public string TwitterLink { get; set; }

   /// <summary>
   /// Gets or sets a value of YouTube channel URL
   /// </summary>
   public string YoutubeLink { get; set; }

   /// <summary>
   /// Gets or sets a value of RuTube channel URL
   /// </summary>
   public string RutubeLink { get; set; }

   /// <summary>
   /// Gets or sets a value of vk.com URL
   /// </summary>
   public string VkLink { get; set; }

   /// <summary>
   /// Gets or sets a value of ok.ru URL 
   /// </summary>
   public string OkLink { get; set; }

   /// <summary>
   /// Gets or sets a value of Telegram channel URL of the site
   /// </summary>
   public string TelegramLink { get; set; }

   /// <summary>
   /// Gets or sets a value of GitHub project URL of the site
   /// </summary>
   public string GitHubLink { get; set; }

   /// <summary>
   /// Copyright
   /// </summary>
   public bool HidePoweredBy { get; set; }
}
