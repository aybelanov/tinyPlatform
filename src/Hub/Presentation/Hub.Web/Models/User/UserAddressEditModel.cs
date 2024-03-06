using Hub.Web.Framework.Models;
using Hub.Web.Models.Common;

namespace Hub.Web.Models.User
{
   public partial record UserAddressEditModel : BaseAppModel
   {
      public UserAddressEditModel()
      {
         Address = new AddressModel();
      }

      public AddressModel Address { get; set; }
   }
}