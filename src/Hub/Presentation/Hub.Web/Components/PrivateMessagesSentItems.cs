using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class PrivateMessagesSentItemsViewComponent : AppViewComponent
{
   private readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;

   public PrivateMessagesSentItemsViewComponent(IPrivateMessagesModelFactory privateMessagesModelFactory)
   {
      _privateMessagesModelFactory = privateMessagesModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync(int pageNumber, string tab)
   {
      var model = await _privateMessagesModelFactory.PrepareSentModelAsync(pageNumber, tab);
      return View(model);
   }
}
