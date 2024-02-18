namespace Hub.Core.Security;

/// <summary>
/// Represents default values related to data protection
/// </summary>
public static partial class AppDataProtectionDefaults
{
   /// <summary>
   /// Gets the name of the key path used to store the protection key list to local file system (used when UseAzureBlobStorageToStoreDataProtectionKeys option not enabled)
   /// </summary>
   public static string DataProtectionKeysPath => "~/App_Data/DataProtectionKeys";

   /// <summary>
   /// Gets the name of the key path used to store the RSA private key list to local file system
   /// </summary>
   public static string SecretRsaKeysFilePath => "~/App_Data/DataProtectionKeys/privatekey.rsa";//"~/App_Data/DataProtectionKeys/rsa.xml"; 

   /// <summary>
   /// Gets the name of the key path used to store the encription private key list to local file system
   /// </summary>
   public static string SymmetricSecretKeyFilePath => "~/App_Data/DataProtectionKeys/encryption.key";
}
