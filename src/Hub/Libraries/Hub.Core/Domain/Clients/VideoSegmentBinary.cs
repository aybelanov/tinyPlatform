using Humanizer.Bytes;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represents video segment binary content
/// </summary>
public class VideoSegmentBinary : BaseEntity
{
   /// <summary>
   /// Video segment identifier
   /// </summary>
   public long VideoSegmentId { get; set; }

   /// <summary>
   /// Binary content
   /// </summary>
   public byte[] Binary { get; set; }
}
