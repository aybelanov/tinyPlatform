using Devices.Dispatcher.Services.Settings;
using System.Text.Json.Serialization;

namespace Devices.Dispatcher.Services.Authentication;

/// <summary>
/// Repesents a token store class
/// </summary>
public class TokenStore : ISettings
{
   /// <summary>
   /// Access token
   /// </summary>
   [JsonPropertyName("access_token")]
   public string AccessToken { get; set; }

   /// <summary>
   /// Token expires in
   /// </summary>
   [JsonPropertyName("expires_in")]
   public int ExpiresIn { get; set; }

   /// <summary>
   /// Token was created on
   /// </summary>
   [JsonPropertyName("created_on")]
   public long CreatedOn { get; set; }

   /// <summary>
   /// Token type
   /// </summary>
   [JsonPropertyName("token_type")]
   public string TokenType { get; set; }

   /// <summary>
   /// Scope
   /// </summary>
   [JsonPropertyName("scope")]
   public string Scope { get; set; }

   /// <summary>
   /// Error
   /// </summary>
   [JsonPropertyName("error")]
   public string Error { get; set; }
}
