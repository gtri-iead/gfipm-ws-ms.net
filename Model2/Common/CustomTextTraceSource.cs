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
