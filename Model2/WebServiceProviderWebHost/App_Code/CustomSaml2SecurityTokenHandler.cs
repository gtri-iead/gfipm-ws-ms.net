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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.Claims;
using System.IdentityModel.Selectors;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Protocols.XmlSignature;
using Microsoft.IdentityModel.SecurityTokenService;
using System.Security.Cryptography;
using System.Configuration;

using GfipmCryptoTrustFabric;
using Common;

namespace CommercialVehicleCollisionWebservice
{
    public class CustomSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
    {
        private GfipmCryptoTrustFabric.GfipmCryptoTrustFabric _trustFabric = null;

        #region CTORs
        public CustomSaml2SecurityTokenHandler()
            : base()
        {
            _trustFabric = OpenTrustFabric();
        }

        public CustomSaml2SecurityTokenHandler(SamlSecurityTokenRequirement tokenRequirement)
            : base(tokenRequirement)
        {
            _trustFabric = OpenTrustFabric();
        }


        public CustomSaml2SecurityTokenHandler(XmlNodeList nodes)
            : base(nodes)
        {
            _trustFabric = OpenTrustFabric();
        }
        #endregion


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

        string TokenToString(SecurityToken token)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xr = XmlWriter.Create(new StringWriter(stringBuilder), new XmlWriterSettings { OmitXmlDeclaration = true });
            this.WriteToken(xr, token);
            xr.Flush();

            return stringBuilder.ToString();
        }

        void SerializeTokenToStream(Saml2SecurityToken saml2Token, CustomTextTraceSource ts, string section)
        {
            // serialize the token
            ts.TraceInformation(section);

            if (saml2Token.Assertion.Issuer != null)
            {
                ts.TraceInformation("\tIssuer: " + saml2Token.Assertion.Issuer.Value);
            }
            else
            {
                ts.TraceInformation("Saml 2 Assertion Issuer: NULL");
            }

            if (saml2Token.Assertion.Issuer != null)
            {
                ts.TraceInformation("\tSigningCredentials: " + saml2Token.Assertion.SigningCredentials.ToString());
            }
            else
            {
                ts.TraceInformation("Saml 2 Assertion SigningCredentials: NULL");
            }

            if (saml2Token.Assertion.Subject != null)
            {
                if (saml2Token.Assertion.Subject.NameId != null)
                {
                    ts.TraceInformation("\tSubject NameId: " + saml2Token.Assertion.Subject.NameId.Value);
                }
                else
                {
                    ts.TraceInformation("\tSubject NameId: NULL");
                }
            }
            else
            {
                ts.TraceInformation("Saml 2 Assertion Subject: NULL");
            }

            ts.TraceInformation("Saml Token:\n");

            ts.TraceInformation(TokenToString(saml2Token));

        }

        #region Deprecate (usefule for debugging)
        // This is actually augmenting the claims in the subject. At the WSP, this should not return any claims!
        //public override ClaimsIdentityCollection ValidateToken(SecurityToken token)
        //{
        //    CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.CustomSaml2SecurityTokenHandler.ValidateToken",
        //           "MyTraceSource", SourceLevels.Information);

        //    ClaimsIdentityCollection ret = null;


        //    SerializeTokenToStream(token as Saml2SecurityToken, ts, "ValidateToken");


        //    IClaimsIdentity claimsIdentity = null;

        //    if (token == null)
        //    {
        //        throw new ArgumentNullException("token");
        //    }
            
        //    Saml2SecurityToken token2 = token as Saml2SecurityToken;
        //    if (token2 == null)
        //    {
        //        throw new ArgumentNullException("null Saml2SecurityToken");
        //    }

        //    if (base.Configuration == null)
        //    {
        //        throw new InvalidOperationException("null Configuration");
        //    }
        //    Saml2Assertion assertion = token2.Assertion;

        //    //bool shouldEnforceAudienceRestriction = this.SamlSecurityTokenRequirement.ShouldEnforceAudienceRestriction(
        //    //    base.Configuration.AudienceRestriction.AudienceMode, token2);

        //    this.ValidateConditions(assertion.Conditions,
        //        this.SamlSecurityTokenRequirement.ShouldEnforceAudienceRestriction(base.Configuration.AudienceRestriction.AudienceMode, token2));

        //    //Saml2SubjectConfirmation confirmation = assertion.Subject.SubjectConfirmations[0];

