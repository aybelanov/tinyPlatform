﻿using System.Collections.Generic;
using Hub.Core.Configuration;

namespace Hub.Core.Domain.Users;

/// <summary>
/// Multi-factor authentication settings
/// </summary>
public class MultiFactorAuthenticationSettings : ISettings
{
   #region Ctor

   /// <summary>
   /// Default Ctor
   /// </summary>
   public MultiFactorAuthenticationSettings()
   {
      ActiveAuthenticationMethodSystemNames = new List<string>();
   }

   #endregion

   /// <summary>
   /// Gets or sets system names of active multi-factor authentication methods
   /// </summary>
   public List<string> ActiveAuthenticationMethodSystemNames { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to force multi-factor authentication
   /// </summary>
   public bool ForceMultifactorAuthentication { get; set; }
}
