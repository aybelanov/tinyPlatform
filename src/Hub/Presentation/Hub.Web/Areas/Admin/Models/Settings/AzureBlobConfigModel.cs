using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents Azure Blob storage configuration model
/// </summary>
public partial record AzureBlobConfigModel : BaseAppModel, IConfigModel
{
   #region Properties

   [AppResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.ConnectionString")]
   public string ConnectionString { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.ContainerName")]
   public string ContainerName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.EndPoint")]
   public string EndPoint { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.AppendContainerName")]
   public bool AppendContainerName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.StoreDataProtectionKeys")]
   public bool StoreDataProtectionKeys { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysContainerName")]
   public string DataProtectionKeysContainerName { get; set; }

   [AppResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysVaultId")]
   public string DataProtectionKeysVaultId { get; set; }

   #endregion
}