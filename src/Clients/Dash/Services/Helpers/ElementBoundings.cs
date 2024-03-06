﻿namespace Clients.Dash.Services.Helpers;

/// <summary>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Element/getBoundingClientRect" />
/// </summary>
public class ElementBoundings
{
#pragma warning disable CS1591 

   public double X { get; set; }
   public double Y { get; set; }
   public double Width { get; set; }
   public double Height { get; set; }
   public double Top { get; set; }
   public double Bottom { get; set; }
   public double Right { get; set; }
   public double Left { get; set; }

#pragma warning restore CS1591
}
