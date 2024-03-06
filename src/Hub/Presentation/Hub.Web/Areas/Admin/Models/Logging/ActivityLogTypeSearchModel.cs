﻿using Hub.Web.Framework.Models;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Logging;

/// <summary>
/// Represents an activity log type search model
/// </summary>
public partial record ActivityLogTypeSearchModel : BaseSearchModel
{
   #region Properties       

   public string Subject { get; set; }

   public IList<ActivityLogTypeModel> ActivityLogTypeListModel { get; set; }

   #endregion
}