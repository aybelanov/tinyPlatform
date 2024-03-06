using Hub.Core;
using Hub.Core.Security;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hub.Services.Security;

/// <summary>
/// Encryption helper
/// </summary>
public static class EncryptionHelper
{
   /// <summary>
   /// RSA security key for signinig
   /// </summary>
   public static RsaSecurityKey RsaSecurityKey => _rsaSecuritykey ??= GenerateOrRebuildRsaSecurityKey();
   private static RsaSecurityKey _rsaSecuritykey;

   /// <summary>
   /// Symetric key for encription
   /// </summary>
   public static SymmetricSecurityKey SymmetricSecurityKey => _symmetricSecurityKey ??= GenerateOrRebuildSymetricSecurityKey();
   private static SymmetricSecurityKey _symmetricSecurityKey;

   /// <summary>
   /// Generate or rebuild Rsa security key
   /// </summary>
   /// <returns></returns>
   /// <exception cref="NotImplementedException"></exception>
   public static RsaSecurityKey GenerateOrRebuildRsaSecurityKey()
   {
      var filePath = CommonHelper.DefaultFileProvider.MapPath(AppDataProtectionDefaults.SecretRsaKeysFilePath);
      var rsaKey = RSA.Create();
      if (!File.Exists(filePath))
      {
         File.WriteAllBytes(filePath, rsaKey.ExportRSAPrivateKey());
         //File.WriteAllText(filePath, rsaKey.ToXmlString(true));
      }
      else
      {
         var bytes = File.ReadAllBytes(filePath);
         rsaKey.ImportRSAPrivateKey(bytes, out _);
         //var xml = File.ReadAllText(filePath);
         //rsaKey.FromXmlString(xml);
      }

      var key = new RsaSecurityKey(rsaKey);
      _rsaSecuritykey = key;

      return key;
   }


   /// <summary>
   /// Generate or rebuild a symetric security security key
   /// </summary>
   /// <returns></returns>
   /// <exception cref="NotImplementedException"></exception>
   public static SymmetricSecurityKey GenerateOrRebuildSymetricSecurityKey()
   {
      var filePath = CommonHelper.DefaultFileProvider.MapPath(AppDataProtectionDefaults.SymmetricSecretKeyFilePath);

      var secretKey = new byte[32];

      if (!File.Exists(filePath))
      {
         using var generator = new SecureRandomNumberGenerator();
         generator.GetBytes(secretKey);
         File.WriteAllBytes(filePath, secretKey);
      }
      else
      {
         secretKey = File.ReadAllBytes(filePath);
      }

      var key = new SymmetricSecurityKey(secretKey);
      _symmetricSecurityKey = key;

      return key;
   }

   /// <summary>
   /// Creates the hash for the various hash claims (e.g. c_hash, at_hash or s_hash).
   /// </summary>
   /// <param name="value">The value to hash.</param>
   /// <param name="tokenSigningAlgorithm">The token signing algorithm</param>
   /// <returns></returns>
   public static string CreateHashClaimValue(string value, string tokenSigningAlgorithm)
   {
      using (var sha = GetHashAlgorithmForSigningAlgorithm(tokenSigningAlgorithm))
      {
         var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(value));
         var size = (sha.HashSize / 8) / 2;

         var leftPart = new byte[size];
         Array.Copy(hash, leftPart, size);

         return Convert.ToBase64String(leftPart);
      }
   }

   /// <summary>
   /// Returns the matching hashing algorithm for a token signing algorithm
   /// </summary>
   /// <param name="signingAlgorithm">The signing algorithm</param>
   /// <returns></returns>
   public static HashAlgorithm GetHashAlgorithmForSigningAlgorithm(string signingAlgorithm)
   {
      var signingAlgorithmBits = int.Parse(signingAlgorithm.Substring(signingAlgorithm.Length - 3));

      return signingAlgorithmBits switch
      {
         256 => SHA256.Create(),
         384 => SHA384.Create(),
         512 => SHA512.Create(),
         _ => throw new InvalidOperationException($"Invalid signing algorithm: {signingAlgorithm}"),
      };
   }
}
