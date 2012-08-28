using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;
using System.ServiceModel.Security;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Protocols.XmlSignature;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common;
using GfipmCryptoTrustFabricModel;

namespace GfipmCryptographicTrustFabricSerializer
{
    public class CtfSerializer2
    {
        WSSecurityTokenSerializer _securityTokenSerializer = null;

        public WSSecurityTokenSerializer SecurityTokenSerializer
        {
            get { return _securityTokenSerializer; }
        }

        public CtfSerializer2()
        {
            _securityTokenSerializer = new WSSecurityTokenSerializer(SecurityVersion.WSSecurity11,
                TrustVersion.WSTrust13,
                SecureConversationVersion.WSSecureConversation13,
                false,
                null,
                null,
                null,
                WSSecurityTokenSerializer.DefaultInstance.MaximumKeyDerivationOffset,
                WSSecurityTokenSerializer.DefaultInstance.MaximumKeyDerivationLabelLength,
                WSSecurityTokenSerializer.DefaultInstance.MaximumKeyDerivationNonceLength);
        }

        public EntitiesDescriptorType ReadMetadata(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);

            if (reader.IsStartElement("EntitiesDescriptor", "urn:oasis:names:tc:SAML:2.0:metadata"))
            {
                return this.ReadEntitiesDescriptor(reader);
            }

            return null;
        }

        private EntitiesDescriptorType ReadEntitiesDescriptor(XmlReader reader)
        {
            EntitiesDescriptorType entitiesDescriptor = new EntitiesDescriptorType();

            //XmlReader reader2 = reader;
            EnvelopedSignatureReader reader2 = new EnvelopedSignatureReader(reader, this.SecurityTokenSerializer, 
                EmptySecurityTokenResolver.Instance, false, false, true);


            string attribute = reader2.GetAttribute("Name", null);
            if (!string.IsNullOrEmpty(attribute))
            {
                entitiesDescriptor.Name = attribute;
            }

            bool isEmptyElement = reader2.IsEmptyElement;
            reader2.ReadStartElement();
            if (!isEmptyElement)
            {
                List<EntityDescriptorType> entities = new List<EntityDescriptorType>();

                while (reader2.IsStartElement())
                {
                    if (reader2.IsStartElement("EntityDescriptor", "urn:oasis:names:tc:SAML:2.0:metadata"))
                    {
                        entities.Add(this.ReadEntityDescriptor(reader2));
                    }
                    else
                    {
                        if (this.IsXmlSignature(reader2))
                        {
                            if (!reader2.TryReadSignature())
                            {
                                reader2.Skip();
                            }
                        }
                    }
                }

                reader2.ReadEndElement();

                entitiesDescriptor.Items = entities.ToArray();
            }

            return entitiesDescriptor;
        }

        private bool IsXmlSignature(XmlReader reader)
        {
            if (reader.IsStartElement("Signature", "http://www.w3.org/2000/09/xmldsig#"))
            {
                return true;
            }
            return false;
        }

        private bool ReadCustomElement<T>(XmlReader reader, T target)
        {
            return false;
        }

