using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hub.Web.Areas.OpenId.Models;

public partial record UserInfoModel
{
   [JsonProperty(PropertyName = "name")]
   public string UserName { get; set; }

   [JsonProperty(PropertyName = "role")]
   public IEnumerable<string> Roles { get; set; }

   [JsonProperty(PropertyName = "sub")]
   public string SubjectId { get; set; }

   [JsonProperty(PropertyName = "scope")]
   public IEnumerable<string> Scope { get; set; }
}
