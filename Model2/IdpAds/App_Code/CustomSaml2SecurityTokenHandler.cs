//
// Copyright (c) 2012, Georgia Institute of Technology. All Rights Reserved.
// This code was developed by Georgia Tech Research Institute (GTRI) under
// a grant from the U.S. Dept. of Justice, Bureau of Justice Assistance.
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Protocols.XmlSignature;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.Security.Principal;

using GfipmCryptoTrustFabric;
using Common;

namespace IdpAds
{
    public class CustomSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
    {
        // have to use namespace to avoid name collisions between the Scope type
        private GfipmCryptoTrustFabric.GfipmCryptoTrustFabric _trustFabric = null;

        #region CTORs
        public CustomSaml2SecurityTokenHandler() : base()
        {
            Init();
        }

        public CustomSaml2SecurityTokenHandler(SamlSecurityTokenRequirement tokenRequirement) : base(tokenRequirement)
        {
            Init();
        }

        public CustomSaml2SecurityTokenHandler(XmlNodeList nodes) : base(nodes)
        {
            Init();
        }
        #endregion

        private void Init()
        {
            _trustFabric = OpenTrustFabric();
        }

        private GfipmCryptoTrustFabric.GfipmCryptoTrustFabric OpenTrustFabric()
        {
            string trustFabricPath = AppDomain.CurrentDomain.BaseDirectory;

            string trustFabricDcoument = ConfigurationManager.AppSettings["TrustFabricDocument"];

            if (String.IsNullOrEmpty(trustFabricDcoument))
            {
                throw new InvalidOperationException("TrustFabricDocument configuration element not present.");
            }

            string trustFabric = Path.Combine(trustFabricPath, trustFabricDcoument);

            if (!File.Exists(trustFabric))
            {
                throw new InvalidOperationException("Trust Fabric document not present.");
            }

            return new GfipmCryptoTrustFabric.GfipmCryptoTrustFabric(trustFabric);
        }

        protected override void WriteAudienceRestriction(XmlWriter writer, Saml2AudienceRestriction data)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data.Audiences == null)
            {
                throw new ArgumentNullException("audience");
            }

            // GFIPM S2S 8.8.6.a Need to limit to just one Audience
            if ( !(data.Audiences.Count == 1) )
            {
                throw new InvalidOperationException("A Saml2AudienceRestriction must specify only one Audience.");
            }

            writer.WriteStartElement("AudienceRestriction", "urn:oasis:names:tc:SAML:2.0:assertion");
            
            Uri uri = data.Audiences[0];
            writer.WriteElementString("Audience", "urn:oasis:names:tc:SAML:2.0:assertion", uri.OriginalString);

