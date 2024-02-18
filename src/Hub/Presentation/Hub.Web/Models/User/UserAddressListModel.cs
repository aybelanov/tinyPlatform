using System.Collections.Generic;
using Hub.Web.Models.Common;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.User
{
   public partial record UserAddressListModel : BaseAppModel
   {
      public UserAddressListModel()
      {
         Addresses = new List<AddressModel>();
      }

      public IList<AddressModel> Addresses { get; set; }
   }
}