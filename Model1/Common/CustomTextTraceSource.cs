using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common
{
    public class CustomTextTraceSource : TraceSource
    {
        private string _targetClassAndMethodName = "Class and Method names not set.";

        public CustomTextTraceSource(string targetClassAndMethodName, string sourceName)
            : base(sourceName)
        {
            _targetClassAndMethodName = targetClassAndMethodName;
            TraceDateTime();
            TraceClassAndMethod();
        }
        public CustomTextTraceSource(string targetClassAndMethodName, string sourceName, SourceLevels defaultLevel) :
            base(sourceName, defaultLevel)
        {
            _targetClassAndMethodName = targetClassAndMethodName;

            TraceDateTime();
            TraceClassAndMethod();
        }

        void TraceDateTime()
        {
            this.TraceInformation("_________________________________________");
                        
            this.TraceInformation("DateTime: " + DateTime.Now.ToString());
        }

        private void TraceClassAndMethod()
        {
            this.TraceInformation("From " + _targetClassAndMethodName);
        }
    }
}
