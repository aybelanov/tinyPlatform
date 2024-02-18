﻿using System.Collections.Generic;
using Hub.Core.Configuration;

namespace Hub.Core.Domain.Common;

/// <summary>
/// Common settings
/// </summary>
public class CommonSettings : ISettings
{
   /// <summary>
   /// Default Ctor
   /// </summary>
   public CommonSettings()
   {
      IgnoreLogWordlist = new List<string>();
   }

   /// <summary>
   /// Gets or sets a value indicating whether the contacts form should have "Subject"
   /// </summary>
   public bool SubjectFieldOnContactUsForm { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the contacts form should use system email
   /// </summary>
   public bool UseSystemEmailForContactUsForm { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to display a warning if java-script is disabled
   /// </summary>
   public bool DisplayJavaScriptDisabledWarning { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether 404 errors (page or file not found) should be logged
   /// </summary>
   public bool Log404Errors { get; set; }

   /// <summary>
   /// Gets or sets a breadcrumb delimiter used on the site
   /// </summary>
   public string BreadcrumbDelimiter { get; set; }

   /// <summary>
   /// Gets or sets ignore words (phrases) to be ignored when logging errors/messages
   /// </summary>
   public List<string> IgnoreLogWordlist { get; set; }

   /// <summary>
   /// Gets or sets the number of days that will be saved in the log when it is cleared
   /// when the corresponding task is performed according to the schedule.
   /// Set to 0 if you want to clear the entire log in its entirety
   /// </summary>
   public int ClearLogOlderThanDays { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether links generated by BBCode Editor should be opened in a new window
   /// </summary>
   public bool BbcodeEditorOpenLinksInNewWindow { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether "accept terms of service" links should be open in popup window. If disabled, then they'll be open on a new page.
   /// </summary>
   public bool PopupForTermsOfServiceLinks { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether jQuery migrate script logging is active
   /// </summary>
   public bool JqueryMigrateScriptLoggingActive { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to compress response (gzip by default). 
   /// You may want to disable it, for example, If you have an active IIS Dynamic Compression Module configured at the server level
   /// </summary>
   public bool UseResponseCompression { get; set; }

   /// <summary>
   /// Gets or sets a value of favicon and app icons <head/> code
   /// </summary>
   public string FaviconAndAppIconsHeadCode { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to enable markup minification
   /// </summary>
   public bool EnableHtmlMinification { get; set; }

   /// <summary>
   /// Gets or sets the timeout (in milliseconds) before restarting the application; set null to use default value
   /// </summary>
   public int? RestartTimeout { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the images can be downloaded from remote server
   /// </summary>
   public bool ExportImportAllowDownloadImages { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the related entities need to be exported/imported using name
   /// </summary>
   public bool ExportImportRelatedEntitiesByName { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to ignore ACL rules (side-wide). It can significantly improve performance when enabled.
   /// </summary>
   public bool IgnoreAcl { get; set; }

   /// <summary>
   /// Site searched enabled
   /// </summary>
   public bool SearchEnabled { get; set; }

   /// <summary>
   /// Get or set a value indicating whether use standart menu in public platform or use Ajax to load menu
   /// </summary>
   public bool UseAjaxLoadMenu { get; set; }
}
