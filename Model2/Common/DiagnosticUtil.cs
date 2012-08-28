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
using System.Xml;

namespace Common
{
    public static class DiagnosticUtil
    {
        public static Exception TryWrapReadException(XmlReader reader, Exception inner)
        {
            if (((inner is FormatException) || (inner is ArgumentException)) || ((inner is InvalidOperationException) || (inner is OverflowException)))
            {
                return DiagnosticUtil.ThrowHelperXml(reader, "An error occurred reading XML data.", inner);
            }

            return null;
        }

        public static Exception ThrowHelperXml(XmlReader reader, string message)
        {
           return ThrowHelperXml(reader, message, null);
        }

        public static Exception ThrowHelperXml(XmlReader reader, string message, Exception inner)
        {
            IXmlLineInfo info = reader as IXmlLineInfo;
            return new XmlException(message, null, (info != null) ? info.LineNumber : 0, (info != null) ? info.LinePosition : 0);
        }
    }
}
