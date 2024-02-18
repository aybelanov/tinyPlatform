using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Clients;

/// <summary>
/// Enum of compression types for exporting files
/// </summary>
public enum FileCompressionType
{
   /// <summary>
   /// none
   /// </summary>
   None,
   /// <summary>
   /// gzip archive
   /// </summary>
   GZIP,
   /// <summary>
   /// zip archive
   /// </summary>
   ZIP,
   ///// <summary>
   ///// 7z archive
   ///// </summary>
   //SevenZip
}
