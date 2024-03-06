using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Messages
{
   public partial record TestMessageTemplateModel : BaseAppEntityModel
   {
      public TestMessageTemplateModel()
      {
         Tokens = new List<string>();
      }

      public long LanguageId { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Test.Tokens")]
      public List<string> Tokens { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.Test.SendTo")]
      public string SendTo { get; set; }
   }
}