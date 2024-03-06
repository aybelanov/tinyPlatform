using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hub.Web.Areas.OpenId.Models;

public partial record class JwksModel
{
   public JwksModel()
   {
      Keys = new List<JsonWebKey>();
   }

   [JsonProperty(PropertyName = "keys")]
   public IList<JsonWebKey> Keys { get; set; }

   public partial record JsonWebKey
   {
      [JsonProperty(PropertyName = "alg")]
      public string Algorithm { get; set; }

      [JsonProperty(PropertyName = "use")]
      public string KeyUse { get; set; }

      [JsonProperty(PropertyName = "kty")]
      public string KeyType { get; set; }

      [JsonProperty(PropertyName = "kid")]
      public string KeyId { get; set; }

      [JsonProperty(PropertyName = "e")]
      public string PublicExponent { get; set; }

      [JsonProperty(PropertyName = "n")]
      public string Modulus { get; set; }
   }
}
