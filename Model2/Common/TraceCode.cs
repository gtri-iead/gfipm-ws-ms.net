using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public enum TraceCode
    {
        AppDomainUnload = 2,
        Diagnostics = 1,
        EventLog = 4,
        HandledException = 8,
        UnhandledException = 0x10
    }
}
