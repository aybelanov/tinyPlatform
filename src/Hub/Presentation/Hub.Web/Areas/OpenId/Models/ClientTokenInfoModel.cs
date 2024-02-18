using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hub.Web.Areas.OpenId.Models;

/// <summary>
/// Represents token model for clients
/// </summary>
public partial record ClientTokenInfoModel
{
   /// <summary>
   /// Access token
   /// </summary>
   [JsonProperty(PropertyName = "access_token")]
   public string AccessToken { get; set; }

   /// <summary>
   /// Id token
   /// </summary>
   [JsonProperty(PropertyName = "id_token")]
   public string IdToken { get; set; }

   /// <summary>
   /// Token create timestamp
   /// </summary>
   [JsonIgnore]
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

   /// <summary>
   /// Scope
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "scope")]
   public IEnumerable<string> Scope { get; set; }
}
