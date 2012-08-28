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
    public class IdpStsCustomSecuredBinding : Binding
    {
        private SecurityBindingElement _securityBindingElement = null;

        public SecurityBindingElement SecurityBindingElement
        {
            get { return _securityBindingElement; }
        }

        private TextMessageEncodingBindingElement _textEncodingBindingElement = null;
        //private HttpsTransportBindingElement _httpsTransportBindingElement = null;
        private HttpTransportBindingElement _httpTransportBindingElement = null;

        public IdpStsCustomSecuredBinding()
        {
            Initialize();
        }

        private void Initialize()
        {
            _securityBindingElement = CreateSecurityBindingElement();

            _textEncodingBindingElement = CreateTextEncodingBindingElement();

            //_httpsTransportBindingElement = CreateHttpsTransportBindingElement();
            _httpTransportBindingElement = CreateHttpTransportBindingElement();
        }

        private TextMessageEncodingBindingElement CreateTextEncodingBindingElement()
        {
            TextMessageEncodingBindingElement textEncodingBindingElement = new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8);
            
            return textEncodingBindingElement;
        }

        private HttpsTransportBindingElement CreateHttpsTransportBindingElement()
        {
            HttpsTransportBindingElement transportBindingElement = new HttpsTransportBindingElement();

            // When set to true, the IIS Site application must have the SSL require certificate set
            transportBindingElement.RequireClientCertificate = false;

            transportBindingElement.MaxBufferSize = 524288;
            transportBindingElement.MaxReceivedMessageSize = 200000000;
            transportBindingElement.MaxBufferSize = 200000000;

            return transportBindingElement;
        }

        private HttpTransportBindingElement CreateHttpTransportBindingElement()
        {
            HttpTransportBindingElement transportBindingElement = new HttpTransportBindingElement();

            // When set to true, the IIS Site application must have the SSL require certificate set
            
            transportBindingElement.MaxBufferSize = 524288;
            transportBindingElement.MaxReceivedMessageSize = 200000000;
            transportBindingElement.MaxBufferSize = 200000000;

            return transportBindingElement;
        }

        private SecurityBindingElement CreateSecurityBindingElement()
        {
            SymmetricSecurityBindingElement secBindingElement = new SymmetricSecurityBindingElement();

            secBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Strict;

            // TEST
            //secBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256Rsa15;
            secBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;

            secBindingElement.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncrypt;
            secBindingElement.IncludeTimestamp = true;
            secBindingElement.SetKeyDerivation(false);
            //secBindingElement.RequireSignatureConfirmation = true;
            //secBindingElement.AllowInsecureTransport = true;

            //////////////////////////////////////////////////////////
            SecurityBindingElement ssbe = (SecurityBindingElement)secBindingElement;

            // Set the Custom IdentityVerifier
            //ssbe.LocalClientSettings.IdentityVerifier = new Common.CustomIdentityVerifier();
            //////////////////////////////////////////////////////////


            X509SecurityTokenParameters protectTokenParameters = new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Thumbprint,
                SecurityTokenInclusionMode.Never);

            protectTokenParameters.X509ReferenceStyle = X509KeyIdentifierClauseType.Thumbprint;

            //X509SecurityTokenParameters protectTokenParameters = new X509SecurityTokenParameters(X509KeyIdentifierClauseType.IssuerSerial,
            //    SecurityTokenInclusionMode.Never);

            protectTokenParameters.RequireDerivedKeys = false;

            //protectTokenParameters.InclusionMode = SecurityTokenInclusionMode.Never;
            //protectTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;

            secBindingElement.ProtectionTokenParameters = protectTokenParameters;

            UserNameSecurityTokenParameters userNameToken = new UserNameSecurityTokenParameters();
            userNameToken.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;

            secBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Add(userNameToken);
            //secBindingElement.EndpointSupportingTokenParameters.Signed.Add(userNameToken);


            //secBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12;
            secBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;


            return secBindingElement;
        }

        public override string Scheme
        {
            //get { return this._httpsTransportBindingElement.Scheme; }
            get { return this._httpTransportBindingElement.Scheme; }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();

            elements.Add(this._securityBindingElement);
            elements.Add(this._textEncodingBindingElement);
            //elements.Add(this._httpsTransportBindingElement);
            elements.Add(this._httpTransportBindingElement);
            
            return elements;
        }
    }
}
