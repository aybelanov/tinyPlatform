using Clients.Dash.Domain;
using Clients.Dash.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clients.Dash.Pages.Reports.Players;

#pragma warning disable CS1591

public partial class VideoRequest
{
   #region Ctors

   /// <summary>
   /// Default Ctor
   /// </summary>
   public VideoRequest()
   {

   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepares download report data file request model
   /// </summary>
   /// <returns>Download request model</returns>
   protected async Task<VideoRequestModel> PrepareFileRequestModelAsync()
   {
      var now = DateTime.UtcNow;
      var model = new VideoRequestModel();
      model.To = ClientHelper.ConvertUtcToBrowserTime(now);
      model.From = model.To.AddDays(-1);

      return await Task.FromResult(model);
   }

   #endregion

   #region nested calsses

   /// <summary>
   /// Dowloadt report data file request model 
   /// </summary>
   public class VideoRequestModel //: global::Shared.Clients.DownloadRequest
   {
      /// <summary>
      /// Date time "from"
      /// </summary>
      public DateTime From { get; set; }

      /// <summary>
      /// Date time "to"
      /// </summary>
      public DateTime To { get; set; }

      /// <summary>
      /// Selected device
      /// </summary>
      public DeviceSelectItem SelectedDevice { get; set; }

      /// <summary>
      /// Sensor ids
      /// </summary>
      public SensorSelectItem SelectedSensor { get; set; }
   }

   #endregion
}
