using Shared.Common;

namespace Hub.Core.Domain.Media;

/// <summary>
/// Represents a picture binary data
/// </summary>
public partial class PictureBinary : BaseEntity
{
   /// <summary>
   /// Gets or sets the picture binary
   /// </summary>
   public byte[] BinaryData { get; set; }

   /// <summary>
   /// Gets or sets the picture identifier
   /// </summary>
   public long PictureId { get; set; }


   //   #region Navigation
   //#pragma warning disable CS1591

   //   public Picture Picture { get; set; }

   //#pragma warning restore CS1591
   //   #endregion
}
