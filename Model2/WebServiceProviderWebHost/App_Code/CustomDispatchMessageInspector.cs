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
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Diagnostics;
using Common;


namespace CommercialVehicleCollisionWebservice
{
    class CustomDispatchMessageInspector : IDispatchMessageInspector
    {
        private string ReplaceLineBreaks(string source, string replacement)
        {
            return source.Replace("\r\n", replacement)
                         .Replace("\r", replacement)
                         .Replace("\n", replacement);
        }

        #region IDispatchMessageInspector Members

        public string GetMessageString(Message message)
        {
            MessageBuffer buffer = message.CreateBufferedCopy(Int32.MaxValue);

            string str = ReplaceLineBreaks(buffer.CreateMessage().ToString(), "");

            buffer.Close();

            return str;
        }
        public Message ReplaceMessageString(Message oldMessage, string to)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(ms);
            oldMessage.WriteMessage(xw);
            xw.Flush();
            string body = Encoding.UTF8.GetString(ms.ToArray());
            xw.Close();

            body = to;

            ms = new MemoryStream(Encoding.UTF8.GetBytes(body));
            XmlDictionaryReader xdr = XmlDictionaryReader.CreateTextReader(ms, new XmlDictionaryReaderQuotas());
            Message newMessage = Message.CreateMessage(xdr, int.MaxValue, oldMessage.Version);
            newMessage.Properties.CopyProperties(oldMessage.Properties);
            return newMessage;
        }


        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {            
            //Console.WriteLine( "Received the following reply: '{0}'", reply.ToString());
                                 
            //MemoryStream requestStream = new MemoryStream();
            //MessageBuffer requestBuffer = request.CreateBufferedCopy(Int32.MaxValue);
            //requestBuffer.WriteMessage(requestStream);
            //byte[] requestArr = requestStream.ToArray();

            //requestBuffer.Close();

            //string requestString = request.ToString(); //System.Text.Encoding.ASCII.GetString(requestArr);

            //WriteMessageToFile("AfterReceiveRequest-Original", requestString);

            //MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);


//            string modifiedRequestString = GetMessageString(request); // ReplaceLineBreaks(requestString, "");


//            WriteMessageToFile("AfterReceiveRequest-ModifiedStr", modifiedRequestString);

            //byte[] modifedRequestMessageInBytes = System.Text.Encoding.UTF8.GetBytes(modifiedRequestString);

            
            //XmlWriterSettings settings = new XmlWriterSettings();
            //settings.Encoding = System.Text.Encoding.UTF8;

            //XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();

            //XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(modifedRequestMessageInBytes, quotas);

            //Message newRequestMessage = Message.CreateMessage(reader, 2147483647, request.Version);

            //MessageBuffer modifiedRequestBuffer = newRequestMessage.CreateBufferedCopy(Int32.MaxValue);

//            request = ReplaceMessageString(request, modifiedRequestString); //modifiedRequestBuffer.CreateMessage();

            //MessageBuffer nbuffer = requestCopy.CreateBufferedCopy(Int32.MaxValue);

            //WriteMessageToFile("AfterReceiveRequest-Original", nbuffer.ToString());

            //buffer.Close();

            LogMessage("AfterReceiveRequest-ModifiedMsg", request);

            //modifiedRequestBuffer.Close();

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            //Console.WriteLine( "Sending the following request: '{0}'", request.ToString());

            LogMessage("BeforeSendReply", reply);
        }

        void LogMessage(string methodName, string msg)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.CustomDispatchMessageInspector." + methodName,
                            "MyTraceSource", SourceLevels.Information);
            if (msg != null)
            {
                ts.TraceInformation("Message:");
                //request.CreateBufferedCopy(int.MaxValue)
                ts.TraceInformation(msg);
                //file.WriteLine(msg.CreateBufferedCopy(int.MaxValue).ToString());
            }
            else
            {
                ts.TraceInformation("Message == NULL");
            }
        }

        void LogMessage(string methodName, Message msg)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.CustomDispatchMessageInspector." + methodName,
                            "MyTraceSource", SourceLevels.Information);
            
            if (msg != null)
            {
                ts.TraceInformation("Message:");
                //request.CreateBufferedCopy(int.MaxValue)
                ts.TraceInformation(msg.ToString());
                //file.WriteLine(msg.CreateBufferedCopy(int.MaxValue).ToString());
            }
            else
            {
                ts.TraceInformation("Message == NULL");
            }
       }


        #endregion
    }
}