        //    // certificate that signed the assertion internally!
        //    X509SecurityToken issuerToken = token2.IssuerToken as X509SecurityToken;
        //    if (issuerToken != null)
        //    {
        //        // Here we validate the Issuer certificate
        //        this.CertificateValidator.Validate(issuerToken.Certificate);
        //    }

        //    claimsIdentity = this.CreateClaims(token2);

        //    if (base.Configuration.DetectReplayedTokens)
        //    {
        //        this.DetectReplayedTokens(token2);
        //    }

        //    // This will be needed if chaining WSP/WSC 
        //    if (base.Configuration.SaveBootstrapTokens)
        //    {
        //        claimsIdentity.BootstrapToken = token;
        //    }

        //    // Debug
        //    ClaimsIdentityCollection ids = new ClaimsIdentityCollection();
        //    ids.Add(claimsIdentity);
        //    IClaimsPrincipal p = new ClaimsPrincipal(ids);
        //    ClaimsUtil.LogClaimsPrincipal(p, "CommercialVehicleCollisionWebservice.CustomSaml2SecurityTokenHandler.ValidateToken)");
                
        //    ret = new ClaimsIdentityCollection(new IClaimsIdentity[] { claimsIdentity });

        //    return ret;
        //}
        #endregion

        protected void ValidateDelegates(DelegationRestrictionType delegation)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.CustomSaml2SecurityTokenHandler.ValidateDelegates",
               "MyTraceSource", SourceLevels.Information);

            // get all intermediaries IDs 
            List<string> wscEndityIds = new List<string>();

            foreach (DelegateType wscDelegate in delegation.Delegate)
            {
                ts.TraceInformation("Delegate NameID: " + wscDelegate.Item.ToString());

                wscEndityIds.Add(wscDelegate.Item.ToString());
            }

            // Here we verify that all the entityIDs are part of the trust fabric
            if (!_trustFabric.Contains(wscEndityIds))
            {
                throw new InvalidSecurityException("Not all of the intermediaries in the delegation chain are present in the Trust Fabric.");
            }
        }

        protected override void ValidateConditions(Saml2Conditions conditions, bool enforceAudienceRestriction)
        {
            Saml2ConditionsDelegateWrapper delegateConditions = conditions as Saml2ConditionsDelegateWrapper;
            
            ValidateDelegates(delegateConditions.Delegates);
            
            base.ValidateConditions(conditions, enforceAudienceRestriction);
        }
        
        // Pass thru the claims of the original user
        protected override IClaimsIdentity CreateClaims(Saml2SecurityToken samlToken)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.CustomSaml2SecurityTokenHandler.CreateClaims",
                "MyTraceSource", SourceLevels.Information);

            // process subject in base 
            IClaimsIdentity subject = base.CreateClaims(samlToken);

            Saml2Assertion assertion = samlToken.Assertion;

            // process Condition/Delegation 
           
            // TODO: Refactor to a function ProcessDelegation()
            ts.TraceInformation("assertion.Conditions Type: " + assertion.Conditions.GetType().Name);

            if (assertion.Conditions is Saml2ConditionsDelegateWrapper)
            {
                Saml2ConditionsDelegateWrapper delegateData = assertion.Conditions as Saml2ConditionsDelegateWrapper;

                // Iterate over the Condition delegate elements
                // Check if there are delegates within an incoming assertion
                IClaimsIdentity currentSubject = subject;

                if (delegateData != null && delegateData.Delegates != null)
                {
                    // Add the delegates in the DelegationRestrictionType
                    ts.TraceInformation("Number of Delegates: " + delegateData.Delegates.Delegate.Length);
                    for (int i = 0; i < delegateData.Delegates.Delegate.Length; i++)
                    {
                        DelegateType del = delegateData.Delegates.Delegate[i];

                        if (del != null)
                        {
                            string nameId = del.Item.ToString();

                            var claims = new List<Claim>();
                            claims.Add(new Claim(ClaimTypes.Name, nameId));
                            claims.Add(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(del.DelegationInstant, DateTimeFormats.Generated)));

                            // now add to current subject
                            currentSubject.Actor = new ClaimsIdentity(claims);

                            currentSubject = currentSubject.Actor;
                        }
                    }
                }
            }

            return subject;
        }
                       
        private Uri ReadSimpleUriElement(XmlReader reader, UriKind kind, bool allowLaxReading)
        {
            Uri uri;
            try
            {
                if (reader.IsEmptyElement)
                {
                    string errorMsg = string.Format("The given element ('{0}','{1}') is empty.", reader.LocalName, reader.NamespaceURI);
                    throw DiagnosticUtil.ThrowHelperXml(reader, errorMsg);
                }
                XmlUtil.ValidateXsiType(reader, "anyURI", "http://www.w3.org/2001/XMLSchema", false);
                reader.MoveToElement();
                string str = reader.ReadElementContentAsString();
                if (string.IsNullOrEmpty(str))
                {
                    throw DiagnosticUtil.ThrowHelperXml(reader, "The URI cannot be null or empty.");
                }
                if (!allowLaxReading && !UriUtil.CanCreateValidUri(str, kind))
                {
                    string errorMsg = string.Empty;
                    if (kind == UriKind.RelativeOrAbsolute)
                    {
                        errorMsg = "The value must be a URI.";
                    }
                    else
                    {
                        errorMsg = "The value must be an absolute URI.";
                    }
                    throw DiagnosticUtil.ThrowHelperXml(reader, errorMsg);
                }
                uri = new Uri(str, kind);
            }
            catch (Exception exception)
            {
                Exception exception2 = DiagnosticUtil.TryWrapReadException(reader, exception);
                if (exception2 == null)
                {
                    throw;
                }
                throw exception2;
            }
            return uri;
        }
               
        private DelegationRestrictionType ReadDelegates(XmlReader reader)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.CustomSaml2SecurityTokenHandler.ReadDelegates",
                "MyTraceSource", SourceLevels.Information);

            DelegationRestrictionType delegate2 = null;

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            bool requireDeclaration = false;
            if (reader.IsStartElement("Condition", "urn:oasis:names:tc:SAML:2.0:assertion"))
            {
                reader.Read();
                requireDeclaration = true;
            }
            else if (!reader.IsStartElement("Delegate", "urn:oasis:names:tc:SAML:2.0:conditions:delegation"))
            {
                reader.ReadStartElement("Delegate", "urn:oasis:names:tc:SAML:2.0:conditions:delegation");
            }

            try
            {
                XmlUtil.ValidateXsiType(reader, "DelegationRestrictionType", "urn:oasis:names:tc:SAML:2.0:conditions:delegation", requireDeclaration);

                if (reader.IsEmptyElement)
                {
                    string errorMsg = string.Format("The given element ('{0}','{1}') is empty.", reader.LocalName, reader.NamespaceURI);
                    throw DiagnosticUtil.ThrowHelperXml(reader, errorMsg);
                }

                List<DelegateType> delegates = new List<DelegateType>();

                ts.TraceInformation("Before while (reader.IsStartElement(Delegate, urn:oasis:names:tc:SAML:2.0:conditions:delegation)" );
                ts.TraceInformation("\treader: " + reader.Name);

                while (reader.IsStartElement("Delegate", "urn:oasis:names:tc:SAML:2.0:conditions:delegation"))
                {
                    ts.TraceInformation("Inside while (reader.IsStartElement(Delegate, urn:oasis:names:tc:SAML:2.0:conditions:delegation)");
                    ts.TraceInformation("\treader: " + reader.Name);

                    DelegateType delegateType = new DelegateType();

                    string attribute = reader.GetAttribute("DelegationInstant");
                    if (!string.IsNullOrEmpty(attribute))
                    {
                        delegateType.DelegationInstant = XmlConvert.ToDateTime(attribute, DateTimeFormats.Accepted);
                    }
                                        
                    reader.Read();
                    ts.TraceInformation("After reader.Read()");
                    ts.TraceInformation("\treader: " + reader.Name);

                    if (!reader.IsStartElement("NameID", "urn:oasis:names:tc:SAML:2.0:assertion"))
                    {
                        ts.TraceInformation("Inside !reader.IsStartElement(NameID, urn:oasis:names:tc:SAML:2.0:assertion)");
                        ts.TraceInformation("\treader: " + reader.Name);

                        reader.ReadStartElement("NameID", "urn:oasis:names:tc:SAML:2.0:assertion");
                    }

                    delegateType.Item = ReadSimpleUriElement(reader, UriKind.RelativeOrAbsolute, true);

                    ts.TraceInformation("Delegate NameID: " + delegateType.Item.ToString());


                    delegates.Add(delegateType);

                    reader.ReadEndElement();
                }

                DelegationRestrictionType delegate1 = new DelegationRestrictionType();
                delegate1.Delegate = delegates.ToArray();

                delegate2 = delegate1;
            }
            catch (Exception exception)
            {
                Exception exception2 = DiagnosticUtil.TryWrapReadException(reader, exception);
                if (exception2 == null)
                {
                    throw;
                }

                throw exception2;
            }

            return delegate2;
        }

        protected override Saml2Conditions ReadConditions(XmlReader reader)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.CustomSaml2SecurityTokenHandler.ReadConditions",
                  "MyTraceSource", SourceLevels.Information);

            Saml2Conditions conditions2 = null;

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (!reader.IsStartElement("Conditions", "urn:oasis:names:tc:SAML:2.0:assertion"))
            {
                reader.ReadStartElement("Conditions", "urn:oasis:names:tc:SAML:2.0:assertion");
            }
            try
            {
                Saml2ConditionsDelegateWrapper conditions = new Saml2ConditionsDelegateWrapper();
                bool isEmptyElement = reader.IsEmptyElement;

                XmlUtil.ValidateXsiType(reader, "ConditionsType", "urn:oasis:names:tc:SAML:2.0:assertion", false);
                string attribute = reader.GetAttribute("NotBefore");
                if (!string.IsNullOrEmpty(attribute))
                {
                    conditions.NotBefore = new DateTime?(XmlConvert.ToDateTime(attribute, DateTimeFormats.Accepted));
                }
                attribute = reader.GetAttribute("NotOnOrAfter");
                if (!string.IsNullOrEmpty(attribute))
                {
                    conditions.NotOnOrAfter = new DateTime?(XmlConvert.ToDateTime(attribute, DateTimeFormats.Accepted));
                }

                reader.ReadStartElement();
                if (!isEmptyElement)
                {
                    while (reader.IsStartElement())
                    {
                        if (reader.IsStartElement("Condition", "urn:oasis:names:tc:SAML:2.0:assertion"))
                        {
                            XmlQualifiedName xsiType = XmlUtil.GetXsiType(reader);

                            if ((null == xsiType) || XmlUtil.EqualsQName(xsiType, "ConditionAbstractType", "urn:oasis:names:tc:SAML:2.0:assertion"))
                            {
                                string errorMsg = string.Format("An abstract element was encountered which does not specify its concrete type. "
                                    + "Element name: '{0}' Element namespace: '{1}'", reader.LocalName, reader.NamespaceURI);
                                throw DiagnosticUtil.ThrowHelperXml(reader, errorMsg);
                            }

                            reader.ReadStartElement("Condition", "urn:oasis:names:tc:SAML:2.0:assertion");

                            conditions.Delegates = ReadDelegates(reader);

                            reader.ReadEndElement();

                            /////////////////////////////////

                            // Need to investigate why this is disabled!!!!!
                            // This looks like it is invalid here

                            //if (EqualsQName(xsiType, "AudienceRestrictionType", "urn:oasis:names:tc:SAML:2.0:assertion"))
                            //{
                            //    conditions.AudienceRestrictions.Add(this.ReadAudienceRestriction(reader));
                            //}
                            //else if (EqualsQName(xsiType, "OneTimeUseType", "urn:oasis:names:tc:SAML:2.0:assertion"))
                            //{
                            //    if (conditions.OneTimeUse)
                            //    {
                            //        throw new InvalidOperationException("ID4115: OneTimeUse");
                            //    }
                            //    ReadEmptyContentElement(reader);
                            //    conditions.OneTimeUse = true;
                            //}
                            //else
                            //{
                            //    if (!EqualsQName(xsiType, "ProxyRestrictionType", "urn:oasis:names:tc:SAML:2.0:assertion"))
                            //    {
                            //        throw new InvalidOperationException("ID4113");
                            //    }
                            //    if (conditions.ProxyRestriction != null)
                            //    {
                            //        throw new InvalidOperationException("ID4115: ProxyRestriction");
                            //    }
                            //    conditions.ProxyRestriction = this.ReadProxyRestriction(reader);
                            //}
                            ////////////////////////////////////
                        }
                        else if (reader.IsStartElement("AudienceRestriction", "urn:oasis:names:tc:SAML:2.0:assertion"))
                        {
                            XmlQualifiedName xsiType = XmlUtil.GetXsiType(reader);

                            conditions.AudienceRestrictions.Add(this.ReadAudienceRestriction(reader));
                        }
                        else
                        {
                            //if (reader.IsStartElement("OneTimeUse", "urn:oasis:names:tc:SAML:2.0:assertion"))
                            //{
                            //    if (conditions.OneTimeUse)
                            //    {
                            //        throw DiagnosticUtil.ExceptionUtil.ThrowHelperXml(reader, "A <saml:Conditions> element contained more than one OneTimeUse condition.");
                            //    }
                            //    ReadEmptyContentElement(reader);
                            //    conditions.OneTimeUse = true;
                            //    continue;
                            //}
                            if (!reader.IsStartElement("ProxyRestriction", "urn:oasis:names:tc:SAML:2.0:assertion"))
                            {
                                break;
                            }
                            if (conditions.ProxyRestriction != null)
                            {
                                throw DiagnosticUtil.ThrowHelperXml(reader, "A <saml:Conditions> element contained more than one ProxyRestriction condition.");
                            }
                            conditions.ProxyRestriction = this.ReadProxyRestriction(reader);
                        }
                    }

                    reader.ReadEndElement();
                }
                conditions2 = conditions;
            }
            catch (Exception exception)
            {
                Exception exception2 = DiagnosticUtil.TryWrapReadException(reader, exception);
                if (exception2 == null)
                {
                    throw;
                }

                throw exception2;
            }

            return conditions2;
        }

        // Looks like this is called for HOK tokens, so why it is called for the SV token?????
        // This is called by ReadToken and this will be needed in order to support other confirmation methods
        protected override ReadOnlyCollection<SecurityKey> ResolveSecurityKeys(Saml2Assertion assertion, SecurityTokenResolver resolver)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.CustomSaml2SecurityTokenHandler.ResolveSecurityKeys",
                   "MyTraceSource", SourceLevels.Information);

            // In model 2, the SAML assertion is , by design, always using the 
            // sender-vouches confirmation method so we always return an empty collection
            return new ReadOnlyCollection<SecurityKey>(new SecurityKey[0]);

            #region Modify to include Holder-of-Key and Bearer
            //Saml2Subject subject = assertion.Subject;
            //if (subject == null)
            //{
            //    throw new SecurityTokenException("null Saml2Subject");
            //}
            //if (subject.SubjectConfirmations.Count == 0)
            //{
            //    throw new SecurityTokenException("Empty SubjectConfirmations");
            //}
            //if (subject.SubjectConfirmations.Count > 1)
            //{
            //    throw new SecurityTokenException("More than 1 SubjectConfirmations");
            //}

            //Saml2SubjectConfirmation confirmation = subject.SubjectConfirmations[0];

            //// enable sender-vouches confirmation method 
            //if (Saml2Constants.ConfirmationMethods.Bearer == confirmation.Method || Saml2Constants.ConfirmationMethods.SenderVouches == confirmation.Method)
            //{
            //    if ((confirmation.SubjectConfirmationData != null) && (confirmation.SubjectConfirmationData.KeyIdentifiers.Count != 0))
            //    {
            //        throw DiagnosticUtil.ExceptionUtil.ThrowHelperError(new SecurityTokenException("A Saml2SecurityToken cannot be created from the Saml2Assertion because it specifies the Bearer confirmation method but identifies keys in the SubjectConfirmationData."));
            //    }
            //    return Common.EmptyReadOnlyCollection<SecurityKey>.Instance;
            //}
            ////if (!(Saml2Constants.ConfirmationMethods.HolderOfKey == confirmation.Method))
            ////{
            ////    throw DiagnosticUtil.ExceptionUtil.ThrowHelperError(new SecurityTokenException(SR.GetString("ID4136", new object[] { confirmation.Method })));
            ////}
            ////if ((confirmation.SubjectConfirmationData == null) || (confirmation.SubjectConfirmationData.KeyIdentifiers.Count == 0))
            ////{
            ////    throw DiagnosticUtil.ExceptionUtil.ThrowHelperError(new SecurityTokenException(SR.GetString("ID4134", new object[0])));
            ////}
            //List<SecurityKey> list = new List<SecurityKey>();

            //if (confirmation.SubjectConfirmationData != null)
            //{
            //    foreach (SecurityKeyIdentifier identifier in confirmation.SubjectConfirmationData.KeyIdentifiers)
            //    {
            //        SecurityKey key = null;
            //        foreach (SecurityKeyIdentifierClause clause in identifier)
            //        {
            //            if ((resolver != null) && resolver.TryResolveSecurityKey(clause, out key))
            //            {
            //                list.Add(key);
            //                break;
            //            }
            //        }
            //        if (key == null)
            //        {
            //            if (identifier.CanCreateKey)
            //            {
            //                key = identifier.CreateKey();
            //                list.Add(key);
            //                continue;
            //            }
            //            list.Add(new SecurityKeyElement(identifier, resolver));
            //        }
            //    }
            //}

            //return list.AsReadOnly();
            #endregion
        }
    }
}
