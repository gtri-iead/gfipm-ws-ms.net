//
// Copyright (c) 2012, Georgia Institute of Technology. All Rights Reserved.
// This code was developed by Georgia Tech Research Institute (GTRI) under
// a grant from the U.S. Dept. of Justice, Bureau of Justice Assistance.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Diagnostics;

using Common;

namespace WebServiceProviderHost
{
    class CommercialVehicleMessageInspector : IDispatchMessageInspector
    {
        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {            
            WriteMessageToLog("AfterReceiveRequest", request);

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            WriteMessageToLog("BeforeSendReply", reply);
        }

        void WriteMessageToLog(string methodName, Message msg)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("WebServiceProvider.WspServiceAuthorizationManager." + methodName,
                "MyTraceSource", SourceLevels.Information);

            if (msg != null)
            {
                ts.TraceInformation("Message:");
                ts.TraceInformation(msg.ToString());
            }
            else
            {
                ts.TraceInformation("Message == NULL");
            }
        }


        #endregion
    }
}
