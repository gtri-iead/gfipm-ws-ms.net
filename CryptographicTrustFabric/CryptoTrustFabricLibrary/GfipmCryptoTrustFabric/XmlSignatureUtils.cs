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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;

namespace GfipmCryptoTrustFabric
{
    public class XmlSignatureUtils
    {      
        public static bool VerifySignature(XmlDocument ctfDoc)
        {
            SignedXml signedXml = new CustomIdSignedXml(ctfDoc.DocumentElement);

            XmlNodeList nodeList = ctfDoc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");

            if (nodeList == null || nodeList.Count == 0)
            {
                throw new InvalidOperationException("The document does not have an XML Signature element.");
            }

            signedXml.LoadXml((XmlElement)nodeList[0]);
                     
            
            X509Certificate2 certificate = null;

            IEnumerator enumerator = signedXml.KeyInfo.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is KeyInfoX509Data)
                {
                    var current = (KeyInfoX509Data)enumerator.Current;
                    if (current.Certificates.Count != 0)
                    {
                        certificate = (X509Certificate2)current.Certificates[0];
                        break;
                    }
                }
            }

            if (certificate == null)
            {
                throw new InvalidOperationException("No Certificate found in XML Signature.");
            }

            // Check the signature and return the result.
            // This will check the certificate chain.
            return signedXml.CheckSignature(certificate, false);
        }
    }

    public class CustomIdSignedXml : SignedXml
    {
        public CustomIdSignedXml(XmlDocument document)
            : base(document)
        {
        }
    
        public CustomIdSignedXml(XmlElement elem)
            : base(elem)
        {
        }

        public CustomIdSignedXml()
            : base()
        {
        }

        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            XmlElement elem = null;
            if ((elem = base.GetIdElement(document, idValue)) == null)
            {
                XmlNodeList nl = document.GetElementsByTagName("*");
                IEnumerator enumerator = nl.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var node = (XmlNode)enumerator.Current;
                    var nodeEnum = node.Attributes.GetEnumerator();
                    while (nodeEnum.MoveNext())
                    {
                        var attr = (XmlAttribute)nodeEnum.Current;
                        if (attr.LocalName.ToLower() == "id" && attr.Value == idValue && node is XmlElement)
                        {
                            return (XmlElement)node;
                        }
                    }
                }
            }
            return elem;
        }
    }
}
