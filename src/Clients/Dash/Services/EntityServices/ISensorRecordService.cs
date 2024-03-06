using Clients.Dash.Domain;
using Clients.Widgets;
using Clients.Widgets.Core;
using Shared.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Clients.Widgets.VideoPlayer;

namespace Clients.Dash.Services.EntityServices;

/// <summary>
/// Sensor data service interface
/// </summary>
public interface ISensorRecordService
{
   /// <summary>
   /// Gets sensor data by filter
   /// </summary>
   /// <param name="filter">Sensor filter</param>
   /// <returns>Sensor record collection</returns>
   Task<IFilterableList<SensorRecord>> GetRecordsAsync(DynamicFilter filter);

   /// <summary>
   /// Gets a chrt data series
   /// </summary>
   /// <param name="request">Chart request</param>
   /// <returns>Chart point data set</returns>
   Task<IList<ChartSet>> GetChartSeriesAsync(ChartRequest request);

   /// <summary>
   /// Deletes sensor data by filter
   /// </summary>
   /// <param name="filter">Sensor filter</param>
   /// <returns></returns>
   Task DeleteRecordsAsync(DynamicFilter filter);

   /// <summary>
   /// Gets data staistics for all users (admins only) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Chart points</returns>
   Task<IList<TimelineChart.Point>> GetAllDataStatistics(DynamicFilter filter);

   /// <summary>
   /// Gets data staistics for current user by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Chart points</returns>
   Task<IList<TimelineChart.Point>> GetUserDataStatistics(DynamicFilter filter);

   /// <summary>
   /// Gets GNSS track
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Geo points</returns>
   Task<IList<OpenLayerBase.GeoPoint>> GetTrackAsync(DynamicFilter filter);

   /// <summary>
   /// Gets the last rcord of the sensor
   /// </summary>
   /// <param name="sensorId">Sensor identifier</param>
   /// <returns>Geo point</returns>
   Task<OpenLayerBase.GeoPoint> GetLastRecordAsync(long sensorId);

   /// <summary>
   /// Gets video segments by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Video segment collection</returns>
   Task<IList<Segment>> GetVideoSegmentsAsync(DynamicFilter filter);
}
