using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;


namespace Common
{
    public class KeyInfo
    {
        // Fields
        private SecurityTokenSerializer _keyInfoSerializer;
        private string _retrieval;
        private SecurityKeyIdentifier _ski;

        // Methods
        public KeyInfo(SecurityTokenSerializer keyInfoSerializer)
        {
            this._keyInfoSerializer = keyInfoSerializer;
            this._ski = new SecurityKeyIdentifier();
        }

        public virtual void ReadXml(XmlDictionaryReader reader)
        {
            if (reader == null)
            {
                throw DiagnosticUtil.ExceptionUtil.ThrowHelperArgumentNull("reader");
            }
            reader.MoveToContent();
            if (reader.IsStartElement("KeyInfo", "http://www.w3.org/2000/09/xmldsig#"))
            {
                reader.ReadStartElement();
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("RetrievalMethod", "http://www.w3.org/2000/09/xmldsig#"))
                    {
                        string attribute = reader.GetAttribute("URI");
                        if (!string.IsNullOrEmpty(attribute))
                        {
                            this._retrieval = attribute;
                        }
                        reader.Skip();
                    }
                    else if (this._keyInfoSerializer.CanReadKeyIdentifierClause(reader))
                    {
                        this._ski.Add(this._keyInfoSerializer.ReadKeyIdentifierClause(reader));
                    }
                    else if (reader.IsStartElement())
                    {
                        string str2 = reader.ReadOuterXml();
                        //if (DiagnosticUtil.TraceUtil.ShouldTrace(TraceEventType.Warning))
                        //{
                        //    DiagnosticUtil.TraceUtil.TraceString(TraceEventType.Warning, SR.GetString("ID8023", new object[] { reader.Name, reader.NamespaceURI, str2 }), new object[0]);
                        //}
                    }
                    reader.MoveToContent();
                }
                reader.MoveToContent();
                reader.ReadEndElement();
            }
        }

        // Properties
        public SecurityKeyIdentifier KeyIdentifier
        {
            get
            {
                return this._ski;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtil.ExceptionUtil.ThrowHelperArgumentNull("value");
                }
                this._ski = value;
            }
        }

        public string RetrievalMethod
        {
            get
            {
                return this._retrieval;
            }
        }
    }
}
