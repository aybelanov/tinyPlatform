using Google.Protobuf;
using Hub.Core.Domain.Clients;
using Hub.Services.Clients.Reports;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Records;

/// <summary>
/// Represents a dashboard client sensor data record service interface 
/// </summary>
public interface ISensorRecordService
{
   /// <summary>
   /// Delete a sensor record entities
   /// </summary>
   /// <param name="ids">Collection of the entity id</param>
   /// <returns>Async operation</returns>
   Task DeleteRecordByIdAsync(IEnumerable<long> ids);

   /// <summary>
   /// Delete a sensor record entity
   /// </summary>
   /// <param name="id">Entity id</param>
   /// <returns>Async operation</returns>
   Task DeleteRecordsByIdAsync(long id);

   /// <summary>
   /// Delete entities by the dynamoc filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Async operation</returns>
   Task DeleteRecordsByFilterAsync(DynamicFilter filter);


   /// <summary>
   /// Get sensor records by a dynamoc filter
   /// </summary>
   /// <param name="filter">Dynamoc filter</param>
   /// <returns>Collection of sensor records (async operation)</returns>
   Task<IFilterableList<SensorRecord>> GetRecordsAsync(DynamicFilter filter);

   /// <summary>
   /// Gets chart set of the sensor records
   /// </summary>
   /// <param name="request">Chart request</param>
   /// <returns>Chart series</returns>
   Task<IList<ChartSet>> GetChartSeriesAsync(ChartRequest request);

   /// <summary>
   /// Gets all data statistics (for admins) for the interval
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Data statistics</returns>
   Task<IList<DataStaticticsItem>> GetDataStatisticsAsync(DynamicFilter filter);

   /// <summary>
   /// Gets GNSS track (points) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Geo point collection</returns>
   /// <exception cref="NotImplementedException"></exception>
   Task<ByteString> GetTrackAsync(DynamicFilter filter);

   /// <summary>
   /// Deletes video segment info by identifiers
   /// </summary>
   /// <param name="ids">Segment identifiers</param>
   /// <returns></returns>
   Task DeleteVideoSegmentsAsync(IEnumerable<long> ids);

   /// <summary>
   /// Gets video segment data by the segment name
   /// </summary>
   /// <param name="segmentName">Videosegment file name</param>
   /// <returns>Videosegment</returns>
   Task<VideoSegment> GetVideoSegmentByFileName(string segmentName);

   /// <summary>
   /// Inserts a inbound video segment info to data base
   /// </summary>
   /// <param name="segment">Video segment</param>
   /// <param name="binary">Binarycontent</param>
   /// <returns>Videosegment identifier</returns>
   Task<long> InsertVideoSegmentAsync(VideoSegment segment, VideoSegmentBinary binary);

   /// <summary>
   /// Update videsegment info into databse
   /// </summary>
   /// <param name="segment">Videsegment</param>
   /// <returns></returns>
   Task UpdateVideoSegmentAsync(VideoSegment segment);

   /// <summary>
   /// Gets video segment data by the dynamic fily=ter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Video segment collection</returns>
   Task<IList<VideoSegment>> GetSegmentsByFilterAsync(DynamicFilter filter);

   /// <summary>
   /// Gets videosegment by guid
   /// </summary>
   /// <param name="guid">video segment guid</param>
   /// <returns>Video segment binary content</returns>
   Task<VideoSegmentBinary> GetSegmentByGuidAsync(Guid guid);

   /// <summary>
   /// User sensor record scope
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>Sensor record user scope query</returns>
   IQueryable<SensorRecord> UserScope(long? userId);
}
