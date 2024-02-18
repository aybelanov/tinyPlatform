using Hub.Core.Domain.Blogs;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.News;
using Hub.Core.Domain.Polls;
using Hub.Core.Domain.Seo;
using Shared.Common;
using System.Collections.Generic;

namespace Hub.Core.Domain.Localization;

/// <summary>
/// Represents a language
/// </summary>
public partial class Language : BaseEntity
{
   /// <summary>
   /// Gets or sets the name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the language culture
   /// </summary>
   public string LanguageCulture { get; set; }

   /// <summary>
   /// Gets or sets the unique SEO code
   /// </summary>
   public string UniqueSeoCode { get; set; }

   /// <summary>
   /// Gets or sets the flag image file name
   /// </summary>
   public string FlagImageFileName { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the language supports "Right-to-left"
   /// </summary>
   public bool Rtl { get; set; }

   /// <summary>
   /// Gets or sets the identifier of the default currency for this language; 0 is set when we use the default currency display order
   /// </summary>
   public long DefaultCurrencyId { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the language is published
   /// </summary>
   public bool Published { get; set; }

   /// <summary>
   /// Gets or sets the display order
   /// </summary>
   public int DisplayOrder { get; set; }


//   #region Navigation
//#pragma warning disable CS1591

//   //public Currency DefaultCurrency { get; set; }
//   public List<LocaleStringResource> LocaleStringResources { get; set; } = new();
//   public List<LocalizedProperty> LocalizedProperties { get; set; } = new();
//   //public List<UrlRecord> UrlRecords { get; set; }
//   public List<BlogPost> BlogPosts { get; set; } = new();   
//   public List<NewsItem> NewsItems { get; set; } = new(); 
//   public List<Poll> Polls { get; set; } = new();  

//#pragma warning restore CS1591
//   #endregion
}
