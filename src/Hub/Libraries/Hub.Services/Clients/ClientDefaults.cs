namespace Hub.Services.Clients;

/// <summary>
/// Represents telemetry defaults
/// </summary>
public static class ClientDefaults
{
   /// <summary>
   /// Data report file expired time (in minutes)
   /// </summary>
   public static int ReportFileExpired => 60;

   /// <summary>
   /// Report data file directory
   /// </summary>
   public static string ReportFilleDirectory => "files\\reports";

   /// <summary>
   /// Directory of the video segment storage
   /// </summary>
   public static string VideoStorageDirectory => "files/ipcam";

   /// <summary>
   /// Endpoint for IP Cam files and streams 
   /// </summary>
   public static string IpCamEndpoint => "/ipcam";


   /// <summary>
   /// SignalR endpoint for the client app connection
   /// </summary>
   public static string SignalrEndpoint => "/dashhub";

   /// <summary>
   /// REST api end point
   /// </summary>
   public static string WebApiEndpoint => "/webapi";

   /// <summary>
   /// gRPC client contract pakage
   /// </summary>
   public static string GrpcClientContract => "clientpackage";

   /// <summary>
   /// gRPC client contract pakage
   /// </summary>
   public static string GrpcDeviceContract => "devicepackage";

   /// <summary>
   /// It uses to define the gRPC requests 
   /// and has to be the same as gRPC package values in proto files
   /// </summary>
   public static string[] GrpcContracts => new[] { GrpcClientContract, GrpcDeviceContract };

   /// <summary>
   /// Device configuration version (include sensors)
   /// </summary>
   public static string DeviceConfigurationVersion => "DeviceConfigurationVersion";
}
