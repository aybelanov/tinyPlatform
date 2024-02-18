using Newtonsoft.Json;

namespace Hub.Web.Areas.OpenId.Models;

/// <summary>
/// Represents a token model for devices
/// </summary>
public partial record DeviceTokenInfoModel
{
   /// <summary>
   /// Access token
   /// </summary>
   [JsonProperty(PropertyName = "access_token")]
   public string AccessToken { get; set; }

   /// <summary>
   /// Token create timestamp
   /// </summary>
   [JsonProperty(PropertyName = "created_on")]
   public long CreatedOn { get; set; }

   /// <summary>
   /// Token expires in (in seonds)
   /// </summary>
   [JsonProperty(PropertyName = "expires_in")]
   public int ExpiresIn { get; set; }

   /// <summary>
   /// Token type 
   /// </summary>
   [JsonProperty(PropertyName = "token_type")]
   public string TokenType { get; set; }
}
