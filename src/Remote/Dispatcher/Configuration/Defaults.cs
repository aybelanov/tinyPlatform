using System;
using System.IO;

namespace Devices.Dispatcher.Configuration;

/// <summary>
/// Dispatcher defaults
/// </summary>
public static class Defaults
{
   /// <summary>
   /// Currennt a dispather application version
   /// </summary>
   public const string ClientVersion = "0.20.01-beta";

   /// <summary>
   /// SQLite connection string
   /// </summary>
   public static string ConnectionString => "DataSource=app.db";

   /// <summary>
   /// Http client to get access tokens
   /// </summary>
   public static string TokenHttpClient => "TokenRequestClient";

   /// <summary>
   /// Http client for grpc clients
   /// </summary>
   public static string HubGrpcHttpClient => "HubGrpcHttpClient";

   /// <summary>
   /// Http client for communication to hub
   /// </summary>
   public static string HubGrpcClient => "HubGrpcClient";

   /// <summary>
   /// Unique current procces identifier
   /// </summary>
   public static Guid UniqueProcessGuid { get; set; }

   /// <summary>
   /// Vieo directory storege path
   /// </summary>
   public static string VideoSegmentsPath { get; set; }

   /// <summary>
   /// Inbound video sesment types
   /// </summary>
   public static string[] SegmentTypes => new[] { ".ts", ".m4s", ".webm" };
}
