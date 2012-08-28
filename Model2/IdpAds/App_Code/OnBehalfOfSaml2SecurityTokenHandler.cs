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
using System.Diagnostics;
using System.Xml;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System.Globalization;
using Common;

namespace IdpAds
{

    public class OnBehalfOfSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
    {
        #region CTORs
        public OnBehalfOfSaml2SecurityTokenHandler()
            : base()
        {
        }

        public OnBehalfOfSaml2SecurityTokenHandler(SamlSecurityTokenRequirement tokenRequirement)
            : base(tokenRequirement)
        {
        }

        public OnBehalfOfSaml2SecurityTokenHandler(XmlNodeList nodes)
            : base(nodes)
        {
        }

        #endregion

        #region Token Validation

        #region Useful (Move to Common)
        //string TokenToString(SecurityToken token)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    XmlWriter xr = XmlWriter.Create(new StringWriter(stringBuilder), new XmlWriterSettings { OmitXmlDeclaration = true });
        //    this.WriteToken(xr, token);
        //    xr.Flush();

        //    return stringBuilder.ToString();
        //}

        //void SerializeTokenToStream(Saml2SecurityToken saml2Token, StreamWriter file, string section)
        //{
        //    // serialize the token
        //    file.WriteLine(section);

        //    if (saml2Token.Assertion.Issuer != null)
        //    {
        //        file.WriteLine("\tIssuer: " + saml2Token.Assertion.Issuer.Value);
        //    }
        //    else
        //    {
        //        file.WriteLine("Saml 2 Assertion Issuer: NULL");
        //    }

        //    if (saml2Token.Assertion.Issuer != null)
        //    {
        //        file.WriteLine("\tSigningCredentials: " + saml2Token.Assertion.SigningCredentials.ToString());
        //    }
        //    else
        //    {
        //        file.WriteLine("Saml 2 Assertion SigningCredentials: NULL");
        //    }

        //    if (saml2Token.Assertion.Subject != null)
        //    {
        //        if (saml2Token.Assertion.Subject.NameId != null)
        //        {
        //            file.WriteLine("\tSubject NameId: " + saml2Token.Assertion.Subject.NameId.Value);
        //        }
        //        else
        //        {
        //            file.WriteLine("\tSubject NameId: NULL");
        //        }
        //    }
        //    else
        //    {
        //        file.WriteLine("Saml 2 Assertion Subject: NULL");
        //    }

        //    file.WriteLine("Saml Token:\n");

        //    file.WriteLine(TokenToString(saml2Token));

        //}
        #endregion

