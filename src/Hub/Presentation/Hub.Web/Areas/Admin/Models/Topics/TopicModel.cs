using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Topics
{
   /// <summary>
   /// Represents a topic model
   /// </summary>
   public partial record TopicModel : BaseAppEntityModel, IAclSupportedModel, ILocalizedModel<TopicLocalizedModel>
   {
      #region Ctor

      public TopicModel()
      {
         AvailableTopicTemplates = new List<SelectListItem>();
         Locales = new List<TopicLocalizedModel>();

         SelectedUserRoleIds = new List<long>();
         AvailableUserRoles = new List<SelectListItem>();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.SystemName")]
      public string SystemName { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInSitemap")]
      public bool IncludeInSitemap { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInTopMenu")]
      public bool IncludeInTopMenu { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn1")]
      public bool IncludeInFooterColumn1 { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn2")]
      public bool IncludeInFooterColumn2 { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn3")]
      public bool IncludeInFooterColumn3 { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.DisplayOrder")]
      public int DisplayOrder { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.AccessibleWhenPlatformClosed")]
      public bool AccessibleWhenPlatformClosed { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.IsPasswordProtected")]
      public bool IsPasswordProtected { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.Password")]
      public string Password { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.URL")]
      public string Url { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
      public string Title { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
      public string Body { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.Published")]
      public bool Published { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.TopicTemplate")]
      public int TopicTemplateId { get; set; }

      public IList<SelectListItem> AvailableTopicTemplates { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
      public string MetaKeywords { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
      public string MetaDescription { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
      public string MetaTitle { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
      public string SeName { get; set; }

      public IList<TopicLocalizedModel> Locales { get; set; }

      //ACL (user roles)
      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.AclUserRoles")]
      public IList<long> SelectedUserRoleIds { get; set; }

      public IList<SelectListItem> AvailableUserRoles { get; set; }

      public string TopicName { get; set; }

      #endregion
   }

   public partial record TopicLocalizedModel : ILocalizedLocaleModel
   {
      public long LanguageId { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
      public string Title { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
      public string Body { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
      public string MetaKeywords { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
      public string MetaDescription { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
      public string MetaTitle { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
      public string SeName { get; set; }
   }
}