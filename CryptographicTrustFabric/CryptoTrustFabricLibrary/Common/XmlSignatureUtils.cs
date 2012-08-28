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

namespace Common
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
