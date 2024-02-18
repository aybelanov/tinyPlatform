using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Reports;

/// <summary>
/// Represent a data statistics item
/// </summary>
public class DataStaticticsItem
{
    /// <summary>
    /// A moment in time
    /// </summary>
    public long Moment { get; set; }

    /// <summary>
    /// Count of the sensor records
    /// </summary>
    public long RecordCount { get; set; }
}
