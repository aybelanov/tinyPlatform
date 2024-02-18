﻿using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an event that occurs when system warnings are creating
/// </summary>
public class SystemWarningCreatedEvent
{
   #region Ctor

   public SystemWarningCreatedEvent()
   {
      SystemWarnings = new List<SystemWarningModel>();
   }

   #endregion

   #region Properties

   /// <summary>
   /// Gets a list of warnings
   /// </summary>
   public IList<SystemWarningModel> SystemWarnings { get; }

   #endregion
}