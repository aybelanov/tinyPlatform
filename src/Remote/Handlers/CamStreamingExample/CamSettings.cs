namespace CamStreamingExample;

/// <summary>
/// Represents a video stream settings
/// </summary>
public class VideoStreamConvertationSettings
{
   /// <summary>
   /// IPCam source url
   /// </summary> 
   public string SourceUrl { get; set; }

   /// <summary>
   /// video soze
   /// </summary>
   public string VideoSize { get; set; }

   /// <summary>
   /// Arguments of FFmpeg
   /// </summary>
   public string[] FFmpegArgs { get; set; }
}