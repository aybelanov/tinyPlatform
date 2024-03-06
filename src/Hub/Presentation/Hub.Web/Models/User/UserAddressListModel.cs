using Hub.Web.Framework.Models;
using Hub.Web.Models.Common;
using System.Collections.Generic;

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