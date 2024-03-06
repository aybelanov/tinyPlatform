using Clients.Dash.Domain;
using Clients.Dash.Infrastructure;
using Clients.Dash.Services.Localization;
using Microsoft.Extensions.DependencyInjection;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clients.Dash.Pages.Reports.Export;

#pragma warning disable CS1591

public partial class DownloadTaskRequest
{
   #region fields

   private readonly static IDictionary<int, string> _compressions;
   private readonly static IDictionary<int, string> _formats;

   #endregion

   #region Ctors

   /// <summary>
   /// Static Ctor
   /// </summary>
   static DownloadTaskRequest()
   {
      using var scope = Singleton<IServiceProvider>.Instance.GetRequiredService<IServiceScopeFactory>().CreateScope();
      var localizer = scope.ServiceProvider.GetRequiredService<Localizer>();
      _compressions = Enum.GetValues<FileCompressionType>().ToDictionary(k => (int)k, v => localizer[$"FileCompressionType.{v}"].ToString());
      _formats = Enum.GetValues<ExportFileType>().ToDictionary(k => (int)k, v => localizer[$"ExportFileType.{v}"].ToString());
   }

   /// <summary>
   /// Default Ctor
   /// </summary>
   public DownloadTaskRequest()
   {

   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepares download report data file request model
   /// </summary>
   /// <returns>Download request model</returns>
   protected async Task<DownloadRequestModel> PrepareFileRequestModelAsync()
   {
      var now = DateTime.UtcNow;
      var model = new DownloadRequestModel();
      model.To = ClientHelper.ConvertUtcToBrowserTime(now);
      model.From = model.To.AddDays(-1);
      model.AvailableCompressions = _compressions;
      model.AvailableFormats = _formats;

      return await Task.FromResult(model);
   }

   #endregion

   /// <summary>
   /// Dowloadt report data file request model 
   /// </summary>
   public class DownloadRequestModel //: global::Shared.Clients.DownloadRequest
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
      /// File format
      /// </summary>
      public ExportFileType Format { get; set; }

      /// <summary>
      /// File compression
      /// </summary>
      public FileCompressionType Compression { get; set; }

      /// <summary>
      /// Current page size (need after added to show firsy page)
      /// </summary>
      public int Top { get; set; }

      /// <summary>
      /// Selected device identfier
      /// </summary>
      public long DeviceId => SelectedDevice?.Id ?? 0;

      /// <summary>
      /// Selected device
      /// </summary>
      public DeviceSelectItem SelectedDevice { get; set; }

      /// <summary>
      /// Sensor ids
      /// </summary>
      public IEnumerable<long> SensorIds => SelectedSensors.Select(x => x.Id);

      /// <summary>
      /// Selected sensors
      /// </summary>
      public IEnumerable<SensorSelectItem> SelectedSensors { get; set; }

      /// <summary>
      /// File format id
      /// </summary>
      public int FormatId
      {
         get => (int)Format;
         set => Format = (ExportFileType)value;
      }

      /// <summary>
      /// File compression id
      /// </summary>
      public int CompressionId
      {
         get => (int)Compression;
         set => Compression = (FileCompressionType)value;
      }

      /// <summary>
      /// Available file formats
      /// </summary>
      public IDictionary<int, string> AvailableFormats { get; set; }

      /// <summary>
      /// Available file compression types
      /// </summary>
      public IDictionary<int, string> AvailableCompressions { get; set; }
   }
}

#pragma warning restore CS1591
