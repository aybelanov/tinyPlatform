using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Framework.Controllers;

/// <summary>
/// Represents a null view class
/// </summary>
public class NullView : IView
{
#pragma warning disable CS1591

   public static readonly NullView Instance = new();

   public string Path => string.Empty;

   public Task RenderAsync(ViewContext context)
   {
      if (context == null)
         throw new ArgumentNullException(nameof(context));

      return Task.CompletedTask;
   }

#pragma warning restore CS1591
}