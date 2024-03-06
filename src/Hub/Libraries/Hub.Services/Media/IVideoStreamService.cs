using System.Threading.Tasks;

namespace Hub.Services.Media;

/// <summary>
/// Represents a video service interface
/// </summary>
public interface IVideoStreamService
{
   ///// <summary>
   ///// Saves video segment
   ///// </summary>
   ///// <param name="metadata">Segment file metadata</param>
   ///// <param name="segment">Video segment</param>
   ///// <returns></returns>
   //Task SaveVideoSegment(SegmentMetadata metadata, VideoSegmentProto segment);

   /// <summary>
   /// Gets file metadat
   /// </summary>
   /// <param name="fileName">Segment file name</param>
   /// <returns>HLS file metadata</returns>
   Task<SegmentMetadata> GetFileMetadataAsync(string fileName);

   /// <summary>
   /// Checks current user to acces ti this video segment
   /// </summary>
   /// <param name="fileName">Video segment file name</param>
   /// <returns>Access result</returns>
   Task<bool> CheckUserAccesAsync(string fileName);

   /// <summary>
   /// Creates ipcam playlist by the datetime interval
   /// </summary>
   /// <param name="id">Ipcam identifier</param>
   /// <param name="from">Period "from"</param>
   /// <param name="to">Period "to"</param>
   /// <returns>Playlist as string</returns>
   Task<string> CreatePlaylistAsync(long id, long from, long to);
}
