﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Clients;

#pragma warning disable CS1591

public enum TimeIntervalType
{
   [EnumMember(Value = "second")]
   Second = 1,

   [EnumMember(Value = "minute")]
   Minute,

   [EnumMember(Value = "hour")]
   Hour,

   [EnumMember(Value = "day")]
   Day,

   [EnumMember(Value = "week")]
   Week,

   [EnumMember(Value = "decade")]
   Decade,

   [EnumMember(Value = "month")]
   Month,

   [EnumMember(Value = "quarter")]
   Quarter,

   [EnumMember(Value = "year")]
   Year
}

#pragma warning restore CS1591