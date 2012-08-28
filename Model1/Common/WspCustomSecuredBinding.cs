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
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;
using System.IdentityModel.Tokens;
using System.Net;

namespace Common
{
    public class WspCustomSecuredBinding : Binding
    {
        private SecurityBindingElement _securityBindingElement = null;

        public SecurityBindingElement SecurityBindingElement
        {
            get { return _securityBindingElement; }
        }

        private ReliableSessionBindingElement _reliableSessionBindingElement = null;
        private MtomMessageEncodingBindingElement _mtomEncodingBindingElement = null;
        private HttpsTransportBindingElement _httpsTransportBindingElement = null;
        
        public WspCustomSecuredBinding()
        {
            Initialize();
        }

        private void Initialize()
        {
            _securityBindingElement = CreateSecurityBindingElement();

            _reliableSessionBindingElement = CreateReliableSessionBindingElement();

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

            return mtomEncodingBindingElement;
        }

        private TransportBindingElement CreateHttpsTransportBindingElement()
        {
            HttpsTransportBindingElement transportBindingElement = new HttpsTransportBindingElement();

            // When set to true, the IIS Site application must have the SSL require certificate set
            transportBindingElement.RequireClientCertificate = true;

            transportBindingElement.MaxBufferSize = 524288;
            transportBindingElement.MaxReceivedMessageSize = 200000000;
            transportBindingElement.MaxBufferSize = 200000000;

            return transportBindingElement;
        }


        private SecurityBindingElement CreateWSS11SecurityBindingElement()
        {
            AsymmetricSecurityBindingElement secBindingElement = new AsymmetricSecurityBindingElement();

            secBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Lax;

            secBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256Sha256;
            secBindingElement.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncrypt;
            secBindingElement.IncludeTimestamp = true;
            secBindingElement.SetKeyDerivation(false);
            secBindingElement.AllowSerializedSigningTokenOnReply = true;
            secBindingElement.RequireSignatureConfirmation = true;

            X509SecurityTokenParameters initiatorTokenParameters = new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Thumbprint,
                SecurityTokenInclusionMode.AlwaysToRecipient);
            initiatorTokenParameters.RequireDerivedKeys = false;
            initiatorTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
            secBindingElement.InitiatorTokenParameters = initiatorTokenParameters;

            X509SecurityTokenParameters recipientTokenParameters = new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Thumbprint,
                SecurityTokenInclusionMode.Never);
            recipientTokenParameters.RequireDerivedKeys = false;
            recipientTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToInitiator;
            secBindingElement.RecipientTokenParameters = recipientTokenParameters;

            secBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
            
            return secBindingElement;
        }

        private SecurityBindingElement CreateSecurityBindingElement()
        {
            return CreateWSS11SecurityBindingElement();
        }

        public override string Scheme
        {
            get { return this._httpsTransportBindingElement.Scheme; }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();

            // comment out to disable ReliableMessaging
            //elements.Add(this._reliableSessionBindingElement);

            elements.Add(this._securityBindingElement);
            elements.Add(this._mtomEncodingBindingElement);
            elements.Add(this._httpsTransportBindingElement);
            
            return elements;
        }
    }
}
