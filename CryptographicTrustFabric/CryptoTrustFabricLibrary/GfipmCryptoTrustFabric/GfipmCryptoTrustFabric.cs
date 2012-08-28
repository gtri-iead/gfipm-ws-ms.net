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
using System.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;

using GfipmCryptoTrustFabricModel;

namespace GfipmCryptoTrustFabric
{
    public class GfipmCryptoTrustFabric
    {
        private EntitiesDescriptorType _gfipmEntities = null;

        public EntitiesDescriptorType GfipmEntities
        {
            get { return _gfipmEntities; }
        }
        
        public GfipmCryptoTrustFabric()
        {
            // Open embedded Trust Fabric document
            XmlDocument trustFabricXmlDoc = GetTrustFabricXmlDocument();
 
            _gfipmEntities = CtfSerializer.Deserialize<EntitiesDescriptorType>(trustFabricXmlDoc.InnerXml, 
                "EntitiesDescriptor", "urn:oasis:names:tc:SAML:2.0:metadata") as EntitiesDescriptorType;
        }

        public GfipmCryptoTrustFabric(string ctfPath)
        {
            _gfipmEntities = OpenCtfFile(ctfPath);
        }
        
        public EntitiesDescriptorType OpenCtfFile(string ctfPath)
        {
            var xmlDoc = new XmlDocument();

            xmlDoc.PreserveWhitespace = true;

            xmlDoc.Load(ctfPath);
                        
            return CtfSerializer.Deserialize<EntitiesDescriptorType>(xmlDoc.InnerXml, 
                "EntitiesDescriptor", "urn:oasis:names:tc:SAML:2.0:metadata") as EntitiesDescriptorType;
        }

        public EntitiesDescriptorType OpenCtfFileAndValidateSignature(string ctfPath)
        {
            var xmlDoc = new XmlDocument();

            xmlDoc.PreserveWhitespace = true;

            xmlDoc.Load(ctfPath);

            if (!XmlSignatureUtils.VerifySignature(xmlDoc))
            {
                throw new CryptographicException("The CTF document XML Signature is not valid");
            }

            return CtfSerializer.Deserialize<EntitiesDescriptorType>(xmlDoc.InnerXml,
                "EntitiesDescriptor", "urn:oasis:names:tc:SAML:2.0:metadata") as EntitiesDescriptorType;
        }


        public EntitiesDescriptorType LoadFromXml(string xml)
        {
            var xmlDoc = new XmlDocument();

            xmlDoc.PreserveWhitespace = true;

            xmlDoc.LoadXml(xml);

            if (!XmlSignatureUtils.VerifySignature(xmlDoc))
            {
                throw new CryptographicException("The CTF document XML Signature is not valid");
            }

            return CtfSerializer.Deserialize<EntitiesDescriptorType>(xmlDoc.InnerXml,
                "EntitiesDescriptor", "urn:oasis:names:tc:SAML:2.0:metadata") as EntitiesDescriptorType;

        }

