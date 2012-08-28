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
using System.Xml;
using System.Xml.Serialization;

namespace GfipmCryptoTrustFabric
{
    public class CtfSerializer
    {
        public static T Deserialize<T>(string objectXml, string rootElementName, string rootElementNamespace)
        {
            T retVal;

            try
            {
                // Need to read it to bytes, to undo the fact that strings are UTF-16 all the time.
                // We want it to handle it as UTF8.
                byte[] bytes = Encoding.UTF8.GetBytes(objectXml);

                using (MemoryStream memStream = new MemoryStream(bytes))
                {
                    XmlRootAttribute xRoot = new XmlRootAttribute();
                    xRoot.ElementName = rootElementName;
                    xRoot.Namespace = rootElementNamespace;
                    xRoot.IsNullable = true;

                    // TODO: Cache the serializer
                    XmlSerializer serializer = new XmlSerializer(typeof(T), xRoot);

                    serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);
                    serializer.UnknownElement += new XmlElementEventHandler(Serializer_OnUnknownElement);
                    serializer.UnknownNode += new XmlNodeEventHandler(Serializer_OnUnknownNode);
                    serializer.UnreferencedObject += new UnreferencedObjectEventHandler(Serializer_UnreferencedObject);

                    retVal = (T)serializer.Deserialize(memStream);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DeserializeFromXmlString = " + typeof(T).ToString());

                throw e;
            }
            return retVal;
        }

        private static void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            //StreamWriter file = new StreamWriter("c:\\temp\\Serializer_UnknownAttribute.txt", false);
            //file.WriteLine("_________________________________________");
            //file.WriteLine("DateTime: " + DateTime.Now.ToString());


            //file.WriteLine("Unknown Attribute");
            //file.WriteLine("\t" + e.Attr.Name + " " + e.Attr.InnerXml);
            //file.WriteLine("\t LineNumber: " + e.LineNumber);
            //file.WriteLine("\t LinePosition: " + e.LinePosition);

            //file.Close();
        }

        private static void Serializer_OnUnknownElement(object sender, XmlElementEventArgs e)
        {
            //StreamWriter file = new StreamWriter("c:\\temp\\Serializer_OnUnknownElement.txt", false);
            //file.WriteLine("_________________________________________");
            //file.WriteLine("DateTime: " + DateTime.Now.ToString());

            //file.WriteLine("Unknown Element");
            //file.WriteLine("\t" + e.Element.Name + " " + e.Element.InnerXml);
            //file.WriteLine("\t LineNumber: " + e.LineNumber);
            //file.WriteLine("\t LinePosition: " + e.LinePosition);

            //file.Close();
        }

        private static void Serializer_OnUnknownNode(object sender, XmlNodeEventArgs e)
        {
            //StreamWriter file = new StreamWriter("c:\\temp\\Serializer_OnUnknownNode.txt", false);
            //file.WriteLine("_________________________________________");
            //file.WriteLine("DateTime: " + DateTime.Now.ToString());

            //file.WriteLine("Unknown Node");
            //file.WriteLine("\t" + e.Name + " " + e.Text);
            //file.WriteLine("\t LineNumber: " + e.LineNumber);
            //file.WriteLine("\t LinePosition: " + e.LinePosition);

            //file.Close();
        }

        private static void Serializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            //StreamWriter file = new StreamWriter("c:\\temp\\Serializer_UnreferencedObject.txt", false);
            //file.WriteLine("_________________________________________");
            //file.WriteLine("DateTime: " + DateTime.Now.ToString());

            //file.WriteLine("Unknown Element");
            //file.WriteLine("\t" + e.UnreferencedId + " " + e.UnreferencedObject.ToString());

            //file.Close();
        }

    }
}
