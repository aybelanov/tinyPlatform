using System.Collections.Generic;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

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