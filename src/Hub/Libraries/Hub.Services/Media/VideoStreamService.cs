using Hub.Core;
using Hub.Core.Configuration;
using Hub.Core.Infrastructure;
using Hub.Services.Clients;
using Hub.Services.Clients.Records;
using Hub.Services.Devices;
using Hub.Services.Users;
using Shared.Clients;
using Shared.Common;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hub.Services.Media;

/// <summary>
/// Represents a video stream service implementation
/// </summary>
public partial class VideoStreamService : IVideoStreamService
{
   #region fields

   private readonly IHubDeviceService _deviceService;
   private readonly IHubSensorService _sensorService;
   private readonly IAppFileProvider _fileProvider;
   private readonly IWorkContext _workContext;
   private readonly ISensorRecordService _sensorRecordService;
   private readonly IUserService _userService;
   private readonly AppSettings _appSettings;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public VideoStreamService(IHubDeviceService deviceService,
      IHubSensorService sensorService,
      AppSettings appSettings,
      IUserService userService,
      IAppFileProvider fileProvider,
      ISensorRecordService sensorRecordService,
      IWorkContext workContext)
   {
      _deviceService = deviceService;
      _userService = userService;
      _sensorService = sensorService;
      _appSettings = appSettings;
      _sensorRecordService = sensorRecordService;
      _fileProvider = fileProvider;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   ///// <summary>
   ///// Saves video segment
   ///// </summary>
   ///// <param name="metadata">Segment file metadata</param>
   ///// <param name="segment">Video segment</param>
   ///// <returns></returns>
   //public virtual async Task SaveVideoSegment(SegmentMetadata metadata, VideoSegmentProto segment)
   //{
   //   //if (!_fileProvider.DirectoryExists(metadata.DestinationDirectory))
   //   //   _fileProvider.CreateDirectory(metadata.DestinationDirectory);

   //   //await File.WriteAllBytesAsync(metadata.FullFileName, bytes);

   //   await _sensorRecordService.InsertVideoSegmentAsync(new()
   //   {
   //      IpcamId = metadata.Source.Id,
   //      OnCreatedUtc = metadata.CreationOnUtc,
   //      InboundName = metadata.FileName,
   //      Extinf = metadata.Duration,
   //      OnReceivedUtc = DateTime.UtcNow,
   //      Resolution = segment.Resolution,
   //   },
   //   new() { Binary = segment.Bytes.ToByteArray() });
   //}

   /// <summary>
   /// Gets file metadat
   /// </summary>
   /// <param name="fileName">Segment file name</param>
   /// <returns>HLS file metadata</returns>
   public virtual async Task<SegmentMetadata> GetFileMetadataAsync(string fileName)
   {
      var sensorId = long.Parse(fileName.Split('-')[0]);
      var ipCam = await _sensorService.GetSensorByIdAsync(sensorId);

      if (ipCam == null)
         throw new AppException("Video source is not registered.");

      var videoStoragePath = _fileProvider.GetAbsolutePath(ClientDefaults.VideoStorageDirectory);
      var destDir = _fileProvider.Combine(videoStoragePath, sensorId.ToString());

      return new SegmentMetadata()
      {
         Source = ipCam,
         DestinationDirectory = destDir,
         FullFileName = _fileProvider.Combine(destDir, fileName),
         Extension = _fileProvider.GetFileExtension(fileName),
         FileName = fileName,
         IsFileExist = _fileProvider.FileExists(_fileProvider.Combine(destDir, fileName))
      };
   }

   /// <summary>
   /// Checks current user to acces this video segment
   /// </summary>
   /// <param name="fileName">Video segment file name</param>
   /// <returns>Access result</returns>
   public virtual async Task<bool> CheckUserAccesAsync(string fileName)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var sensorId = long.Parse(fileName.Split('-')[0]);
      var ipCam = await _sensorService.GetSensorByIdAsync(sensorId);

      var ownDevices = await _deviceService.GetOwnDevicesByUserIdAsync(user.Id);
      var sharedDevices = await _deviceService.GetSharedDeviceByUserIdAsync(user.Id);

      var result = ownDevices.Select(x => x.Id).Union(sharedDevices.Select(x => x.Id)).Contains(ipCam.DeviceId);

      return result;
   }

   /// <summary>
   /// Creates ipcam playlist by the datetime interval
   /// </summary>
   /// <param name="id">Ipcam identifier</param>
   /// <param name="from">Period "from"</param>
   /// <param name="to">Period "to"</param>
   /// <returns>Playlist as string</returns>
   public async Task<string> CreatePlaylistAsync(long id, long from, long to)
   {
      var filter = new DynamicFilter()
      {
         SensorId = id,
         From = from.ToDateTimeFromUinxEpoch(),
         To = to.ToDateTimeFromUinxEpoch()
      };
      var user = await _workContext.GetCurrentUserAsync();
      if (!await _userService.IsAdminAsync(user))
         filter.UserId = user.Id;

      var segments = await _sensorRecordService.GetSegmentsByFilterAsync(filter);

      if (!segments.Any())
         return string.Empty;

      var targetDuration = (int)Math.Round(segments.Max(x => x.Extinf), 0, MidpointRounding.ToPositiveInfinity);

      var header =
         $"""
         #EXTM3U
         #EXT-X-VERSION:3
         #EXT-X-TARGETDURATION:{targetDuration.ToString(CultureInfo.InvariantCulture)}
         #EXT-X-MEDIA-SEQUENCE:5320
         """ + Environment.NewLine;

      var m3u8List = header;
      var baseUrl = _appSettings.Get<HostingConfig>().HubHostUrl + "/ipcam";
      foreach (var segment in segments)
      {
         m3u8List += $"#EXTINF:{segment.Extinf}";
         m3u8List += Environment.NewLine;
         m3u8List += $"{baseUrl}/{segment.IpcamId}/{segment.InboundName}";
         m3u8List += Environment.NewLine;
      }

      return m3u8List;
   }

   #endregion


   #region Props

   [GeneratedRegex("#EXTINF:(?<value>.*?),")]
   private static partial Regex ExtinfRegex();

   #endregion
}