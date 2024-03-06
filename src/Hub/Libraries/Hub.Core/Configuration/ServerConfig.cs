using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Hub.Core.Configuration;

/// <summary>
/// Represents Kestrel configuration
/// </summary>
public partial class ServerConfig : IConfig
{
   /// <inheritdoc/>
   public bool HasOwnSection() => false;

   ///// <summary>
   ///// Listen urls
   ///// </summary>
   //public string Urls { get; private set; } = "http://*:5000;https://*:5001";

   /// <summary>
   /// Endpoint confuguration
   /// </summary>
   public JToken Kestrel { get; private set; } = JToken.Parse(
   """
    {
      "Endpoints": {
        "Http": {
           "Url": "http://*:5000"
        },
        "Https": {
           "Url": "https://*:5001"
        }
      }
    }
    """);

   /// <summary>
   /// Allowed hosts
   /// </summary>
   public string AllowedHosts { get; private set; } = "*";

   /// <summary>
   /// Default logging configuration
   /// </summary>
   public object Logging { get; private set; } = new Dictionary<string, object>()
   {
      {
         "Loglevel", new Dictionary<string, object>()
         {
            { "Default", "Information" },
            { "Microsoft.AspNetCore", "Information" },
            { "Microsoft.EntityFrameworkCore.Database.Command", "Warning" },
            { "Microsoft.EntityFrameworkCore.Infrastructure", "Warning" }
         }
      }
   };
}