        public override ClaimsIdentityCollection ValidateToken(SecurityToken token)
        {
            CustomAdsTextTraceSource ts = new CustomAdsTextTraceSource("IdpAds.OnBehalfOfSaml2SecurityTokenHandler.ValidateToken",
                "AdsTraceSource", SourceLevels.Information);

            Saml2SecurityToken saml2Token = token as Saml2SecurityToken;
            if (saml2Token != null && saml2Token.Assertion != null)
            {
                // GFIPM S2S 8.8.2.5.a.v
                Saml2AuthenticationStatement authnStatement = null;

                foreach (Saml2Statement st in saml2Token.Assertion.Statements)
                {
                    authnStatement = st as Saml2AuthenticationStatement;
                    if (authnStatement != null)
                    {
                        break;
                    }
                }

                if (authnStatement != null)
                {
                    ts.TraceInformation("Assertion Age: " + DateTime.UtcNow.Subtract(authnStatement.AuthenticationInstant).TotalSeconds.ToString() + " seconds");
                    ts.TraceInformation("Max Assertion Age:" + TimeSpan.FromSeconds(Constants.AdsMaxAssertionAge).TotalHours.ToString() + " hours");

                    if (DateTime.UtcNow.Subtract(authnStatement.AuthenticationInstant) > TimeSpan.FromSeconds(Constants.AdsMaxAssertionAge))
                    {
                        throw new SecurityTokenException("The authentication instant of the SAML2 assertion cannot be older than " 
                            + TimeSpan.FromSeconds(Constants.AdsMaxAssertionAge).TotalHours.ToString() + " hours");
                    }
                }
                else
                {
                    ts.TraceInformation("authnStatement: NULL");
                }


                // TODO: Refactor

                X509Certificate2 signingCert = Common.CertificateUtil.GetCertificate(StoreName.My, 
                    StoreLocation.LocalMachine, WebConfigurationManager.AppSettings["SigningCertificateName"]);

                // TODO: What if the KeyIdentifierClause changes?
                // How to prevent recompiling?

                SecurityKeyIdentifier secKeyIdentifier = saml2Token.Assertion.SigningCredentials.SigningKeyIdentifier;

                SecurityKeyIdentifierClause keyIdentifierClause = secKeyIdentifier.Find<X509RawDataKeyIdentifierClause>();

                if (keyIdentifierClause is X509RawDataKeyIdentifierClause)
                {
                    //X509SubjectKeyIdentifierClause x509IdentClause = keyIdentifierClause as X509SubjectKeyIdentifierClause;
                    X509RawDataKeyIdentifierClause x509IdentClause = keyIdentifierClause as X509RawDataKeyIdentifierClause;
            
                    // GFIPM S2S 8.8.2.5.a.iv
                    if (!x509IdentClause.Matches(signingCert))
                    {
                        ts.TraceInformation("The OnBehalfOf SAML 2 assertion was not signed by this IDP");

                        throw new SecurityTokenException("The OnBehalfOf SAML 2 assertion was not signed by this IDP");
                    }
                }

                // Seems this is not needed! See note below
                #region Deprecate?
                if (saml2Token.Assertion.Subject != null && saml2Token.Assertion.Subject.SubjectConfirmations != null)
                {
                    // first serialize before
                    //SerializeTokenToStream(saml2Token, file, "Saml 2 Assertion Before Removing: ");

                    ts.TraceInformation("Saml 2 Assertion: SubjectConfirmations");

                    // GFIPM S2S 8.8.2.6.b: Remove <SubjectConfirmationData> if present
                    // TODO: Need to test Holder-Of-Key confirmation method to make sure this works.
                    // TODO: This is actually not needed. When the new assertion is created, the 
                    // <SubjectConfirmationData> element is not created which is the same effect as deleting it.
                    for (int i = 0; i < saml2Token.Assertion.Subject.SubjectConfirmations.Count; i++)
                    {                        
                        saml2Token.Assertion.Subject.SubjectConfirmations[i].SubjectConfirmationData = null;
                    }
                }
                #endregion
            }
                        
            return base.ValidateToken(token);
        }

