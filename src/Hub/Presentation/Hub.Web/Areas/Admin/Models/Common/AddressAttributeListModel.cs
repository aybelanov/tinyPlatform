﻿using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an address attribute list model
/// </summary>
public record AddressAttributeListModel : BasePagedListModel<AddressAttributeModel>
{
}