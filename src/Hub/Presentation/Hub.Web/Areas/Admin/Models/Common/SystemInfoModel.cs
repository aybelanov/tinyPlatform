using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Common;

public partial record SystemInfoModel : BaseAppModel
{
   public SystemInfoModel()
   {
      Headers = new List<HeaderModel>();
      LoadedAssemblies = new List<LoadedAssembly>();
   }

   [AppResourceDisplayName("Admin.System.SystemInfo.ASPNETInfo")]
   public string AspNetInfo { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.IsFullTrust")]
   public bool IsFullTrust { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.PlatformVersion")]
   public string AppVersion { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.OperatingSystem")]
   public string OperatingSystem { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.ServerLocalTime")]
   public DateTime ServerLocalTime { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.ServerTimeZone")]
   public string ServerTimeZone { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.UTCTime")]
   public DateTime UtcTime { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.CurrentUserTime")]
   public DateTime CurrentUserTime { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.CurrentStaticCacheManager")]
   public string CurrentStaticCacheManager { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.HTTPHOST")]
   public string HttpHost { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.Headers")]
   public IList<HeaderModel> Headers { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.LoadedAssemblies")]
   public IList<LoadedAssembly> LoadedAssemblies { get; set; }

   [AppResourceDisplayName("Admin.System.SystemInfo.AzureBlobStorageEnabled")]
   public bool AzureBlobStorageEnabled { get; set; }

   public partial record HeaderModel : BaseAppModel
   {
      public string Name { get; set; }
      public string Value { get; set; }
   }

   public partial record LoadedAssembly : BaseAppModel
   {
      public string FullName { get; set; }
      public string Location { get; set; }
      public bool IsDebug { get; set; }
      public DateTime? BuildDate { get; set; }
   }
}