        // Handle Condition/Delegates
        protected override IClaimsIdentity CreateClaims(Saml2SecurityToken samlToken)
        {
            CustomAdsTextTraceSource ts = new CustomAdsTextTraceSource("IdpAds.OnBehalfOfSaml2SecurityTokenHandler.CreateClaims",
                "AdsTraceSource", SourceLevels.Information);

   
            // process subject in base 
            IClaimsIdentity subject = base.CreateClaims(samlToken);
            
            Saml2Assertion assertion = samlToken.Assertion;

            // process Condition/Delegation            
            ts.TraceInformation("assertion.Conditions Type: " + assertion.Conditions.GetType().Name);

            if (assertion.Conditions is Saml2ConditionsDelegateWrapper)
            {
                Saml2ConditionsDelegateWrapper delegateData = assertion.Conditions as Saml2ConditionsDelegateWrapper;

                 //iterate over the Condition delegate elements
                 //Check if there are delegates within an incoming assertion                                
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

        #endregion

        #region Delegates reader
                
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
                    if ( kind == UriKind.RelativeOrAbsolute) 
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
            CustomAdsTextTraceSource ts = new CustomAdsTextTraceSource("IdpAds.OnBehalfOfSaml2SecurityTokenHandler.ReadDelegates",
                "AdsTraceSource", SourceLevels.Information);

            //DelegationRestrictionType delegate2 = null;

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
                        
            bool isEmptyElement = reader.IsEmptyElement;
            XmlUtil.ValidateXsiType(reader, "DelegationRestrictionType", "urn:oasis:names:tc:SAML:2.0:conditions:delegation", requireDeclaration);

            if (isEmptyElement)
            {
                string errorMsg = string.Format("The given element ('{0}','{1}') is empty.", reader.LocalName, reader.NamespaceURI);

                throw DiagnosticUtil.ThrowHelperXml(reader, errorMsg);
            }

            List<DelegateType> delegates = new List<DelegateType>();

            while (reader.IsStartElement("Delegate", "urn:oasis:names:tc:SAML:2.0:conditions:delegation"))
            {
                DelegateType delegateType = new DelegateType();

                string attribute = reader.GetAttribute("DelegationInstant");

                if (!string.IsNullOrEmpty(attribute))
                {
                    delegateType.DelegationInstant = XmlConvert.ToDateTime(attribute, DateTimeFormats.Accepted);
                }

                // What does this do?
                reader.Read();

                if (!reader.IsStartElement("NameID", "urn:oasis:names:tc:SAML:2.0:assertion"))
                {
                    reader.ReadStartElement("NameID", "urn:oasis:names:tc:SAML:2.0:assertion");
                }

                // TODO: Investgate the proper way to read this
                delegateType.Item = ReadSimpleUriElement(reader, UriKind.RelativeOrAbsolute, true);
                delegates.Add(delegateType);

                reader.ReadEndElement();
            }

            DelegationRestrictionType delegateRestriction = new DelegationRestrictionType();
            delegateRestriction.Delegate = delegates.ToArray();

            //delegate2 = delegate1;
            return delegateRestriction;
        

            //return delegate2;
        }

        #region temp

        //private DelegationRestrictionType ReadDelegates(XmlReader reader)
        //{
        //    CustomAdsTextTraceSource ts = new CustomAdsTextTraceSource("IdpAds.OnBehalfOfSaml2SecurityTokenHandler.ReadDelegates",
        //        "AdsTraceSource", SourceLevels.Information);

        //    DelegationRestrictionType delegate2 = null;

        //    if (reader == null)
        //    {
        //        throw new ArgumentNullException("reader");
        //    }
        //    bool requireDeclaration = false;
        //    if (reader.IsStartElement("Condition", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //    {
        //        reader.Read();
        //        requireDeclaration = true;
        //    }
        //    else if (!reader.IsStartElement("Delegate", "urn:oasis:names:tc:SAML:2.0:conditions:delegation"))
        //    {
        //        reader.ReadStartElement("Delegate", "urn:oasis:names:tc:SAML:2.0:conditions:delegation");
        //    }

        //    try
        //    {
        //        bool isEmptyElement = reader.IsEmptyElement;
        //        XmlUtil.ValidateXsiType(reader, "DelegationRestrictionType", "urn:oasis:names:tc:SAML:2.0:conditions:delegation", requireDeclaration);

        //        if (isEmptyElement)
        //        {
        //            string errorMsg = string.Format("The given element ('{0}','{1}') is empty.", reader.LocalName, reader.NamespaceURI);

        //            throw DiagnosticUtil.ThrowHelperXml(reader, errorMsg);
        //        }

        //        List<DelegateType> delegates = new List<DelegateType>();

        //        while (reader.IsStartElement("Delegate", "urn:oasis:names:tc:SAML:2.0:conditions:delegation"))
        //        {
        //            DelegateType delegateType = new DelegateType();

        //            string attribute = reader.GetAttribute("DelegationInstant");

        //            if (!string.IsNullOrEmpty(attribute))
        //            {
        //                delegateType.DelegationInstant = XmlConvert.ToDateTime(attribute, DateTimeFormats.Accepted);
        //            }

        //            // What does this do?
        //            reader.Read();

        //            if (!reader.IsStartElement("NameID", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //            {
        //                reader.ReadStartElement("NameID", "urn:oasis:names:tc:SAML:2.0:assertion");
        //            }

        //            // TODO: Investgate the proper way to read this
        //            delegateType.Item = ReadSimpleUriElement(reader, UriKind.RelativeOrAbsolute, true);
        //            delegates.Add(delegateType);

        //            reader.ReadEndElement();
        //        }

        //        DelegationRestrictionType delegate1 = new DelegationRestrictionType();
        //        delegate1.Delegate = delegates.ToArray();

        //        delegate2 = delegate1;
        //    }
        //    catch (Exception exception)
        //    {
        //        Exception exception2 = DiagnosticUtil.TryWrapReadException(reader, exception);
        //        if (exception2 == null)
        //        {
        //            throw;
        //        }

        //        throw exception2;
        //    }

        //    return delegate2;
        //}

        //protected override Saml2Conditions ReadConditions(XmlReader reader)
        //{
        //    CustomAdsTextTraceSource ts = new CustomAdsTextTraceSource("IdpAds.OnBehalfOfSaml2SecurityTokenHandler.ReadConditions",
        //        "AdsTraceSource", SourceLevels.Information);

        //    //Saml2Conditions conditions2 = null;

        //    if (reader == null)
        //    {
        //        throw new ArgumentNullException("reader");
        //    }
        //    if (!reader.IsStartElement("Conditions", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //    {
        //        reader.ReadStartElement("Conditions", "urn:oasis:names:tc:SAML:2.0:assertion");
        //    }
            
        //    // TODO: Seems this needs to be declared below at "conditions.Delegates = ReadDelegates(reader);"
        //    // Let's assume that from now on, Saml2ConditionsDelegateWrapper is the right type and test for the 
        //    // existence of Condition/Delegate elements!
        //    Saml2ConditionsDelegateWrapper conditions = new Saml2ConditionsDelegateWrapper();
        //    bool isEmptyElement = reader.IsEmptyElement;

        //    XmlUtil.ValidateXsiType(reader, "ConditionsType", "urn:oasis:names:tc:SAML:2.0:assertion", false);
        //    string attribute = reader.GetAttribute("NotBefore");
        //    if (!string.IsNullOrEmpty(attribute))
        //    {
        //        conditions.NotBefore = new DateTime?(XmlConvert.ToDateTime(attribute, DateTimeFormats.Accepted));
        //    }
        //    attribute = reader.GetAttribute("NotOnOrAfter");
        //    if (!string.IsNullOrEmpty(attribute))
        //    {
        //        conditions.NotOnOrAfter = new DateTime?(XmlConvert.ToDateTime(attribute, DateTimeFormats.Accepted));
        //    }

        //    reader.ReadStartElement();
        //    if (!isEmptyElement)
        //    {
        //        while (reader.IsStartElement())
        //        {
        //            if (reader.IsStartElement("Condition", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //            {
        //                XmlQualifiedName xsiType = XmlUtil.GetXsiType(reader);

        //                if ((null == xsiType) || XmlUtil.EqualsQName(xsiType, "ConditionAbstractType", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //                {
        //                    string errorMsg = string.Format("An abstract element was encountered which does not specify its concrete type. " +
        //                        "Element name: '{0}' Element namespace: '{1}'", reader.LocalName, reader.NamespaceURI);
        //                    throw DiagnosticUtil.ThrowHelperXml(reader, errorMsg);
        //                }

        //                reader.ReadStartElement("Condition", "urn:oasis:names:tc:SAML:2.0:assertion");

        //                // TODO: VALIDATION!!!
        //                // Seems that Validation is done in ValidateToken.VsalidateConditions above
        //                conditions.Delegates = ReadDelegates(reader);

        //                reader.ReadEndElement();

        //                /////////////////////////////////

        //                // Need to investigate why this is disabled!!!!!
        //                // This looks like it is invalid here

        //                //if (EqualsQName(xsiType, "AudienceRestrictionType", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //                //{
        //                //    conditions.AudienceRestrictions.Add(this.ReadAudienceRestriction(reader));
        //                //}
        //                //else if (EqualsQName(xsiType, "OneTimeUseType", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //                //{
        //                //    if (conditions.OneTimeUse)
        //                //    {
        //                //        throw new InvalidOperationException("ID4115: OneTimeUse");
        //                //    }
        //                //    ReadEmptyContentElement(reader);
        //                //    conditions.OneTimeUse = true;
        //                //}
        //                //else
        //                //{
        //                //    if (!EqualsQName(xsiType, "ProxyRestrictionType", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //                //    {
        //                //        throw new InvalidOperationException("ID4113");
        //                //    }
        //                //    if (conditions.ProxyRestriction != null)
        //                //    {
        //                //        throw new InvalidOperationException("ID4115: ProxyRestriction");
        //                //    }
        //                //    conditions.ProxyRestriction = this.ReadProxyRestriction(reader);
        //                //}
        //                ////////////////////////////////////
        //            }
        //            else if (reader.IsStartElement("AudienceRestriction", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //            {
        //                XmlQualifiedName xsiType = XmlUtil.GetXsiType(reader);

        //                conditions.AudienceRestrictions.Add(this.ReadAudienceRestriction(reader));
        //            }
        //            else
        //            {
        //                //if (reader.IsStartElement("OneTimeUse", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //                //{
        //                //    if (conditions.OneTimeUse)
        //                //    {
        //                //        throw DiagnosticUtil.ThrowHelperXml(reader, SR.GetString("ID4115", new object[] { "OneTimeUse" }));
        //                //    }
        //                //    ReadEmptyContentElement(reader);
        //                //    conditions.OneTimeUse = true;
        //                //    continue;
        //                //}
        //                if (!reader.IsStartElement("ProxyRestriction", "urn:oasis:names:tc:SAML:2.0:assertion"))
        //                {
        //                    break;
        //                }
        //                if (conditions.ProxyRestriction != null)
        //                {
        //                    throw DiagnosticUtil.ThrowHelperXml(reader,
        //                        "A <saml:Conditions> element contained more than one ProxyRestriction condition.");
        //                }
        //                conditions.ProxyRestriction = this.ReadProxyRestriction(reader);
        //            }
        //        }

        //        reader.ReadEndElement();
        //    }

        //    return conditions as Saml2Conditions;
        //}
        #endregion

        protected override Saml2Conditions ReadConditions(XmlReader reader)
        {
            CustomAdsTextTraceSource ts = new CustomAdsTextTraceSource("IdpAds.OnBehalfOfSaml2SecurityTokenHandler.ReadConditions",
                "AdsTraceSource", SourceLevels.Information);

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
                // TODO: Seems this needs to be declared below at "conditions.Delegates = ReadDelegates(reader);"
                // Let's assume that from now on, Saml2ConditionsDelegateWrapper is the right type and test for the 
                // existence of Condition/Delegate elements!
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
                                string errorMsg = string.Format("An abstract element was encountered which does not specify its concrete type. " +
                                    "Element name: '{0}' Element namespace: '{1}'", reader.LocalName, reader.NamespaceURI);
                                throw DiagnosticUtil.ThrowHelperXml(reader, errorMsg);
                            }

                            reader.ReadStartElement("Condition", "urn:oasis:names:tc:SAML:2.0:assertion");

                            // TODO: VALIDATION!!!
                            // Seems that Validation is done in ValidateToken.VsalidateConditions above
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
                            //        throw DiagnosticUtil.ThrowHelperXml(reader, SR.GetString("ID4115", new object[] { "OneTimeUse" }));
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
                                throw DiagnosticUtil.ThrowHelperXml(reader,
                                    "A <saml:Conditions> element contained more than one ProxyRestriction condition.");
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

        #endregion

  
        // TODO: The WSP and ADS incoming SAML token handlers share code. Extract base class
        protected override ReadOnlyCollection<SecurityKey> ResolveSecurityKeys(Saml2Assertion assertion, SecurityTokenResolver resolver)
        {
            Saml2Subject subject = assertion.Subject;

            Saml2SubjectConfirmation confirmation = subject.SubjectConfirmations[0];

            if (Saml2Constants.ConfirmationMethods.SenderVouches == confirmation.Method)
            {
                // empty ReadOnlyCollection
                return new ReadOnlyCollection<SecurityKey>(new SecurityKey[0]);
            }

            // let the base class handle assertion with the bearer or 
            // holder-of-key confirmation methods
            return base.ResolveSecurityKeys(assertion, resolver);
        }
    }
}
