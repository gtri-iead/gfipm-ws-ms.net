using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Globalization;

namespace Common
{
    public static class XmlUtil
    {
        // Fields
        public const string LanguageAttribute = "xml:lang";
        public const string LanguageLocalname = "lang";
        public const string LanguageNamespaceUri = "http://www.w3.org/XML/1998/namespace";
        public const string LanguagePrefix = "xml";

        // Methods
        public static bool EqualsQName(XmlQualifiedName qname, string localName, string namespaceUri)
        {
            return (((null != qname) && StringComparer.Ordinal.Equals(localName, qname.Name)) && StringComparer.Ordinal.Equals(namespaceUri, qname.Namespace));
        }

        public static List<XmlElement> GetXmlElements(XmlNodeList nodeList)
        {
            if (nodeList == null)
            {
                throw new ArgumentNullException("nodeList");
            }
            List<XmlElement> list = new List<XmlElement>();
            foreach (XmlNode node in nodeList)
            {
                XmlElement item = node as XmlElement;
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public static XmlQualifiedName GetXsiType(XmlReader reader)
        {
            string attribute = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
            reader.MoveToElement();
            if (string.IsNullOrEmpty(attribute))
            {
                return null;
            }
            return ResolveQName(reader, attribute);
        }

        public static bool IsNil(XmlReader reader)
        {
            string attribute = reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
            return (!string.IsNullOrEmpty(attribute) && XmlConvert.ToBoolean(attribute));
        }

        public static bool IsValidXmlIDValue(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            if ((((val[0] < 'A') || (val[0] > 'Z')) && ((val[0] < 'a') || (val[0] > 'z'))) && (val[0] != '_'))
            {
                return (val[0] == ':');
            }
            return true;
        }

        public static XmlQualifiedName ResolveQName(XmlReader reader, string qstring)
        {
            string name = qstring;
            string prefix = string.Empty;
            int index = qstring.IndexOf(':');
            if (index > -1)
            {
                prefix = qstring.Substring(0, index);
                name = qstring.Substring(index + 1, qstring.Length - (index + 1));
            }
            return new XmlQualifiedName(name, reader.LookupNamespace(prefix));
        }

        public static void ValidateXsiType(XmlReader reader, string expectedTypeName, string expectedTypeNamespace)
        {
            ValidateXsiType(reader, expectedTypeName, expectedTypeNamespace, false);
        }

        public static void ValidateXsiType(XmlReader reader, string expectedTypeName, string expectedTypeNamespace, bool requireDeclaration)
        {
            XmlQualifiedName xsiType = GetXsiType(reader);
            if (null == xsiType)
            {
                if (requireDeclaration)
                {
                    string errorMsg = string.Format("An abstract element was encountered which does not specify its concrete type. "
                        + "Element name: '{0}' Element namespace: '{1}'", reader.LocalName, reader.NamespaceURI);

                    throw new InvalidOperationException( errorMsg );
                }
            }
            else if (!StringComparer.Ordinal.Equals(expectedTypeNamespace, xsiType.Namespace) || !StringComparer.Ordinal.Equals(expectedTypeName, xsiType.Name))
            {
                string errorMsg = string.Format("An element of an unexpected type was encountered. To support extended types in SAML2 assertions, extend Saml2SecurityTokenHandler. "
                    + "Expected type name: '{0}'  Expected type namespace: '{1}'  Encountered type name: '{2}'  Encountered type namespace: '{3}'",
                expectedTypeName, expectedTypeNamespace, xsiType.Name, xsiType.Namespace);
                
                throw new InvalidOperationException(errorMsg);
            }
        }
    }
}
