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
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;
using System.IdentityModel.Tokens;

namespace Common
{
    public class WspCustomSecuredBinding : Binding
    {
        private bool _enableStrTransform = false;
        private SecurityBindingElement _securityBindingElement = null;

        public SecurityBindingElement SecurityBindingElement
        {
            get { return _securityBindingElement; }
        }

        private ReliableSessionBindingElement _reliableSessionBindingElement = null;
        private MtomMessageEncodingBindingElement _mtomEncodingBindingElement = null;
        private HttpsTransportBindingElement _httpsTransportBindingElement = null;
        //private HttpTransportBindingElement _httpTransportBindingElement = null;

        public WspCustomSecuredBinding( bool enableStrTransform)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("Common.WspCustomSecuredBinding.WspCustomSecuredBinding",
               "MyTraceSource", System.Diagnostics.SourceLevels.Information);

            ts.TraceInformation("enableStrTransform = " + enableStrTransform.ToString());

            this._enableStrTransform = enableStrTransform;

            Initialize();
        }

        private void Initialize()
        {
            _securityBindingElement = CreateSecurityBindingElement();
            //_securityBindingElement = CreateTransportSecurityBindingElement();

            _reliableSessionBindingElement = CreateReliableSessionBindingElement();

            //binding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8));

            _mtomEncodingBindingElement = CreateMtomEncodingBindingElement();

            _httpsTransportBindingElement = CreateHttpsTransportBindingElement() as HttpsTransportBindingElement;
        }

        private ReliableSessionBindingElement CreateReliableSessionBindingElement()
        {
            // create and configure reliable session
            ReliableSessionBindingElement reliableSession = new ReliableSessionBindingElement();
            reliableSession.Ordered = true;
            reliableSession.ReliableMessagingVersion = ReliableMessagingVersion.WSReliableMessaging11;

            return reliableSession;
        }


        private MtomMessageEncodingBindingElement CreateMtomEncodingBindingElement()
        {
            MtomMessageEncodingBindingElement mtomEncodingBindingElement = new MtomMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8);
            //mtomEncodingBindingElement.MaxBufferSize = 20000000;
            //mtomEncodingBindingElement.MaxReadPoolSize = 20000000;
            //mtomEncodingBindingElement.MaxWritePoolSize = 20000000;

            //mtomEncodingBindingElement.ReaderQuotas.MaxStringContentLength = 200000000;
            //mtomEncodingBindingElement.ReaderQuotas.MaxArrayLength = 200000000;
            //mtomEncodingBindingElement.ReaderQuotas.MaxBytesPerRead = 200000000;
            //mtomEncodingBindingElement.ReaderQuotas.MaxDepth = 200000000;
            //mtomEncodingBindingElement.ReaderQuotas.MaxNameTableCharCount = 200000000;