        private EntityDescriptorType ReadEntityDescriptor(XmlReader inputReader)
        {
            EntityDescriptorType target = new EntityDescriptorType();
            XmlReader reader = inputReader;

            string attribute = reader.GetAttribute("entityID", null);
            if (!string.IsNullOrEmpty(attribute))
            {
                target.entityID = attribute;
            }

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                List<RoleDescriptorType> roles = new List<RoleDescriptorType>();

                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("SPSSODescriptor", "urn:oasis:names:tc:SAML:2.0:metadata"))
                    {
                        SPSSODescriptorType spssDEscriptor = this.ReadServiceProviderSingleSignOnDescriptor(reader);
                        roles.Add(spssDEscriptor);
                    }
                    else
                    {
                        if (reader.IsStartElement("IDPSSODescriptor", "urn:oasis:names:tc:SAML:2.0:metadata"))
                        {
                            IDPSSODescriptorType idpssDEscriptor = this.ReadIdentityProviderSingleSignOnDescriptor(reader);
                            roles.Add(idpssDEscriptor);
                            reader.Skip();
                            continue;
                        }
                        if (reader.IsStartElement("RoleDescriptor", "urn:oasis:names:tc:SAML:2.0:metadata"))
                        {
                            string type = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
                            int length = type.IndexOf(":", 0, StringComparison.Ordinal);

                            string prefix = type.Substring(0, length);
                            string nameSpace = reader.LookupNamespace(prefix);

                            if (StringComparer.Ordinal.Equals(nameSpace, "http://docs.oasis-open.org/wsfed/federation/200706"))
                            {
                                if (!StringComparer.Ordinal.Equals(type, prefix + ":ApplicationServiceType"))
                                {
                                    SecurityTokenServiceType stsDescriptor = this.ReadSecurityTokenServiceDescriptor(reader);
                                    roles.Add(stsDescriptor);
                                }
                                else
                                {
                                    ApplicationServiceType appSvcDescriptor = this.ReadApplicationServiceDescriptor(reader);
                                    roles.Add(appSvcDescriptor);
                                }
                            }
                            else
                            {
                                this.ReadCustomRoleDescriptor(type, reader, target);
                            }
                            continue;
                        }
                        if (reader.IsStartElement("Organization", "urn:oasis:names:tc:SAML:2.0:metadata"))
                        {
                            target.Organization = this.ReadOrganization(reader);
                        }
                        else
                        {
                            if (reader.IsStartElement("ContactPerson", "urn:oasis:names:tc:SAML:2.0:metadata"))
                            {
                                List<ContactType> contacts = new List<ContactType>();
                                contacts.Add(this.ReadContactPerson(reader));

                                target.ContactPerson = contacts.ToArray();
                                continue;
                            }
                            if (!this.ReadCustomElement<EntityDescriptorType>(reader, target))
                            {
                                reader.Skip();
                            }
                        }
                    }
                }
                reader.ReadEndElement();

                target.Items = roles.ToArray();
            }
            return target;
        }

        private void ReadCustomRoleDescriptor(string xsiType, XmlReader reader, EntityDescriptorType entityDescriptor)
        {
            reader.Skip();
        }

        private SPSSODescriptorType ReadServiceProviderSingleSignOnDescriptor(XmlReader reader)
        {
            SPSSODescriptorType spssoDescriptor = new SPSSODescriptorType();
            string attribute = reader.GetAttribute("AuthnRequestsSigned");
            if (!string.IsNullOrEmpty(attribute))
            {
                spssoDescriptor.AuthnRequestsSigned = XmlConvert.ToBoolean(attribute.ToLowerInvariant());
            }
            string assertionSigned = reader.GetAttribute("WantAssertionsSigned");
            if (!string.IsNullOrEmpty(assertionSigned))
            {
                spssoDescriptor.WantAssertionsSigned = XmlConvert.ToBoolean(assertionSigned.ToLowerInvariant());
            }

            this.ReadRoleDescriptorAttributes(reader, spssoDescriptor);
            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                List<IndexedEndpointType> indexedEndpoints = new List<IndexedEndpointType>();

                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("AssertionConsumerService", "urn:oasis:names:tc:SAML:2.0:metadata"))
                    {
                        IndexedEndpointType endpoint = this.ReadIndexedProtocolEndpoint(reader);
                        indexedEndpoints.Add(endpoint);
                    }
                    else if (!this.ReadSingleSignOnDescriptorElement(reader, spssoDescriptor) &&
                        !this.ReadCustomElement<SPSSODescriptorType>(reader, spssoDescriptor))
                    {
                        reader.Skip();
                    }
                }
                reader.ReadEndElement();

                spssoDescriptor.AssertionConsumerService = indexedEndpoints.ToArray();
            }

            return spssoDescriptor;
        }

        private void ReadRoleDescriptorAttributes(XmlReader reader, RoleDescriptorType roleDescriptor)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (roleDescriptor == null)
            {
                throw new ArgumentNullException("roleDescriptor");
            }
            string attribute = reader.GetAttribute("validUntil", null);
            if (!string.IsNullOrEmpty(attribute))
            {
                DateTime time;
                if (DateTime.TryParse(attribute, out time))
                {
                    roleDescriptor.validUntil = time;
                }
            }
            string errorURL = reader.GetAttribute("errorURL", null);
            if (!string.IsNullOrEmpty(errorURL))
            {
                Uri uri;
                if (UriUtil.TryCreateValidUri(errorURL, UriKind.RelativeOrAbsolute, out uri))
                {
                    roleDescriptor.errorURL = uri.ToString();
                }
            }
            string protocolSupportEnumeration = reader.GetAttribute("protocolSupportEnumeration", null);
            List<string> protocolsSupported = new List<string>();

            foreach (string protocolSupport in protocolSupportEnumeration.Split(new char[] { ' ' }))
            {
                string ps = protocolSupport.Trim();
                if (!string.IsNullOrEmpty(ps))
                {
                    protocolsSupported.Add(ps);
                }
            }

            roleDescriptor.protocolSupportEnumeration = protocolsSupported.ToArray();
        }

        private IndexedEndpointType ReadIndexedProtocolEndpoint(XmlReader reader)
        {
            Uri uri;
            Uri uri2;
            int num;

            IndexedEndpointType target = new IndexedEndpointType();
            string attribute = reader.GetAttribute("Binding", null);
            if (UriUtil.TryCreateValidUri(attribute, UriKind.RelativeOrAbsolute, out uri))
            {
                target.Binding = uri.ToString();
            }

            string uriString = reader.GetAttribute("Location", null);
            if (UriUtil.TryCreateValidUri(uriString, UriKind.RelativeOrAbsolute, out uri2))
            {
                target.Location = uri2.ToString();
            }

            string s = reader.GetAttribute("index", null);
            if (int.TryParse(s, out num))
            {
                target.index = Convert.ToUInt16(num);
            }

            string str4 = reader.GetAttribute("ResponseLocation", null);
            if (!string.IsNullOrEmpty(str4))
            {
                Uri uri3;
                if (UriUtil.TryCreateValidUri(str4, UriKind.RelativeOrAbsolute, out uri3))
                {
                    target.ResponseLocation = uri3.ToString();
                }

            }

            string str5 = reader.GetAttribute("isDefault", null);
            if (!string.IsNullOrEmpty(str5))
            {
                target.isDefault = XmlConvert.ToBoolean(str5.ToLowerInvariant());
            }

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                while (reader.IsStartElement())
                {
                    reader.Skip();
                }
                reader.ReadEndElement();
            }
            return target;
        }

        private bool ReadSingleSignOnDescriptorElement(XmlReader reader, SSODescriptorType ssoDescriptor)
        {
            if (ssoDescriptor == null)
            {
                throw new ArgumentNullException("ssoDescriptor");
            }

            if (!this.ReadRoleDescriptorElement(reader, ssoDescriptor))
            {
                if (reader.IsStartElement("ArtifactResolutionService", "urn:oasis:names:tc:SAML:2.0:metadata"))
                {
                    List<IndexedEndpointType> indexedEndpoints = new List<IndexedEndpointType>();
                    IndexedEndpointType endpoint = this.ReadIndexedProtocolEndpoint(reader);
                    indexedEndpoints.Add(endpoint);

                    ssoDescriptor.ArtifactResolutionService = indexedEndpoints.ToArray();
                    return true;
                }
                if (reader.IsStartElement("SingleLogoutService", "urn:oasis:names:tc:SAML:2.0:metadata"))
                {
                    List<EndpointType1> endPoints = new List<EndpointType1>();
                    endPoints.Add(this.ReadProtocolEndpoint(reader));
                    ssoDescriptor.SingleLogoutService = endPoints.ToArray();
                    return true;
                }
                if (!reader.IsStartElement("NameIDFormat", "urn:oasis:names:tc:SAML:2.0:metadata"))
                {
                    return this.ReadCustomElement<SSODescriptorType>(reader, ssoDescriptor);
                }
                string uriString = reader.ReadElementContentAsString("NameIDFormat", "urn:oasis:names:tc:SAML:2.0:metadata");
                if (!UriUtil.CanCreateValidUri(uriString, UriKind.Absolute))
                {
                    throw new InvalidOperationException("NameIDFormat attribute");
                }
                List<string> nameIDs = new List<string>();
                nameIDs.Add(uriString);
                
                ssoDescriptor.NameIDFormat = nameIDs.ToArray();
            }
            return true;
        }

        private bool ReadRoleDescriptorElement(XmlReader reader, RoleDescriptorType roleDescriptor)
        {
            if (roleDescriptor == null)
            {
                throw new ArgumentNullException("roleDescriptor");
            }

            if (reader.IsStartElement("Organization", "urn:oasis:names:tc:SAML:2.0:metadata"))
            {
                roleDescriptor.Organization = this.ReadOrganization(reader);
                return true;
            }
            if (reader.IsStartElement("KeyDescriptor", "urn:oasis:names:tc:SAML:2.0:metadata"))
            {
                List<KeyDescriptorType> keyDescriptors = new List<KeyDescriptorType>();

                keyDescriptors.Add(this.ReadKeyDescriptor(reader));

                roleDescriptor.KeyDescriptor = keyDescriptors.ToArray();
                                
                return true;
            }
            if (reader.IsStartElement("ContactPerson", "urn:oasis:names:tc:SAML:2.0:metadata"))
            {
                List<ContactType> contacts = new List<ContactType>();
                contacts.Add(this.ReadContactPerson(reader));

                roleDescriptor.ContactPerson = contacts.ToArray();

                return true;
            }
            return this.ReadCustomElement<RoleDescriptorType>(reader, roleDescriptor);
        }

        private OrganizationType ReadOrganization(XmlReader reader)
        {
            OrganizationType target = new OrganizationType();

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                List<localizedNameType> orgNames = new List<localizedNameType>();
                List<localizedNameType> dispNames = new List<localizedNameType>();
                List<localizedURIType> urlNames = new List<localizedURIType>();

                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("OrganizationName", "urn:oasis:names:tc:SAML:2.0:metadata"))
                    {
                        orgNames.Add(this.ReadLocalizedName(reader));
                    }
                    else
                    {
                        if (reader.IsStartElement("OrganizationDisplayName", "urn:oasis:names:tc:SAML:2.0:metadata"))
                        {
                            dispNames.Add(this.ReadLocalizedName(reader));
                            continue;
                        }
                        if (reader.IsStartElement("OrganizationURL", "urn:oasis:names:tc:SAML:2.0:metadata"))
                        {
                            urlNames.Add(this.ReadLocalizedUri(reader));
                            continue;
                        }

                        reader.Skip();
                    }
                }

                target.OrganizationName = orgNames.ToArray();
                target.OrganizationDisplayName = dispNames.ToArray();
                target.OrganizationURL = urlNames.ToArray();

                reader.ReadEndElement();
            }
            return target;
        }

        private localizedNameType ReadLocalizedName(XmlReader reader)
        {
            localizedNameType target = new localizedNameType();
            string attribute = reader.GetAttribute("lang", "http://www.w3.org/XML/1998/namespace");

            target.lang = CultureInfo.GetCultureInfo(attribute).ToString();

            bool isEmptyElement = reader.IsEmptyElement;
            string name = reader.Name;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                target.Value = reader.ReadContentAsString();
                while (reader.IsStartElement())
                {
                    reader.Skip();
                }
                reader.ReadEndElement();
            }

            return target;
        }

        private localizedURIType ReadLocalizedUri(XmlReader reader)
        {
            localizedURIType target = new localizedURIType();
            string attribute = reader.GetAttribute("lang", "http://www.w3.org/XML/1998/namespace");

            target.lang = CultureInfo.GetCultureInfo(attribute).ToString();

            bool isEmptyElement = reader.IsEmptyElement;
            string name = reader.Name;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                Uri uri2;
                string uriString = reader.ReadContentAsString();
                if (UriUtil.TryCreateValidUri(uriString, UriKind.RelativeOrAbsolute, out uri2))
                {
                    target.Value = uri2.ToString();
                }

                while (reader.IsStartElement())
                {
                    reader.Skip();
                }
                reader.ReadEndElement();
            }

            return target;
        }

        private KeyDescriptorType ReadKeyDescriptor(XmlReader reader)
        {
            KeyDescriptorType target = new KeyDescriptorType();

            string attribute = reader.GetAttribute("use", null);
            if (!string.IsNullOrEmpty(attribute))
            {
                target.use = GetKeyDescriptorType(attribute);
            }

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("KeyInfo", "http://www.w3.org/2000/09/xmldsig#"))
                    {
                        SecurityKeyIdentifier keyIdent = this.SecurityTokenSerializer.ReadKeyIdentifier(reader);
                        KeyInfoType keyInfo = new KeyInfoType();
                        X509RawDataKeyIdentifierClause x509IdentClause = keyIdent.Find<X509RawDataKeyIdentifierClause>();

                        if (x509IdentClause != null)
                        {
                            List<byte[]> list = new List<byte[]>();

                            X509DataType x509DataType = new X509DataType();
                            byte[] rawx509Cert = x509IdentClause.GetX509RawData();
                            list.Add(rawx509Cert);
                            x509DataType.Items = list.ToArray();

                            keyInfo.Items = new X509DataType[] { x509DataType };
                        }

                        target.KeyInfo = keyInfo;
                    }
                    else
                    {
                        if (reader.IsStartElement("EncryptionMethod", "urn:oasis:names:tc:SAML:2.0:metadata"))
                        {
                            string str2 = reader.GetAttribute("Algorithm");
                            if (!string.IsNullOrEmpty(str2) && UriUtil.CanCreateValidUri(str2, UriKind.Absolute))
                            {
                                EncryptionMethodType enc = new EncryptionMethodType();
                                enc.Algorithm = str2;

                                target.EncryptionMethod = new EncryptionMethodType[] { enc };
                            }
                            isEmptyElement = reader.IsEmptyElement;
                            reader.ReadStartElement("EncryptionMethod", "urn:oasis:names:tc:SAML:2.0:metadata");
                            if (!isEmptyElement)
                            {
                                while (reader.IsStartElement())
                                {
                                    if (!this.ReadCustomElement<KeyDescriptorType>(reader, target))
                                    {
                                        reader.Skip();
                                    }
                                }
                                reader.ReadEndElement();
                            }
                            continue;
                        }
                        if (!this.ReadCustomElement<KeyDescriptorType>(reader, target))
                        {
                            reader.Skip();
                        }
                    }
                }
                reader.ReadEndElement();
            }

            return target;
        }

        private static KeyTypes GetKeyDescriptorType(string keyType)
        {
            if (StringComparer.Ordinal.Equals(keyType, "encryption"))
            {
                return KeyTypes.encryption;
            }

            return KeyTypes.signing;
        }

        private ContactType ReadContactPerson(XmlReader reader)
        {
            ContactType target = new ContactType();
            string attribute = reader.GetAttribute("contactType", null);
            bool found = false;
            target.contactType = GetContactPersonType(attribute, out found);

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                List<string> emailAddresses = new List<string>();
                List<string> phoneNumbers = new List<string>();

                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("Company", "urn:oasis:names:tc:SAML:2.0:metadata"))
                    {
                        target.Company = reader.ReadElementContentAsString("Company", "urn:oasis:names:tc:SAML:2.0:metadata");
                    }
                    else
                    {
                        if (reader.IsStartElement("GivenName", "urn:oasis:names:tc:SAML:2.0:metadata"))
                        {
                            target.GivenName = reader.ReadElementContentAsString("GivenName", "urn:oasis:names:tc:SAML:2.0:metadata");
                            continue;
                        }
                        if (reader.IsStartElement("SurName", "urn:oasis:names:tc:SAML:2.0:metadata"))
                        {
                            target.SurName = reader.ReadElementContentAsString("SurName", "urn:oasis:names:tc:SAML:2.0:metadata");
                            continue;
                        }
                        if (reader.IsStartElement("EmailAddress", "urn:oasis:names:tc:SAML:2.0:metadata"))
                        {
                            string email = reader.ReadElementContentAsString("EmailAddress", "urn:oasis:names:tc:SAML:2.0:metadata");
                            if (!string.IsNullOrEmpty(email))
                            {
                                emailAddresses.Add(email);
                            }
                            continue;
                        }
                        if (reader.IsStartElement("TelephoneNumber", "urn:oasis:names:tc:SAML:2.0:metadata"))
                        {
                            string phone = reader.ReadElementContentAsString("TelephoneNumber", "urn:oasis:names:tc:SAML:2.0:metadata");
                            if (!string.IsNullOrEmpty(phone))
                            {
                                phoneNumbers.Add(phone);
                            }
                            continue;
                        }
                        if (!this.ReadCustomElement<ContactType>(reader, target))
                        {
                            reader.Skip();
                        }
                    }
                }

                target.TelephoneNumber = phoneNumbers.ToArray();
                target.EmailAddress = emailAddresses.ToArray();

                reader.ReadEndElement();
            }
            return target;
        }

        private static ContactTypeType GetContactPersonType(string conactType, out bool found)
        {
            if (conactType == null)
            {
                throw new ArgumentNullException("contactType");
            }
            found = true;
            if (!StringComparer.Ordinal.Equals(conactType, "unspecified"))
            {
                if (StringComparer.Ordinal.Equals(conactType, "administrative"))
                {
                    return ContactTypeType.administrative;
                }
                if (StringComparer.Ordinal.Equals(conactType, "billing"))
                {
                    return ContactTypeType.billing;
                }
                if (StringComparer.Ordinal.Equals(conactType, "other"))
                {
                    return ContactTypeType.other;
                }
                if (StringComparer.Ordinal.Equals(conactType, "support"))
                {
                    return ContactTypeType.support;
                }
                if (StringComparer.Ordinal.Equals(conactType, "technical"))
                {
                    return ContactTypeType.technical;
                }
                found = false;
            }
            return ContactTypeType.other; // ContactTypeType.Unspecified;
        }

        private IDPSSODescriptorType ReadIdentityProviderSingleSignOnDescriptor(XmlReader reader)
        {
            IDPSSODescriptorType roleDescriptor = new IDPSSODescriptorType();
            this.ReadSingleSignOnDescriptorAttributes(reader, roleDescriptor);
            string attribute = reader.GetAttribute("WantAuthnRequestsSigned");
            if (!string.IsNullOrEmpty(attribute))
            {
                try
                {
                    roleDescriptor.WantAuthnRequestsSigned = XmlConvert.ToBoolean(attribute.ToLowerInvariant());
                }
                catch (FormatException)
                {
                    throw new InvalidOperationException("WantAuthnRequestsSigned attribute");
                }
            }
            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("SingleSignOnService", "urn:oasis:names:tc:SAML:2.0:metadata"))
                    {
                        List<EndpointType1> ep = new List<EndpointType1>();
                        ep.Add(this.ReadProtocolEndpoint(reader));
                        roleDescriptor.SingleSignOnService = ep.ToArray();
                    }
                    else
                    {
                        if (reader.IsStartElement("Attribute", "urn:oasis:names:tc:SAML:2.0:assertion"))
                        {
                            AttributeType attrib = new AttributeType();
                            Saml2Attribute samlAttrib = this.ReadAttribute(reader);
                            attrib.Name = samlAttrib.Name;
                            attrib.AttributeValue = new List<string>(samlAttrib.Values).ToArray();
                            attrib.NameFormat = samlAttrib.NameFormat.ToString();
                            attrib.FriendlyName = samlAttrib.FriendlyName;
                            attrib.Name = samlAttrib.Name;

                            roleDescriptor.Attribute = new AttributeType[] { attrib };
                            continue;
                        }
                        if (!this.ReadSingleSignOnDescriptorElement(reader, roleDescriptor) &&
                            !this.ReadCustomElement<IDPSSODescriptorType>(reader, roleDescriptor))
                        {
                            reader.Skip();
                        }
                    }
                }
                reader.ReadEndElement();
            }
            return roleDescriptor;
        }

        private SecurityTokenServiceType ReadSecurityTokenServiceDescriptor(XmlReader reader)
        {
            SecurityTokenServiceType roleDescriptor = new SecurityTokenServiceType();
            this.ReadWebServiceDescriptorAttributes(reader, roleDescriptor);

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                List<EndpointReferenceType> securityTokenServiceEndpoints = new List<EndpointReferenceType>();
                List<EndpointReferenceType> passiveRequestorEndpoints = new List<EndpointReferenceType>();
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("SecurityTokenServiceEndpoint", "http://docs.oasis-open.org/wsfed/federation/200706"))
                    {
                        isEmptyElement = reader.IsEmptyElement;
                        reader.ReadStartElement();
                        if (!isEmptyElement && reader.IsStartElement())
                        {
                            EndpointAddress endpointAddress = EndpointAddress.ReadFrom(this.GetAddressingVersion(reader.NamespaceURI), reader);
                            EndpointReferenceType endPointRefType = new EndpointReferenceType();
                            AttributedURIType address = new AttributedURIType();
                            address.Value = endpointAddress.Uri.ToString();

                            endPointRefType.Address = address;

                            securityTokenServiceEndpoints.Add(endPointRefType);

                            reader.ReadEndElement();
                        }
                    }
                    else
                    {
                        if (reader.IsStartElement("PassiveRequestorEndpoint", "http://docs.oasis-open.org/wsfed/federation/200706"))
                        {
                            isEmptyElement = reader.IsEmptyElement;
                            reader.ReadStartElement();
                            if (!isEmptyElement && reader.IsStartElement())
                            {
                                EndpointAddress endpointAddress = EndpointAddress.ReadFrom(this.GetAddressingVersion(reader.NamespaceURI), reader);
                                EndpointReferenceType endPointRefType = new EndpointReferenceType();
                                AttributedURIType address = new AttributedURIType();
                                address.Value = endpointAddress.Uri.ToString();

                                endPointRefType.Address = address;

                                passiveRequestorEndpoints.Add(endPointRefType);

                                reader.ReadEndElement();
                            }
                            continue;
                        }
                        if (!this.ReadWebServiceDescriptorElement(reader, roleDescriptor) &&
                            !this.ReadCustomElement<SecurityTokenServiceType>(reader, roleDescriptor))
                        {
                            reader.Skip();
                        }
                    }
                }
                roleDescriptor.SecurityTokenServiceEndpoint = securityTokenServiceEndpoints.ToArray();
                roleDescriptor.PassiveRequestorEndpoint = passiveRequestorEndpoints.ToArray();

                reader.ReadEndElement();
            }
            return roleDescriptor;
        }

        private ApplicationServiceType ReadApplicationServiceDescriptor(XmlReader reader)
        {
            ApplicationServiceType roleDescriptor = new ApplicationServiceType();
            this.ReadWebServiceDescriptorAttributes(reader, roleDescriptor);
            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {

                List<EndpointReferenceType> appSvcEndPoints = new List<EndpointReferenceType>();
                List<EndpointReferenceType> passReqEndPoints = new List<EndpointReferenceType>();

                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("ApplicationServiceEndpoint", "http://docs.oasis-open.org/wsfed/federation/200706"))
                    {
                        isEmptyElement = reader.IsEmptyElement;
                        reader.ReadStartElement();
                        if (!isEmptyElement && reader.IsStartElement())
                        {
                            EndpointAddress endpointAddress = EndpointAddress.ReadFrom(this.GetAddressingVersion(reader.NamespaceURI), reader);
                            EndpointReferenceType endpointRefType = new EndpointReferenceType();
                            AttributedURIType address = new AttributedURIType();
                            address.Value = endpointAddress.Uri.ToString();

                            endpointRefType.Address = address;
                            
                            appSvcEndPoints.Add(endpointRefType);

                            reader.ReadEndElement();
                        }
                    }
                    else
                    {
                        if (reader.IsStartElement("PassiveRequestorEndpoint", "http://docs.oasis-open.org/wsfed/federation/200706"))
                        {
                            isEmptyElement = reader.IsEmptyElement;
                            reader.ReadStartElement();
                            if (!isEmptyElement && reader.IsStartElement())
                            {
                                EndpointAddress endpointAddress = EndpointAddress.ReadFrom(this.GetAddressingVersion(reader.NamespaceURI), reader);
                                EndpointReferenceType endpointRefType = new EndpointReferenceType();
                                AttributedURIType address = new AttributedURIType();
                                address.Value = endpointAddress.Uri.ToString();

                                endpointRefType.Address = address;
                            
                                passReqEndPoints.Add(endpointRefType);
                                
                                reader.ReadEndElement();
                            }
                            continue;
                        }
                        if (!this.ReadWebServiceDescriptorElement(reader, roleDescriptor) && 
                            !this.ReadCustomElement<ApplicationServiceType>(reader, roleDescriptor))
                        {
                            reader.Skip();
                        }
                    }
                }

                roleDescriptor.ApplicationServiceEndpoint = appSvcEndPoints.ToArray();
                roleDescriptor.PassiveRequestorEndpoint = passReqEndPoints.ToArray();

                reader.ReadEndElement();
            }
            return roleDescriptor;
        }

        private EndpointType1 ReadProtocolEndpoint(XmlReader reader)
        {
            Uri uri;
            Uri uri2;

            EndpointType1 target = new EndpointType1();

            string attribute = reader.GetAttribute("Binding", null);
            if (!UriUtil.TryCreateValidUri(attribute, UriKind.RelativeOrAbsolute, out uri))
            {
                throw new InvalidOperationException("Binding attribute");
            }
            target.Binding = uri.ToString();

            string uriString = reader.GetAttribute("Location", null);
            if (!UriUtil.TryCreateValidUri(uriString, UriKind.RelativeOrAbsolute, out uri2))
            {
                throw new InvalidOperationException("Location attribute");
            }
            target.Location = uri2.ToString();

            string str3 = reader.GetAttribute("ResponseLocation", null);
            if (!string.IsNullOrEmpty(str3))
            {
                Uri uri3;
                if (!UriUtil.TryCreateValidUri(str3, UriKind.RelativeOrAbsolute, out uri3))
                {
                    throw new InvalidOperationException("ResponseLocation attribute");
                }
                target.ResponseLocation = uri3.ToString();
            }

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                while (reader.IsStartElement())
                {
                    if (!this.ReadCustomElement<EndpointType1>(reader, target))
                    {
                        reader.Skip();
                    }
                }
                reader.ReadEndElement();
            }
            return target;
        }

        private void ReadSingleSignOnDescriptorAttributes(XmlReader reader, SSODescriptorType roleDescriptor)
        {
            this.ReadRoleDescriptorAttributes(reader, roleDescriptor);
        }

        private Saml2Attribute ReadAttribute(XmlReader reader)
        {
            Saml2Attribute attribute2;

            if (!reader.IsStartElement("Attribute", "urn:oasis:names:tc:SAML:2.0:assertion"))
            {
                reader.ReadStartElement("Attribute", "urn:oasis:names:tc:SAML:2.0:assertion");
            }

            bool isEmptyElement = reader.IsEmptyElement;
            XmlUtil.ValidateXsiType(reader, "AttributeType", "urn:oasis:names:tc:SAML:2.0:assertion");
            string str = reader.GetAttribute("Name");
            if (string.IsNullOrEmpty(str))
            {
                throw new InvalidOperationException("Name attribute");
            }
            Saml2Attribute attribute = new Saml2Attribute(str);
            str = reader.GetAttribute("NameFormat");
            if (!string.IsNullOrEmpty(str))
            {
                if (!UriUtil.CanCreateValidUri(str, UriKind.Absolute))
                {
                    throw new InvalidOperationException("NameFormat attribute");
                }
                attribute.NameFormat = new Uri(str);
            }
            attribute.FriendlyName = reader.GetAttribute("FriendlyName");
            reader.Read();
            if (!isEmptyElement)
            {
                while (reader.IsStartElement("AttributeValue", "urn:oasis:names:tc:SAML:2.0:assertion"))
                {
                    bool flag2 = reader.IsEmptyElement;
                    bool flag3 = XmlUtil.IsNil(reader);
                    XmlUtil.ValidateXsiType(reader, "string", "http://www.w3.org/2001/XMLSchema");
                    if (flag3)
                    {
                        reader.Read();
                        if (!flag2)
                        {
                            reader.ReadEndElement();
                        }
                        attribute.Values.Add(null);
                    }
                    else if (flag2)
                    {
                        reader.Read();
                        attribute.Values.Add("");
                    }
                    else
                    {
                        attribute.Values.Add(reader.ReadElementString());
                    }
                }
                reader.ReadEndElement();
            }

            attribute2 = attribute;

            return attribute2;
        }

        private void ReadWebServiceDescriptorAttributes(XmlReader reader, WebServiceDescriptorType1 roleDescriptor)
        {
            if (roleDescriptor == null)
            {
                throw new ArgumentNullException("roleDescriptor");
            }
            this.ReadRoleDescriptorAttributes(reader, roleDescriptor);
            string attribute = reader.GetAttribute("ServiceDisplayName", null);
            if (!string.IsNullOrEmpty(attribute))
            {
                roleDescriptor.ServiceDisplayName = attribute;
            }
            string str2 = reader.GetAttribute("ServiceDescription", null);
            if (!string.IsNullOrEmpty(str2))
            {
                roleDescriptor.ServiceDescription = str2;
            }
        }

        private AddressingVersion GetAddressingVersion(string namespaceUri)
        {
            if (string.IsNullOrEmpty(namespaceUri))
            {
                throw new ArgumentNullException("namespaceUri");
            }
            if (StringComparer.Ordinal.Equals(namespaceUri, "http://www.w3.org/2005/08/addressing"))
            {
                return AddressingVersion.WSAddressing10;
            }
            if (StringComparer.Ordinal.Equals(namespaceUri, "http://schemas.xmlsoap.org/ws/2004/08/addressing"))
            {
                return AddressingVersion.WSAddressingAugust2004;
            }
            return null;
        }

        public virtual bool ReadWebServiceDescriptorElement(XmlReader reader, WebServiceDescriptorType1 roleDescriptor)
        {
            if (roleDescriptor == null)
            {
                throw new ArgumentNullException("roleDescriptor");
            }
            if (roleDescriptor.ClaimTypesOffered == null)
            {
                roleDescriptor.ClaimTypesOffered = new ClaimTypesOfferedType();
            }
            if (roleDescriptor.ClaimTypesRequested == null)
            {
                roleDescriptor.ClaimTypesRequested = new ClaimTypesRequestedType();
            }
            if (roleDescriptor.TokenTypesOffered == null)
            {
                roleDescriptor.TokenTypesOffered = new TokenTypesOfferedType();
            }

            if (!this.ReadRoleDescriptorElement(reader, roleDescriptor))
            {
                if (reader.IsStartElement("TargetScopes", "http://docs.oasis-open.org/wsfed/federation/200706"))
                {
                    bool flag = reader.IsEmptyElement;
                    reader.ReadStartElement();
                    if (!flag)
                    {
                        List<EndpointReferenceType> targetScopes = null;
                        if (roleDescriptor.TargetScopes != null)
                        {
                            targetScopes = new List<EndpointReferenceType>(roleDescriptor.TargetScopes);
                        }
                        else
                        {
                            targetScopes = new List<EndpointReferenceType>();
                        }

                        while (reader.IsStartElement())
                        {
                            EndpointAddress endpointAddress = EndpointAddress.ReadFrom(this.GetAddressingVersion(reader.NamespaceURI), reader);
                            EndpointReferenceType endpointRefType = new EndpointReferenceType();
                            AttributedURIType address = new AttributedURIType();
                            address.Value = endpointAddress.Uri.ToString();

                            endpointRefType.Address = address;
                            
                            targetScopes.Add(endpointRefType);
                        }
                        roleDescriptor.TargetScopes = targetScopes.ToArray();
                        reader.ReadEndElement();
                    }
                    return true;
                }
                if (reader.IsStartElement("ClaimTypesOffered", "http://docs.oasis-open.org/wsfed/federation/200706"))
                {
                    bool flag2 = reader.IsEmptyElement;
                    reader.ReadStartElement();
                    if (!flag2)
                    {
                        List<ClaimType> claimTypesOffered = null;

                        if (roleDescriptor.ClaimTypesOffered != null && roleDescriptor.ClaimTypesOffered.ClaimType != null)
                        {
                            claimTypesOffered = new List<ClaimType>(roleDescriptor.ClaimTypesOffered.ClaimType);
                        }
                        else
                        {
                            claimTypesOffered = new List<ClaimType>();
                        }

                        while (reader.IsStartElement())
                        {
                            if (reader.IsStartElement("ClaimType", "http://docs.oasis-open.org/wsfed/authorization/200706"))
                            {
                                claimTypesOffered.Add(this.ReadDisplayClaim(reader));
                            }
                            else
                            {
                                reader.Skip();
                            }
                        }

                        roleDescriptor.ClaimTypesOffered.ClaimType = claimTypesOffered.ToArray();
                        reader.ReadEndElement();
                    }
                    return true;
                }
                if (reader.IsStartElement("ClaimTypesRequested", "http://docs.oasis-open.org/wsfed/federation/200706"))
                {
                    bool flag3 = reader.IsEmptyElement;
                    reader.ReadStartElement();
                    if (!flag3)
                    {
                        List<ClaimType> claimTypesRequested = null;
                        if (roleDescriptor.ClaimTypesRequested != null && roleDescriptor.ClaimTypesRequested.ClaimType != null)
                        {
                            claimTypesRequested = new List<ClaimType>(roleDescriptor.ClaimTypesRequested.ClaimType);
                        }
                        else
                        {
                            claimTypesRequested = new List<ClaimType>();
                        }

                        while (reader.IsStartElement())
                        {
                            if (reader.IsStartElement("ClaimType", "http://docs.oasis-open.org/wsfed/authorization/200706"))
                            {
                                claimTypesRequested.Add(this.ReadDisplayClaim(reader));
                            }
                            else
                            {
                                reader.Skip();
                            }
                        }
                        roleDescriptor.ClaimTypesRequested.ClaimType = claimTypesRequested.ToArray();
                        reader.ReadEndElement();
                    }
                    return true;
                }
                if (!reader.IsStartElement("TokenTypesOffered", "http://docs.oasis-open.org/wsfed/federation/200706"))
                {
                    return this.ReadCustomElement<WebServiceDescriptorType1>(reader, roleDescriptor);
                }
                bool isEmptyElement = reader.IsEmptyElement;
                reader.ReadStartElement("TokenTypesOffered", "http://docs.oasis-open.org/wsfed/federation/200706");
                if (!isEmptyElement)
                {
                    List<TokenType> tokenTypesOffered = null;

                    if (roleDescriptor.TokenTypesOffered != null && roleDescriptor.TokenTypesOffered.TokenType != null)
                    {
                        tokenTypesOffered = new List<TokenType>(roleDescriptor.TokenTypesOffered.TokenType);
                    }
                    else
                    {
                        tokenTypesOffered = new List<TokenType>();
                    }

                    while (reader.IsStartElement())
                    {
                        if (reader.IsStartElement("TokenType", "http://docs.oasis-open.org/wsfed/federation/200706"))
                        {
                            Uri uri;
                            string attribute = reader.GetAttribute("Uri", null);
                            if (!UriUtil.TryCreateValidUri(attribute, UriKind.Absolute, out uri))
                            {
                                throw new InvalidOperationException("TokenType Uri attribute");
                            }
                            TokenType tokenType = new TokenType();
                            tokenType.Uri = uri.ToString();
                            tokenTypesOffered.Add(tokenType);
                            
                            isEmptyElement = reader.IsEmptyElement;
                            reader.ReadStartElement();
                            if (!isEmptyElement)
                            {
                                reader.ReadEndElement();
                            }
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                    roleDescriptor.TokenTypesOffered.TokenType = tokenTypesOffered.ToArray();
                    reader.ReadEndElement();
                }
            }
            return true;
        }

        private ClaimType ReadDisplayClaim(XmlReader reader)
        {
            string attribute = reader.GetAttribute("Uri", null);
            if (!UriUtil.CanCreateValidUri(attribute, UriKind.Absolute))
            {
                throw new InvalidOperationException("Uri attribute");
            }
            
            ClaimType claim = new ClaimType();

            bool flag = true;
            string str2 = reader.GetAttribute("Optional");
            if (!string.IsNullOrEmpty(str2))
            {
                try
                {
                    flag = XmlConvert.ToBoolean(str2.ToLowerInvariant());
                }
                catch (FormatException)
                {
                    throw new InvalidOperationException("Optional attribute");
                }
            }
            claim.Optional = flag;
            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("DisplayName", "http://docs.oasis-open.org/wsfed/authorization/200706"))
                    {
                        claim.DisplayName = new DisplayNameType();
                        claim.DisplayName.Value = reader.ReadElementContentAsString("DisplayName", "http://docs.oasis-open.org/wsfed/authorization/200706");
                    }
                    else
                    {
                        if (reader.IsStartElement("Description", "http://docs.oasis-open.org/wsfed/authorization/200706"))
                        {
                            claim.Description = new DescriptionType();
                            claim.Description.Value = reader.ReadElementContentAsString("Description", "http://docs.oasis-open.org/wsfed/authorization/200706");
                            continue;
                        }
                        reader.Skip();
                    }
                }
                reader.ReadEndElement();
            }
            return claim;
        }
    }
}
