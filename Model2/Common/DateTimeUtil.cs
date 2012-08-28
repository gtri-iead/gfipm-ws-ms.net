//
// Copyright (c) 2012, Georgia Institute of Technology. All Rights Reserved.
// This code was developed by Georgia Tech Research Institute (GTRI) under
// a grant from the U.S. Dept. of Justice, Bureau of Justice Assistance.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class DateTimeUtil
    {
        // Methods
        public static DateTime Add(DateTime time, TimeSpan timespan)
        {
            if ((timespan >= TimeSpan.Zero) && ((DateTime.MaxValue - time) <= timespan))
            {
                return GetMaxValue(time.Kind);
            }
            if ((timespan <= TimeSpan.Zero) && ((DateTime.MinValue - time) >= timespan))
            {
                return GetMinValue(time.Kind);
            }
            return (time + timespan);
        }

        public static DateTime AddNonNegative(DateTime time, TimeSpan timespan)
        {
            if (timespan <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("Lifetime must be greater than or equal to TimeSpan.Zero.");
            }
            return Add(time, timespan);
        }

        public static DateTime GetMaxValue(DateTimeKind kind)
        {
            return new DateTime(DateTime.MaxValue.Ticks, kind);
        }

        public static DateTime GetMinValue(DateTimeKind kind)
        {
            return new DateTime(DateTime.MinValue.Ticks, kind);
        }

        public static DateTime? ToUniversalTime(DateTime? value)
        {
            if (value.HasValue && (value.Value.Kind != DateTimeKind.Utc))
            {
                return new DateTime?(ToUniversalTime(value.Value));
            }
            return value;
        }

        public static DateTime ToUniversalTime(DateTime value)
        {
            if (value.Kind == DateTimeKind.Utc)
            {
                return value;
            }
            return value.ToUniversalTime();
        }
    }
}
