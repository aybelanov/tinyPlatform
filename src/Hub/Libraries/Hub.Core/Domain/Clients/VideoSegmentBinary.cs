using Shared.Common;

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