        private XmlDocument GetTrustFabricXmlDocument()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "GfipmCryptoTrustFabric.gfipm-trust-fabric-model2-sample-signed.xml");

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);

            return xmlDoc;
        }

        public bool Contains(string entityID)
        {
            var result = from entity in _gfipmEntities.Items
                         where ((EntityDescriptorType)entity).entityID == entityID
                         select entity;

            if (result.Count() > 0)
            {
                return true;
            }

            return false;
        }

        public bool Contains(List<string> entityIDs)
        {
            foreach (string entityID in entityIDs)
            {
                if (!Contains(entityID))
                {
                    return false;
                }
            }

            return true;
        }
        
        public string GetWspEntityIdFromEndpointAddress(string enpointAddressUrl)
        {
            if (_gfipmEntities == null)
            {
                throw new Exception("trust fabric");
            }

            foreach (EntityDescriptorType entity in _gfipmEntities.Items)
            {
                foreach (RoleDescriptorType roleDescriptor in entity.Items)
                {
                    if (roleDescriptor is GFIPMWebServiceProviderType)
                    {
                        GFIPMWebServiceProviderType wsp = roleDescriptor as GFIPMWebServiceProviderType;

                        foreach (EndpointReferenceType ep in wsp.WebServiceEndpoint)
                        {
                            if (ep.Address.Value.ToUpperInvariant() == enpointAddressUrl.ToUpperInvariant())
                            {
                                return entity.entityID;
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        public string GetAdsEndpointAddressFromEntityId(string entityID)
        {
            if (_gfipmEntities == null)
            {
                throw new Exception("trust fabric");
            }

            if (_gfipmEntities.Items == null)
            {
                throw new Exception("trust fabric Items");
            }

            EntityDescriptorType entity = null;

            foreach (EntityDescriptorType ed in _gfipmEntities.Items)
            {
                if (ed.entityID == entityID)
                {
                    entity = ed;
                    break;
                }
            }

            if (entity == null)
            {
                throw new Exception("Entity ID not found");
            }

            foreach (RoleDescriptorType roleDescriptor in entity.Items)
            {
                if (roleDescriptor is GFIPMAssertionDelegateServiceType)
                {
                    GFIPMAssertionDelegateServiceType adsService = roleDescriptor as GFIPMAssertionDelegateServiceType;

                    return adsService.DelegatedTokenServiceEndpoint[0].Address.Value;
                }
            }            

            return string.Empty;
        }

        public X509Certificate2 GetAdsSigningCertificateFromEndpointAddress(string adsEndpointAddressUrl)
        {
            if (_gfipmEntities == null)
            {
                throw new Exception("trust fabric");
            }

            foreach (EntityDescriptorType entity in _gfipmEntities.Items)
            {
                foreach (RoleDescriptorType roleDescriptor in entity.Items)
                {
                    if (roleDescriptor is GFIPMAssertionDelegateServiceType)
                    {
                        GFIPMAssertionDelegateServiceType adsService = roleDescriptor as GFIPMAssertionDelegateServiceType;

                        if (adsService.DelegatedTokenServiceEndpoint[0].Address.Value.ToUpperInvariant() == adsEndpointAddressUrl.ToUpperInvariant())
                        {
                            KeyDescriptorType key = (from c in roleDescriptor.KeyDescriptor
                                                     where c.use == KeyTypes.signing
                                                     select c).First();

                            KeyInfoType keyInfo = key.KeyInfo;

                            X509DataType x509Data = keyInfo.Items[0] as X509DataType;

                            return new X509Certificate2(x509Data.Items[0] as byte[]);
                        }
                    }
                }
            }

            return null;
        }

        public string GetWscEntityIdFromX509Thumprint(string x509Thumbprint)
        {
            if (_gfipmEntities == null)
            {
                throw new Exception("trust fabric");
            }

            foreach (EntityDescriptorType entity in _gfipmEntities.Items)
            {
                foreach (RoleDescriptorType roleDescriptor in entity.Items)
                {
                    if (roleDescriptor is GFIPMWebServiceConsumerType)
                    {
                        KeyDescriptorType key = (from c in roleDescriptor.KeyDescriptor
                                                 where c.use == KeyTypes.signing
                                                 select c).First();

                        KeyInfoType keyInfo = key.KeyInfo;

                        X509DataType x509Data = keyInfo.Items[0] as X509DataType;

                        X509Certificate2 cert = new X509Certificate2(x509Data.Items[0] as byte[]);

                        string certThumbprint = BitConverter.ToString(cert.GetCertHash()).Replace("-", string.Empty); ;
                        if (certThumbprint.ToUpperInvariant() == x509Thumbprint.ToUpperInvariant())
                        {
                            return entity.entityID;
                        }
                    }
                }
            }

            return string.Empty;
        }

        public List<EntityAttribute> GetWscEntityAttributesFromX509Thumprint(string x509Thumbprint)
        {
            if (_gfipmEntities == null)
            {
                throw new Exception("trust fabric");
            }

            RoleDescriptorType roleDescriptor = this.GetRoleDescriptor(x509Thumbprint, typeof(GFIPMWebServiceConsumerType));

            List<EntityAttribute> entityAttributes = new List<EntityAttribute>();

            if (roleDescriptor is GFIPMWebServiceConsumerType)
            {
                // Get the TechnicalRole entity attribute value
                foreach (XmlElement xmlElem in roleDescriptor.Extensions.Any)
                {
                    EntityAttributeType entityAttrib = CtfSerializer.Deserialize<EntityAttributeType>(xmlElem.OuterXml,
                        "EntityAttribute", "http://gfipm.net/standards/metadata/2.0/entity") as EntityAttributeType;

                    EntityAttribute entityAttribute = new EntityAttribute(entityAttrib.Name, entityAttrib.EntityAttributeValue[0].ToString());

                    entityAttributes.Add(entityAttribute);
                }
            }

            return entityAttributes;
        }

        public string GetWscTechnicalRoleFromX509Thumbprint(string x509Thumbprint)
        {
            if (_gfipmEntities == null)
            {
                throw new Exception("trust fabric");
            }

            RoleDescriptorType roleDescriptor = this.GetRoleDescriptor(x509Thumbprint, typeof(GFIPMWebServiceConsumerType));

            if (roleDescriptor is GFIPMWebServiceConsumerType)
            {
                return GetEntityAttributeValue(roleDescriptor, "gfipm:2.0:entity:TechnicalRole");
            }

            return string.Empty;
        }

        public string GetWscOwnerAgencyCountryCodeFromX509Thumbprint(string x509Thumbprint)
        {
            if (_gfipmEntities == null)
            {
                throw new Exception("trust fabric");
            }

            RoleDescriptorType roleDescriptor = this.GetRoleDescriptor(x509Thumbprint, typeof(GFIPMWebServiceConsumerType));

            if (roleDescriptor is GFIPMWebServiceConsumerType)
            {
                return GetEntityAttributeValue(roleDescriptor, "gfipm:2.0:entity:OwnerAgencyCountryCode");
            }

            return string.Empty;
        }

        private string GetEntityAttributeValue(RoleDescriptorType roleDescriptor, string entityAttributeName)
        {
            // Get the TechnicalRole entity attribute value
            foreach (XmlElement xmlElem in roleDescriptor.Extensions.Any)
            {
                EntityAttributeType entityAttrib = CtfSerializer.Deserialize<EntityAttributeType>(xmlElem.OuterXml,
                    "EntityAttribute", "http://gfipm.net/standards/metadata/2.0/entity") as EntityAttributeType;

                if (entityAttrib.Name.ToUpperInvariant() == entityAttributeName.ToUpperInvariant())
                {
                    return entityAttrib.EntityAttributeValue[0].ToString();
                }
            }

            return string.Empty;
        }

        private RoleDescriptorType GetRoleDescriptor(string x509Thumbprint, Type roleDescriptorType)
        {
            foreach (EntityDescriptorType entity in _gfipmEntities.Items)
            {
                foreach (RoleDescriptorType roleDescriptor in entity.Items)
                {
                    if (roleDescriptor != null && (roleDescriptorType.FullName == roleDescriptor.GetType().FullName) )
                    {
                        KeyDescriptorType key = (from c in roleDescriptor.KeyDescriptor
                                                 where c.use == KeyTypes.signing
                                                 select c).First();

                        KeyInfoType keyInfo = key.KeyInfo;

                        X509DataType x509Data = keyInfo.Items[0] as X509DataType;

                        X509Certificate2 cert = new X509Certificate2(x509Data.Items[0] as byte[]);

                        string certThumbprint = BitConverter.ToString(cert.GetCertHash()).Replace("-", string.Empty);
                        if (certThumbprint.ToUpperInvariant() == x509Thumbprint.ToUpperInvariant())
                        {
                            return roleDescriptor;
                        }
                    }
                }
            }
            
            return null;
        }
    }
}