            return mtomEncodingBindingElement;
        }

        private TransportBindingElement CreateHttpsTransportBindingElement()
        {
            HttpsTransportBindingElement transportBindingElement = new HttpsTransportBindingElement();

            // When set to true, the IIS Site application must have the SSL require certificate set
            transportBindingElement.RequireClientCertificate = false;

            transportBindingElement.MaxBufferSize = 524288;
            transportBindingElement.MaxReceivedMessageSize = 200000000;
            transportBindingElement.MaxBufferSize = 200000000;

            return transportBindingElement;
        }

        private TransportBindingElement CreateHttpTransportBindingElement()
        {
            HttpTransportBindingElement transportBindingElement = new HttpTransportBindingElement();

            transportBindingElement.MaxBufferSize = 524288;
            transportBindingElement.MaxReceivedMessageSize = 200000000;
            transportBindingElement.MaxBufferSize = 200000000;

            return transportBindingElement;
        }
               
        private SecurityBindingElement CreateSecurityBindingElement()
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("Common.WspCustomSecuredBinding.CreateSecurityBindingElement",
                "MyTraceSource", System.Diagnostics.SourceLevels.Information);

            AsymmetricSecurityBindingElement secBindingElement = new AsymmetricSecurityBindingElement();

            secBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Lax;

            //secBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            secBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256Sha256;

            secBindingElement.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncrypt;
            secBindingElement.IncludeTimestamp = true;
            secBindingElement.SetKeyDerivation(false);
            secBindingElement.AllowSerializedSigningTokenOnReply = true;

            secBindingElement.RequireSignatureConfirmation = true;

            // SAML assertion as a signed-encrypted supporting token
            IssuedSecurityTokenParameters issuedTokenParameters =
                new IssuedSecurityTokenParameters("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0");

            // Compliance with WSS SAML Token Profile 1.1
            // Target .Net 3.5. Does not work with .Net 4
            issuedTokenParameters.UseStrTransform = _enableStrTransform;
            ts.TraceInformation("issuedTokenParameters.UseStrTransform = " + issuedTokenParameters.UseStrTransform.ToString());

            // Using bearer key type which means no proof key
            issuedTokenParameters.KeyType = SecurityKeyType.BearerKey;
            issuedTokenParameters.KeySize = 0;


            issuedTokenParameters.RequireDerivedKeys = false;
            issuedTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
            issuedTokenParameters.ReferenceStyle = SecurityTokenReferenceStyle.Internal;
            //issuedTokenParameters.ReferenceStyle = SecurityTokenReferenceStyle.External;

            // These claims are not really needed here. We are doing out of band requests!
            // Claims
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:SurName"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:GivenName"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:EmailAddressText"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:TelephoneNumber"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:FederationId"));


            // GFIPM S2S 6.4 User Authorization - Encrypted GFIPM User Assertion
            //secBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Add(issuedTokenParameters);

            // For debug
            secBindingElement.EndpointSupportingTokenParameters.Signed.Add(issuedTokenParameters);

            X509KeyIdentifierClauseType keyIdClauseType = X509KeyIdentifierClauseType.Thumbprint;

            X509SecurityTokenParameters initiatorTokenParameters = new X509SecurityTokenParameters(keyIdClauseType,
                SecurityTokenInclusionMode.AlwaysToRecipient);
            initiatorTokenParameters.RequireDerivedKeys = false;
            initiatorTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;

            //initiatorTokenParameters.ReferenceStyle = (SecurityTokenReferenceStyle)X509KeyIdentifierClauseType.RawDataKeyIdentifier;
            secBindingElement.InitiatorTokenParameters = initiatorTokenParameters;

            X509SecurityTokenParameters recipientTokenParameters = new X509SecurityTokenParameters(keyIdClauseType,
                SecurityTokenInclusionMode.AlwaysToInitiator);
            recipientTokenParameters.RequireDerivedKeys = false;
            recipientTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToInitiator;
            //recipientTokenParameters.ReferenceStyle = SecurityTokenReferenceStyle.External;
            secBindingElement.RecipientTokenParameters = recipientTokenParameters;

            secBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;

            //Set the Custom IdentityVerifier
            secBindingElement.LocalClientSettings.IdentityVerifier = new Common.CustomIdentityVerifier();
            //////////////////////////////////////////////////////////
            
            return secBindingElement;
        }

        private SecurityBindingElement CreateTransportSecurityBindingElement()
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("Common.WspCustomSecuredBinding.CreateSecurityBindingElement",
                "MyTraceSource", System.Diagnostics.SourceLevels.Information);


            // SAML assertion as a signed-encrypted supporting token
            IssuedSecurityTokenParameters issuedTokenParameters =
                new IssuedSecurityTokenParameters("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0");

          
            // Using bearer key type which means no proof key
            issuedTokenParameters.KeyType = SecurityKeyType.BearerKey;
            issuedTokenParameters.KeySize = 0;


            issuedTokenParameters.RequireDerivedKeys = false;
            issuedTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
            issuedTokenParameters.ReferenceStyle = SecurityTokenReferenceStyle.Internal;
            //issuedTokenParameters.ReferenceStyle = SecurityTokenReferenceStyle.External;

            // These claims are not really needed here. We are doing out of band requests!
            // Claims
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:SurName"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:GivenName"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:EmailAddressText"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:TelephoneNumber"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:FederationId"));


            // GFIPM S2S 6.4 User Authorization - Encrypted GFIPM User Assertion
            //secBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Add(issuedTokenParameters);

            // For debug
            //secBindingElement.EndpointSupportingTokenParameters.Signed.Add(issuedTokenParameters);


            TransportSecurityBindingElement transportSecurityBinding = SecurityBindingElement.CreateIssuedTokenOverTransportBindingElement(issuedTokenParameters);

            transportSecurityBinding.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;

            transportSecurityBinding.SecurityHeaderLayout = SecurityHeaderLayout.Lax;

            transportSecurityBinding.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;

            transportSecurityBinding.IncludeTimestamp = true;
            transportSecurityBinding.SetKeyDerivation(false);

            //transportSecurityBinding.RequireSignatureConfirmation = true;

            return transportSecurityBinding;
        }

        public override string Scheme
        {
            get { return this._httpsTransportBindingElement.Scheme; }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();

            // comment out to disable ReliableMessaging
            elements.Add(this._reliableSessionBindingElement);


            elements.Add(this._securityBindingElement);
            elements.Add(this._mtomEncodingBindingElement);
            //elements.Add(new CustomTextMessageEncoder.CustomTextMessageBindingElement("UTF-8", "text/xml", MessageVersion.Soap11WSAddressing10));

            elements.Add(this._httpsTransportBindingElement);
           
            return elements;
        }
    }
}
