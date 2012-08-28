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
using System.IO;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace Common
{
    public class CustomMessageInspector : IClientMessageInspector
    {
        #region IClientMessageInspector Members

        public void AfterReceiveReply( ref Message reply, object correlationState)
        {            
            Console.WriteLine( "Received the following reply: '{0}'", reply.ToString());

            LogMessage("AfterReceiveReply", reply);
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            Console.WriteLine( "Sending the following request: '{0}'", request.ToString());

            LogMessage("BeforeSendRequest", request);
            
            return null;
        }

        void LogMessage(string methodName, Message msg)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("Common.CustomMessageInspector." + methodName,
                "MyTraceSource", SourceLevels.Information);

            if (msg != null)
            {
                ts.TraceInformation("Message:");
                ts.TraceInformation(msg.ToString());
            }
            else
            {
                ts.TraceInformation("Message is NULL");
            }
        }


        #endregion
    }
}
