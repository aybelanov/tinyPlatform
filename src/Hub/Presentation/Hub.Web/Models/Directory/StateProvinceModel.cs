using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Directory
{
   public partial record StateProvinceModel : BaseAppModel
   {
      public long id { get; set; }
      public string name { get; set; }
   }
}