            writer.WriteEndElement();
        }

        protected override void WriteConditions(XmlWriter writer, Saml2Conditions data)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            writer.WriteStartElement("Conditions", "urn:oasis:names:tc:SAML:2.0:assertion");

            if (data.NotBefore.HasValue)
            {
                writer.WriteAttributeString("NotBefore", XmlConvert.ToString(data.NotBefore.Value.ToUniversalTime(), DateTimeFormats.Generated));
            }
            if (data.NotOnOrAfter.HasValue)
            {
                writer.WriteAttributeString("NotOnOrAfter", XmlConvert.ToString(data.NotOnOrAfter.Value.ToUniversalTime(), DateTimeFormats.Generated));
            }
            
            foreach (Saml2AudienceRestriction restriction in data.AudienceRestrictions)
            {
                this.WriteAudienceRestriction(writer, restriction);
            }

            if (data.OneTimeUse)
            {
                writer.WriteStartElement("OneTimeUse", "urn:oasis:names:tc:SAML:2.0:assertion");
                writer.WriteEndElement();
            }

            if (data.ProxyRestriction != null)
            {
                this.WriteProxyRestriction(writer, data.ProxyRestriction);
            }

            // GFIPM S2S 8.8.2.6.d Condition/Delegates
            WriteConditionDelegates(writer, data);

            writer.WriteEndElement();
        }

        private void WriteConditionDelegates(XmlWriter writer, Saml2Conditions data)
        {
            const string xsi = "http://www.w3.org/2001/XMLSchema-instance";

            writer.WriteStartElement("Condition", "urn:oasis:names:tc:SAML:2.0:assertion");
            writer.WriteAttributeString("xmlns", "del", "http://www.w3.org/2000/xmlns/", "urn:oasis:names:tc:SAML:2.0:conditions:delegation");
            writer.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", xsi);

            writer.WriteAttributeString("xsi", "type", xsi, "del:DelegationRestrictionType");

            // Check if there are delegates within an incoming assertion
            Saml2ConditionsDelegateWrapper delegateData = data as Saml2ConditionsDelegateWrapper;

            if (delegateData != null && delegateData.Delegates != null)
            {
                // Add the delegates in the DelegationRestrictionType
                foreach (DelegateType del in delegateData.Delegates.Delegate)
                {
                    writer.WriteStartElement("del", "Delegate", "urn:oasis:names:tc:SAML:2.0:conditions:delegation");
                    writer.WriteAttributeString("DelegationInstant", XmlConvert.ToString(del.DelegationInstant, DateTimeFormats.Generated));
                    writer.WriteElementString("NameID", "urn:oasis:names:tc:SAML:2.0:assertion", del.Item.ToString());

                    writer.WriteEndElement();
                }
            }
                    
            // Condition
            writer.WriteEndElement();
        }

        protected override void WriteAttribute(XmlWriter writer, Saml2Attribute data)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            // Do not serialize the actor attribute since the Condition/Delegate element has the delegate
            if ("http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor" == data.Name)
            {
                return;
            }
            
            writer.WriteStartElement("Attribute", "urn:oasis:names:tc:SAML:2.0:assertion");
            writer.WriteAttributeString("Name", data.Name);
            
            if (null != data.NameFormat)
            {
                writer.WriteAttributeString("NameFormat", data.NameFormat.AbsoluteUri);
            }

            if (data.FriendlyName != null)
            {
                writer.WriteAttributeString("FriendlyName", data.FriendlyName);
            }

            if (data.OriginalIssuer != null)
            {
                writer.WriteAttributeString("OriginalIssuer", "http://schemas.xmlsoap.org/ws/2009/09/identity/claims", data.OriginalIssuer);
            }

            string xmlSchemaNamespace = null;
            string xmlSchemaType = null;
            if (!StringComparer.Ordinal.Equals(data.AttributeValueXsiType, "http://www.w3.org/2001/XMLSchema#string"))
            {
                int index = data.AttributeValueXsiType.IndexOf('#');
                xmlSchemaNamespace = data.AttributeValueXsiType.Substring(0, index);
                xmlSchemaType = data.AttributeValueXsiType.Substring(index + 1);
            }

            foreach (string attributeValue in data.Values)
            {
                writer.WriteStartElement("AttributeValue", "urn:oasis:names:tc:SAML:2.0:assertion");
                if (attributeValue == null)
                {
                    writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", XmlConvert.ToString(true));
                }
                else if (attributeValue.Length > 0)
                {
                    if ((xmlSchemaNamespace != null) && (xmlSchemaType != null))
                    {
                        writer.WriteAttributeString("xmlns", "tn", null, xmlSchemaNamespace);
                        writer.WriteAttributeString("type", "http://www.w3.org/2001/XMLSchema-instance", "tn:" + xmlSchemaType);
                    }
                    else
                    {
                        // GFIPM S2S Appendix A: GFIPM-Specific SAML Assertion Format Rules
                        // Sect 19. <AttributeValue> attributes
                        writer.WriteAttributeString("xmlns", "xs", null, "http://www.w3.org/2001/XMLSchema");
                        writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                        writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", "xs:string");
                    }
                    this.WriteAttributeValue(writer, attributeValue, data);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private DelegateType CreateDelegate(string nameID, string delegationInstant)
        {
            DelegateType del = new DelegateType();

            del.Item = new Uri(nameID, UriKind.RelativeOrAbsolute);
            del.DelegationInstant = XmlConvert.ToDateTime(delegationInstant, XmlDateTimeSerializationMode.Utc);

            return del;
        }

        protected override Saml2Conditions CreateConditions(Microsoft.IdentityModel.Protocols.WSTrust.Lifetime tokenLifetime,
            string relyingPartyAddress, SecurityTokenDescriptor tokenDescriptor)
        {
            if (null == tokenLifetime && !string.IsNullOrEmpty(relyingPartyAddress))
            {
                return null;
            }

            Saml2ConditionsDelegateWrapper conditions = new Saml2ConditionsDelegateWrapper();

            conditions.NotBefore = tokenLifetime.Created;
            conditions.NotOnOrAfter = tokenLifetime.Expires;

            // GFIPM S2S 8.8.2.6.a
            string relyingPartyEntityID = _trustFabric.GetWspEntityIdFromEndpointAddress(relyingPartyAddress);

            // Make sure that the entity id is present in the <microsoft.identityModel><service><audienceUris> collection in the 
            // WSP configuration file
            conditions.AudienceRestrictions.Add(new Saml2AudienceRestriction(new Uri(relyingPartyEntityID, UriKind.RelativeOrAbsolute)));
            
            // check if the actors are set in the subject and create the delegates from the actors
            IClaimsIdentity currentDelegate = tokenDescriptor.Subject.Actor;

            List<DelegateType> delegates = new List<DelegateType>();

            while (currentDelegate != null)
            {
                DelegateType delegateType = CreateDelegate(currentDelegate.Claims[0].Value, currentDelegate.Claims[1].Value);

                delegates.Add(delegateType);

                // most recent delegate, synchronize token creation and delegationInstant
                if (currentDelegate.Actor == null)
                {
                    if (conditions.NotBefore.HasValue)
                    {
                        delegateType.DelegationInstant = conditions.NotBefore.Value;
                    }
                }

                currentDelegate = currentDelegate.Actor;
            }

            DelegationRestrictionType delegate1 = new DelegationRestrictionType();

            delegate1.Delegate = delegates.ToArray();

            conditions.Delegates = delegate1;

            return conditions;
        }

        protected override Saml2Subject CreateSamlSubject(SecurityTokenDescriptor tokenDescriptor)
        {
            Saml2SubjectConfirmation confirmation;
            if (tokenDescriptor == null)
            {
                throw new ArgumentNullException("tokenDescriptor");
            }
            Saml2Subject subject = new Saml2Subject();
            string name = null;
            string uriString = null;

            if ((tokenDescriptor.Subject != null) && (tokenDescriptor.Subject.Claims != null))
            {
                foreach (Claim claim in tokenDescriptor.Subject.Claims)
                {
                    if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                    {
                        if (name != null)
                        {
                            throw new InvalidOperationException(
                                "No suitable Saml2NameIdentifier could be created for the SAML2:Subject because more than one Claim of type NameIdentifier was supplied.");
                        }
                        name = claim.Value;
                        if (claim.Properties.ContainsKey("http://schemas.xmlsoap.org/ws/2005/05/identity/claimproperties/format"))
                        {
                            uriString = claim.Properties["http://schemas.xmlsoap.org/ws/2005/05/identity/claimproperties/format"];
                        }                        
                    }
                }
            }
            if (name != null)
            {
                Saml2NameIdentifier identifier = new Saml2NameIdentifier(name);

                if ((uriString != null) && UriUtil.CanCreateValidUri(uriString, UriKind.Absolute))
                {
                    identifier.Format = new Uri(uriString);
                }
                subject.NameId = identifier;
            }

            // IGNORE SAML confirmation and just create the SenderVouches

            // GFIPM S2S 8.8.2.6.c
            confirmation = new Saml2SubjectConfirmation(Saml2Constants.ConfirmationMethods.SenderVouches);

            // SAML V2.0 Condition for Delegation Restriction Version 1.0
            // 2.5 Use of Identifiers Within <saml:SubjectConfirmation>
            string lastDelegateNameId = tokenDescriptor.Subject.Actor.Name;
            confirmation.NameIdentifier = new Saml2NameIdentifier(lastDelegateNameId);

            subject.SubjectConfirmations.Add(confirmation);

            return subject;
        }

        #region These are needed if WriteAssertion has to be overriden (include WrapperSerializer.cs in the project)!
        //public SecurityKeyIdentifier ReadSigningKeyInfoHelper(XmlReader reader, Saml2Assertion assertion)
        //{
        //    return this.ReadSigningKeyInfo(reader, assertion);
        //}

        //public void WriteSigningKeyInfoHelper(XmlWriter writer, SecurityKeyIdentifier signingKeyIdentifier)
        //{
        //    this.WriteSigningKeyInfo(writer, signingKeyIdentifier);
        //}

        //public override void WriteKeyIdentifierClause(XmlWriter writer, SecurityKeyIdentifierClause securityKeyIdentifierClause)
        //{
        //    StreamWriter file = new StreamWriter("c:\\temp\\ActAsSts.CustomSaml2SecurityTokenHandler - WriteKeyIdentifierClause.txt", true);
        //    file.WriteLine("_________________________________________");
        //    file.WriteLine("DateTime: " + DateTime.Now.ToString());

        //    StackTrace stackTrace = new StackTrace();

        //    if (stackTrace.FrameCount > 0)
        //    {
        //        file.WriteLine("StackTrace:");

        //        for (int i = 0; i < stackTrace.FrameCount; i++)
        //        {
        //            file.WriteLine("\t" + stackTrace.GetFrame(i).GetMethod().Name);
        //        }
        //    }                   


        //    if (writer == null)
        //    {
        //        file.WriteLine("writer == null");
        //        throw DiagnosticUtil.ThrowHelperArgumentNull("writer");
        //    }
        //    if (securityKeyIdentifierClause == null)
        //    {
        //        file.WriteLine("securityKeyIdentifierClause == null");
        //        throw DiagnosticUtil.ThrowHelperArgumentNull("keyIdentifierClause");
        //    }

        //    try
        //    {
        //        Saml2AssertionKeyIdentifierClause wrappedClause = securityKeyIdentifierClause as Saml2AssertionKeyIdentifierClause;
        //        WrappedSaml2AssertionKeyIdentifierClause clause2 = securityKeyIdentifierClause as WrappedSaml2AssertionKeyIdentifierClause;
        //        if (clause2 != null)
        //        {
        //            file.WriteLine("clause2 != null");
        //            wrappedClause = clause2.WrappedClause;
        //        }
        //        if (wrappedClause == null)
        //        {
        //            file.WriteLine("wrappedClause == null");
        //            throw DiagnosticUtil.ThrowHelperArgument("keyIdentifierClause", SR.GetString("ID4162", new object[0]));
        //        }

        //        file.WriteLine("Saml Assertion ID:" + wrappedClause.Id);

        //        writer.WriteStartElement("SecurityTokenReference", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
        //        byte[] derivationNonce = wrappedClause.GetDerivationNonce();

        //        if (derivationNonce != null)
        //        {
        //            writer.WriteAttributeString("Nonce", "http://schemas.xmlsoap.org/ws/2005/02/sc", Convert.ToBase64String(derivationNonce));
        //            int derivationLength = wrappedClause.DerivationLength;
        //            if ((derivationLength != 0) && (derivationLength != 0x20))
        //            {
        //                throw DiagnosticUtil.ThrowHelperError(new InvalidOperationException(SR.GetString("ID4129", new object[0])));
        //            }
        //        }
        //        writer.WriteAttributeString("TokenType", "http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd", "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0");
        //        writer.WriteStartElement("KeyIdentifier", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
        //        writer.WriteAttributeString("ValueType", "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLID");
        //        writer.WriteString(wrappedClause.Id);
        //        writer.WriteEndElement();
        //        writer.WriteEndElement();
        //    }
        //    finally
        //    {
        //        if (file != null)
        //        {
        //            file.Close();
        //        }
        //    }
        //}
        #endregion

        #region Enable for .Net 4 and test wsu:Id problem. This will require the key identifier methods above
        //protected override void WriteAssertion(XmlWriter writer, Saml2Assertion data)
        //{
        //    if (writer == null)
        //    {
        //        throw DiagnosticUtil.ThrowHelperArgumentNull("writer");
        //    }
        //    if (data == null)
        //    {
        //        throw DiagnosticUtil.ThrowHelperArgumentNull("data");
        //    }
        //    XmlWriter writer2 = writer;

        //    // Encryption
        //    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //    MemoryStream stream = null;
        //    XmlDictionaryWriter writer3 = null;
        //    if ((data.EncryptingCredentials != null) && !(data.EncryptingCredentials is ReceivedEncryptingCredentials))
        //    {
        //        stream = new MemoryStream();
        //        writer = writer3 = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false);
        //    }
        //    else if ((data.ExternalEncryptedKeys == null) || (data.ExternalEncryptedKeys.Count > 0))
        //    {
        //        throw DiagnosticUtil.ThrowHelperError(new InvalidOperationException(SR.GetString("ID4173", new object[0])));
        //    }
        //    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //    if (data.CanWriteSourceData)
        //    {
        //        data.WriteSourceData(writer);
        //    }
        //    else
        //    {
        //        StreamWriter file = new StreamWriter("c:\\temp\\ActAsSts.CustomSaml2SecurityTokenHandler - WriteAssertionHelper.txt", true);
        //        file.WriteLine("_________________________________________");
        //        file.WriteLine("DateTime: " + DateTime.Now.ToString());

        //        try
        //        {
        //            EnvelopedSignatureWriter signaturewriter = null;
        //            if (data.SigningCredentials != null)
        //            {
        //                writer = signaturewriter = new EnvelopedSignatureWriter(writer, data.SigningCredentials, data.Id.Value,
        //                    new WrappedSerializer(this, data));
        //            }
        //            if (data.Subject == null)
        //            {
        //                if ((data.Statements == null) || (data.Statements.Count == 0))
        //                {
        //                    throw DiagnosticUtil.ThrowHelperError(new InvalidOperationException(SR.GetString("ID4106", new object[0])));
        //                }
        //                foreach (Saml2Statement statement in data.Statements)
        //                {
        //                    if (((statement is Saml2AuthenticationStatement) || (statement is Saml2AttributeStatement)) || (statement is Saml2AuthorizationDecisionStatement))
        //                    {
        //                        throw DiagnosticUtil.ThrowHelperError(new InvalidOperationException(SR.GetString("ID4119", new object[0])));
        //                    }
        //                }
        //            }
        //            writer.WriteStartElement("Assertion", "urn:oasis:names:tc:SAML:2.0:assertion");

        //            // This was added to support Sender-Vouches
        //            // Need to investigate if this can be added by calling base.WriteAssertion and then 
        //            // adding the atribute

        //            // This code needs to be enabled and investigated when we target again .Net 4.0. Its not needed in .Net 3.5?????
        //            //writer.WriteAttributeString("wsu", "Id",
        //            //    "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", data.Id.Value);

        //            writer.WriteAttributeString("ID", data.Id.Value);
        //            writer.WriteAttributeString("IssueInstant", XmlConvert.ToString(data.IssueInstant.ToUniversalTime(), DateTimeFormats.Generated));
        //            writer.WriteAttributeString("Version", data.Version);
        //            this.WriteIssuer(writer, data.Issuer);

        //            if (signaturewriter != null)
        //            {
        //                signaturewriter.WriteSignature();
        //            }

        //            if (data.Subject != null)
        //            {
        //                this.WriteSubject(writer, data.Subject);
        //            }
        //            if (data.Conditions != null)
        //            {
        //                this.WriteConditions(writer, data.Conditions);
        //            }
        //            if (data.Advice != null)
        //            {
        //                this.WriteAdvice(writer, data.Advice);
        //            }
        //            foreach (Saml2Statement statement2 in data.Statements)
        //            {
        //                this.WriteStatement(writer, statement2);
        //            }
        //            writer.WriteEndElement();

        //            file.WriteLine("Assertion: ");
        //            //file.WriteLine(writer.);
        //        }
        //        finally
        //        {
        //            if (file != null)
        //            {
        //                file.Close();
        //            }
        //        }
        //    }

        //    // Encryption
        //    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //    if (writer3 != null)
        //    {
        //        ((IDisposable)writer3).Dispose();
        //        writer3 = null;
        //        EncryptedDataElement element = new EncryptedDataElement();
        //        element.Type = "http://www.w3.org/2001/04/xmlenc#Element";
        //        element.Algorithm = data.EncryptingCredentials.Algorithm;
        //        element.KeyIdentifier = data.EncryptingCredentials.SecurityKeyIdentifier;
        //        SymmetricSecurityKey securityKey = data.EncryptingCredentials.SecurityKey as SymmetricSecurityKey;
        //        if (securityKey == null)
        //        {
        //            throw DiagnosticUtil.ThrowHelperError(new CryptographicException(SR.GetString("ID3064", new object[0])));
        //        }
        //        SymmetricAlgorithm symmetricAlgorithm = securityKey.GetSymmetricAlgorithm(data.EncryptingCredentials.Algorithm);
        //        element.Encrypt(symmetricAlgorithm, stream.GetBuffer(), 0, (int)stream.Length);
        //        stream.Dispose();
        //        writer2.WriteStartElement("EncryptedAssertion", "urn:oasis:names:tc:SAML:2.0:assertion");

        //        // This was added to support Sender-Vouches
        //        // Need to investigate if this can be added by calling base.WriteAssertion and then 
        //        // adding the atribute

        //        // This code needs to be enabled and investigated when we target again .Net 4.0. Its not needed in .Net 3.5?????

        //        //writer2.WriteAttributeString("wsu", "Id",
        //        //    "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", data.Id.Value);

        //        element.WriteXml(writer2, this.KeyInfoSerializer);
        //        foreach (EncryptedKeyIdentifierClause clause in data.ExternalEncryptedKeys)
        //        {
        //            this.KeyInfoSerializer.WriteKeyIdentifierClause(writer2, clause);
        //        }
        //        writer2.WriteEndElement();
        //    }
        //    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //}
        #endregion
    }
}