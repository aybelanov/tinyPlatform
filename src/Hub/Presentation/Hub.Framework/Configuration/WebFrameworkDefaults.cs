using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Microsoft.AspNetCore.Http;
using Shared.Common;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Hub.Web.Framework.Configuration;

/// <summary>
/// Defaults for Web.Framework
/// </summary>
public static class WebFrameworkDefaults
{
   #region Service features

   /// <summary>
   /// Uses service stub for exetrnal dependency
   /// </summary>
   /// <remarks>
   /// usage example:
   /// services.AddScopedFeature{ISomeService, SomeServiceStub, SomeService}(Defaults.UseSomeServiceStub);
   /// </remarks>
   public static string UseSomeServiceStub => nameof(UseSomeServiceStub);

   #endregion

   #region CORS

   /// <summary>
   /// CORS policy name
   /// </summary>
   /// <remarks> "__DefaultCorsPolicy" - it's default cors policy name into the asp.net core source code. Allows use app.UseCors() without name </remarks>
   public const string CorsPolicyName = "AppCorsPolicy";

   #endregion

   #region Common

   /// <summary>
   /// Solution name
   /// </summary>
   public static string SolutionName => "tinyPlatform";

   #endregion
}
