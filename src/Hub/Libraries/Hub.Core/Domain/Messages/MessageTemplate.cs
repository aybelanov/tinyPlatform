using Hub.Core.Domain.Localization;
using Shared.Common;

namespace Hub.Core.Domain.Messages;

/// <summary>
/// Represents a message template
/// </summary>
public partial class MessageTemplate : BaseEntity, ILocalizedEntity
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the BCC Email addresses
    /// </summary>
    public string BccEmailAddresses { get; set; }

    /// <summary>
    /// Gets or sets the subject
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets the body
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the template is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the delay before sending message
    /// </summary>
    public int? DelayBeforeSend { get; set; }

    /// <summary>
    /// Gets or sets the period of message delay 
    /// </summary>
    public int DelayPeriodId { get; set; }

    /// <summary>
    /// Gets or sets the download identifier of attached file
    /// </summary>
    public long AttachedDownloadId { get; set; }

    /// <summary>
    /// Gets or sets the used email account identifier
    /// </summary>
    public long EmailAccountId { get; set; }

    /// <summary>
    /// Gets or sets the period of message delay
    /// </summary>
    public MessageDelayPeriod DelayPeriod
    {
        get => (MessageDelayPeriod)DelayPeriodId;
        set => DelayPeriodId = (int)value;
    }

//   #region Navigation
//#pragma warning disable CS1591

//   public EmailAccount EmailAccount { get; set; } 

//#pragma warning restore CS1591
//   #endregion
}
