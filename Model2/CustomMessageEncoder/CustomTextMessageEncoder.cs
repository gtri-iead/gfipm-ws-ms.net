//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace CustomTextMessageEncoder
{
    public class CustomTextMessageEncoder : MessageEncoder
    {
        private CustomTextMessageEncoderFactory factory;
        private XmlWriterSettings writerSettings;
        private string contentType;
        
        public CustomTextMessageEncoder(CustomTextMessageEncoderFactory factory)
        {
            this.factory = factory;
            
            this.writerSettings = new XmlWriterSettings();
            this.writerSettings.Encoding = Encoding.GetEncoding(factory.CharSet);
            this.contentType = string.Format("{0}; charset={1}", 
                this.factory.MediaType, this.writerSettings.Encoding.HeaderName);
        }

        public override string ContentType
        {
            get
            {
                return this.contentType;
            }
        }

        public override string MediaType
        {
            get 
            {
                return factory.MediaType;
            }
        }

        public override MessageVersion MessageVersion
        {
            get 
            {
                return this.factory.MessageVersion;
            }
        }

        private XmlElement GetMainSignatureElement(XmlNodeList nodeList, string parent)
        {
            XmlElement elem = null;

            foreach (XmlNode n in nodeList)
            {
                if (n.ParentNode.Name.Contains(parent))
                {
                    elem = n as XmlElement;
                    break;
                }
            }

            return elem;
        }

        private XmlElement GetSignatureElement(XmlDocument doc, string signatureNamespaceUrl, string parent)
        {
            XmlNodeList nodeList = null;

            if (string.IsNullOrEmpty(signatureNamespaceUrl))
            {
                nodeList = doc.GetElementsByTagName("Signature");
            }
            else
            {
                nodeList = doc.GetElementsByTagName("Signature", signatureNamespaceUrl);
            }

            XmlElement sigElem = null;

            if (nodeList.Count == 1)
            {
                sigElem = (XmlElement)nodeList[0];
            }
            else
            {
                sigElem = GetMainSignatureElement(nodeList, parent);
            }

            return sigElem;
        }


        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {   
            byte[] msgContents = new byte[buffer.Count];
            Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
            bufferManager.ReturnBuffer(buffer.Array);

            MemoryStream stream = new MemoryStream(msgContents);

            Common.CustomTextTraceSource ts = new Common.CustomTextTraceSource("CustomTextMessageEncoder.CustomTextMessageEncoder.ReadMessage",
                            "MyTraceSource", SourceLevels.Information);

            string xmlstring = string.Empty;

            try
            {
                MemoryStream tmpStream = new MemoryStream(msgContents);

                var sr = new StreamReader(tmpStream);
                xmlstring = sr.ReadToEnd();

                ts.TraceInformation(xmlstring);

                //XmlDocument signedDoc = new XmlDocument() { PreserveWhitespace = false };

                //signedDoc.LoadXml(xmlstring);

                //XmlSigner verifier = new XmlSigner(signedDoc);

                //XmlElement sigElement = GetSignatureElement(signedDoc, @"http://www.w3.org/2000/09/xmldsig#", "Security");

                //bool valid = false;
                //if (sigElement != null)
                //{
                //    //valid = verifier.ValidateSignature(sigElement, algorithm);
                //    valid = verifier.ValidateSignature(sigElement);

                //    file.WriteLine("Signature Validation = " + valid.ToString());
                //}
            }
            catch (Exception e)
            {
                ts.TraceInformation("Exception:  " + e.Message);
                if (e.InnerException != null)
                {
                    ts.TraceInformation("InnerException:  " + e.InnerException.Message);
                }

            }

            //file.WriteLine("Request Message: ");
            //file.WriteLine(xmlstring);

            //file.Close();

            return ReadMessage(stream, int.MaxValue);
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            XmlReader reader = XmlReader.Create(stream);
            return Message.CreateMessage(reader, maxSizeOfHeaders, this.MessageVersion);
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            MemoryStream stream = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(stream, this.writerSettings);
            message.WriteMessage(writer);
            writer.Close();

            byte[] messageBytes = stream.GetBuffer();
            int messageLength = (int)stream.Position;

            //Common.CustomTextTraceSource ts = new Common.CustomTextTraceSource("CustomTextMessageEncoder.CustomTextMessageEncoder.WriteMessage",
            //"MyTraceSource", SourceLevels.Information);

            //stream.Position = 0;

            //StreamReader sr = new StreamReader(stream);

            //ts.TraceInformation(sr.ReadToEnd());


            stream.Close();

            int totalLength = messageLength + messageOffset;
            byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

            ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
            return byteArray;
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            XmlWriter writer = XmlWriter.Create(stream, this.writerSettings);
            message.WriteMessage(writer);
            writer.Close();
        }
    }
}
