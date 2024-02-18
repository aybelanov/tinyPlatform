using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Helpers
{
   public interface ITinyMceHelper
   {
      Task<string> GetTinyMceLanguageAsync();
   }
}