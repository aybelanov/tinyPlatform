using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represents a videsegment
/// </summary>
public class VideoSegment : BaseEntity
{
   /// <summary>
   /// Default ctor
   /// </summary>
   public VideoSegment()
   {
      Guid = Guid.NewGuid();  
   }

   /// <summary>
   /// Video segment duration [#EXTINF]
   /// </summary>
   /// <remarks>
   /// <see href="https://datatracker.ietf.org/doc/html/rfc8216#section-4.3.2.1"/>
   /// </remarks>
   public double Extinf {  get; set; } 

   /// <summary>
   /// Inbound (from device) segment file name
   /// </summary>
   public string InboundName {get;set;}

   /// <summary>
   /// Guid uses as an outbound segment name (for cleint stream's playlist)
   /// </summary>
   public Guid Guid { get; set; }

   /// <summary>
   /// IP cam identifier
   /// </summary>
   public long IpcamId { get; set; } 
   
   /// <summary>
   /// Segment creation datetime UTC
   /// </summary>
   public DateTime OnCreatedUtc { get; set; }

   /// <summary>
   /// Segment receiver datetime UTC
   /// </summary>
   public DateTime OnReceivedUtc { get; set; }  

   /// <summary>
   /// Segment resolution
   /// </summary>
   public string Resolution { get; set; }
}