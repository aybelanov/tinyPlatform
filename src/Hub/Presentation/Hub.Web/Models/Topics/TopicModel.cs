﻿using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Topics
{
   public partial record TopicModel : BaseAppEntityModel
   {
      public string SystemName { get; set; }

      public bool IncludeInSitemap { get; set; }

      public bool IsPasswordProtected { get; set; }

      public string Title { get; set; }

      public string Body { get; set; }

      public string MetaKeywords { get; set; }

      public string MetaDescription { get; set; }

      public string MetaTitle { get; set; }

      public string SeName { get; set; }

      public long TopicTemplateId { get; set; }
   }
}