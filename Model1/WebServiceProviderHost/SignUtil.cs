using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;

namespace WebServiceProvider
{
    public class SignUtil
    {
        string STR_SOAP_NS = "http://schemas.xmlsoap.org/soap/envelope/";
        string STR_SOAPSEC_NS = "http://schemas.xmlsoap.org/soap/security/2000-12";

        /// <summary>
        /// Signs the SOAP document and adds a digital signature to it.
        /// 
        /// Note a lot of optional settings are applied against
        /// key and certificate info to match the required XML document
        /// structure the server requests.
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="certFriendlyName">Friendly Name of Cert installed in the Certificate Store under CurrentUser | Personal</param>
        /// <returns></returns>
        public XmlDocument SignSoapBody(XmlDocument xmlDoc, X509Certificate2 cert)
        {    
            // *** Add search Namespaces references to ensure we can reliably work     
            // *** against any SOAP docs regardless of tag naming    
            XmlNamespaceManager ns = new XmlNamespaceManager(xmlDoc.NameTable);    
            ns.AddNamespace("SOAP", STR_SOAP_NS);    
            ns.AddNamespace("SOAP-SEC", STR_SOAPSEC_NS);     
            
            // *** Grab the body element - this is what we create the signature from    
            XmlElement body = xmlDoc.DocumentElement.SelectSingleNode(@"//SOAP:Body", ns) as XmlElement;    
            
            if (body == null)        
                throw new ApplicationException("No body tag found");     
            
            // *** We'll only encode the <SOAP:Body> - add id: Reference as #Body    
            body.SetAttribute("id", "Body");     
            
            // *** Signed XML will create Xml Signature - Xml fragment    
            SignedXml signedXml = new SignedXml(xmlDoc);     
            
            // *** Create a KeyInfo structure    
            KeyInfo keyInfo = new KeyInfo();     
            
            // *** The actual key for signing - MAKE SURE THIS ISN'T NULL!    
            signedXml.SigningKey = cert.PrivateKey;     
            
            // *** Specifically use the issuer and serial number for the data rather than the default    
            KeyInfoX509Data keyInfoData = new KeyInfoX509Data();    
            keyInfoData.AddIssuerSerial(cert.Issuer, cert.GetSerialNumberString());    
            keyInfo.AddClause(keyInfoData);      
            
            // *** provide the certficate info that gets embedded - note this is only    
            // *** for specific formatting of the message to provide the cert info    
            signedXml.KeyInfo = keyInfo;      
            
            // *** Again unusual - meant to make the document match template    
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;     
            
            // *** Now create reference to sign: Point at the Body element    
            Reference reference = new Reference();    
            reference.Uri = "#Body";  
            
            // reference id=body section in same doc    
            reference.AddTransform(new XmlDsigExcC14NTransform());  
            
            // required to match doc    
            signedXml.AddReference(reference);     
            
            // *** Finally create the signature    
            signedXml.ComputeSignature();     
            
            // *** Result is an XML node with the signature detail below it    
            // *** Now let's add the sucker into the SOAP-HEADER    
            XmlElement signedElement = signedXml.GetXml();     
            
            // *** Create SOAP-SEC:Signature element    
            XmlElement soapSignature = xmlDoc.CreateElement("Signature", STR_SOAPSEC_NS);    
            soapSignature.Prefix = "SOAP-SEC";    soapSignature.SetAttribute("MustUnderstand", "", "1");     
            
            // *** And add our signature as content    
            soapSignature.AppendChild(signedElement);     
            
            // *** Now add the signature header into the master header    
            XmlElement soapHeader = xmlDoc.DocumentElement.SelectSingleNode("//SOAP:Header", ns) as XmlElement;    
            if (soapHeader == null)    
            {        
                soapHeader = xmlDoc.CreateElement("Header", STR_SOAP_NS);        
                soapHeader.Prefix = "SOAP";        
                xmlDoc.DocumentElement.InsertBefore(soapHeader, xmlDoc.DocumentElement.ChildNodes[0]);    
            }    
            
            soapHeader.AppendChild(soapSignature);     
            
            return xmlDoc;
        }

        /// <summary>
        /// Validates the Xml Signature in a document.
        /// 
        /// This routine is significantly simpler because the key parameters
        /// are embedded into the signature itself. All that's needed is a
        /// certificate to provide the key - the rest can be read from the
        /// Signature itself.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="publicCertFileName"></param>
        /// <returns></returns>
        public bool ValidateSoapBodySignature(XmlDocument doc, X509Certificate2 cert)
        {    
            // *** Load the doc this time    
            SignedXml sdoc = new SignedXml(doc);    
            
            // *** Find the signature and load it into SignedXml    
            XmlNodeList nodeList = doc.GetElementsByTagName("Signature");    
            sdoc.LoadXml((XmlElement)nodeList[0]);     
            
            // *** Now read the actual signature and validate    
            bool result = sdoc.CheckSignature(cert, true);     
            
            return result;
        }

        /// <summary>
        /// Retrieve a Certificate from the Windows Certificate store
        /// by its Friendly name.
        /// </summary>
        /// <param name="subject">The friendly name of the certificate</param>
        /// <param name="storeName">The store name type ( for example: Storename.My )</param>
        /// <param name="storeLocation">Top level Location (CurrentUser,LocalMachine)</param>
        /// <returns></returns>
        public X509Certificate2 GetCertificateBySubject(string subject, StoreName storeName, StoreLocation storeLocation)
        {    
            X509Store xstore = new X509Store(storeName, storeLocation);    
            xstore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);     
            
            X509Certificate2 cert = null;    
            
            foreach (X509Certificate2 cert2 in xstore.Certificates)   
            {
                //string sub = wwUtils.ExtractString(cert2.Subject, "CN=", ",", true, true);         
                string sub = cert2.GetNameInfo(X509NameType.SimpleName, false);
                
                if (subject == sub)        
                {            
                    cert = cert2;            
                    break;        
                }    
            }    
            return cert;
        } 
        
        /// <summary>
        /// Retrieve a Certificate from the Windows Certificate store
        /// by its Friendly name.
        /// 
        /// This code pulls from CurrentUser - Personal cert store
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public X509Certificate2 GetCertificateBySubject(string subject)
        {    
            return this.GetCertificateBySubject(subject, StoreName.My, StoreLocation.CurrentUser);
        } 
        
        /// <summary>
        /// Creates a Certificate from a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public X509Certificate2 GetCertificateFromFile(string fileName)
        {    
            X509Certificate cert = X509Certificate.CreateFromCertFile(fileName);    
            
            return new X509Certificate2(cert);
        }

        /// Gets a properly formatted instance of an Xml document
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetXmlDocument()
        {    
            XmlDocument doc = new XmlDocument();    
            
            doc.PreserveWhitespace = true;    
            
            return doc;
        } 
        
        /// <summary>
        /// Gets a properly formatted Xml Document from an input stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public XmlDocument GetXmlDocument(Stream stream)
        {    
            XmlDocument doc = this.GetXmlDocument();    
            
            doc.Load(stream);    
            return doc;
        } 
        
        /// <summary>
        /// Gets a properly formatted Xml document from a file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public XmlDocument GetXmlDocument(string fileName)
        {    
            XmlDocument doc = this.GetXmlDocument();                
            
            doc.Load(fileName);    
            
            return doc;
        }
    }
}
