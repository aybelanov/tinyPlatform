using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Gdpr;

 /// <summary>
 /// Represents a GDPR log search model
 /// </summary>
 public partial record GdprLogSearchModel : BaseSearchModel
 {
     #region Ctor

     public GdprLogSearchModel()
     {
         AvailableRequestTypes = new List<SelectListItem>();
     }

     #endregion

     #region Properties

     [AppResourceDisplayName("Admin.Users.GdprLog.List.SearchEmail")]
     [DataType(DataType.EmailAddress)]
     public string SearchEmail { get; set; }

     [AppResourceDisplayName("Admin.Users.GdprLog.List.SearchRequestType")]
     public int SearchRequestTypeId { get; set; }

     public IList<SelectListItem> AvailableRequestTypes { get; set; }

     #endregion
 }