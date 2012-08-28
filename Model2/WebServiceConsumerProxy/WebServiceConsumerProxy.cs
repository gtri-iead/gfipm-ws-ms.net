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
using System.Diagnostics;
using System.Configuration;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Claims;
using System.Security.Cryptography.X509Certificates;

using GfipmCryptoTrustFabric;
using Common;

namespace WebServiceConsumerProxy
{
    public class WebServiceConsumerProxy<ServiceInterface>
    {
        private GfipmCryptoTrustFabric.GfipmCryptoTrustFabric _trustFabric = null;

        public WebServiceConsumerProxy()
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

        public ServiceInterface CreateChannelWithIssuedToken(SecurityToken callerToken, string appliesToUri, string endpointConfigName)
        {
            Common.CustomTextTraceSource ts = new Common.CustomTextTraceSource("WebServiceConsumerProxy.WebServiceConsumerProxy.CreateChannelWithIssuedToken",
               "MyTraceSource", SourceLevels.Information);


            SecurityToken token = GetSecurityToken(callerToken, appliesToUri);

            // Call the backend service
            ChannelFactory<ServiceInterface> factory = new ChannelFactory<ServiceInterface>(endpointConfigName);
            factory.ConfigureChannelFactory();

            // This behavior can be added in configuration. Will need a behavior conf. element class
            // This should be present during debug
            factory.Endpoint.Behaviors.Add(new CustomMessageInspectorBehavior());

            // turn off CardSpace - we already have the token
            factory.Credentials.SupportInteractive = false;
            
            ts.TraceInformation("Before factory.CreateChannelWithIssuedToken()");

            ServiceInterface serviceChannel = factory.CreateChannelWithIssuedToken<ServiceInterface>(token);

            ts.TraceInformation("After factory.CreateChannelWithIssuedToken()");

            return serviceChannel;
        }

        private SecurityToken GetSecurityToken(SecurityToken bootstrapToken, string appliesToUri)
        {
            Common.CustomTextTraceSource ts = new Common.CustomTextTraceSource("WebServiceConsumerProxy.WebServiceConsumerProxy.GetSecurityToken",
               "MyTraceSource", SourceLevels.Information);

            Saml2SecurityToken saml2Token = bootstrapToken as Saml2SecurityToken;

            ts.TraceInformation("saml2Token.Assertion.Issuer.Value = " + saml2Token.Assertion.Issuer.Value);

            // Need to create all configuration in software here, so that bindings for both Net and Metr0 should match!!!!
            // With the IDP entityID, get the ADS address and the ADS cryptographic key to create
            // an EndPoint (which needs an X509 identity)
            // GFIPM S2S 8.2.2.11
            string adsAddress = _trustFabric.GetAdsEndpointAddressFromEntityId(saml2Token.Assertion.Issuer.Value);
            ts.TraceInformation("adsAddress = " + adsAddress);

            ServiceEndpoint adsEndpoint = CreateAdsEndpoint(saml2Token.Assertion.Issuer.Value, adsAddress);


            // Create a class that subclass WSTrustChannelFactory and we can setr the certificate from the entityId of the ADS
            WSTrustChannelFactory trustChannelFactory = new WSTrustChannelFactory(adsEndpoint);

            // WSC Service Certificate
            // Shall we use configuration to manage the certificat?
            // <!-- Certificate Configuration - ha50wscm2 -->
            string wscServiceCertificateThumbprint = ConfigurationManager.AppSettings["WscServiceCertificateThumbprint"];

            trustChannelFactory.Credentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, 
                X509FindType.FindByThumbprint, wscServiceCertificateThumbprint);

            trustChannelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;
            
            trustChannelFactory.Endpoint.Contract.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;

            WSTrustChannel channel = (WSTrustChannel)trustChannelFactory.CreateChannel();

            // needed?
            channel.ChannelFactory.Endpoint.Contract.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;

            // GFIPM S2S 8.8.2.4.e
            // Endpoint WS-Addressing URL of WSP receiver
            // Do we need to get the WSP endpoint address from the CTF?
            EndpointAddress appliesToEndPoint = new EndpointAddress(ConfigurationManager.AppSettings["WspAppliesTo"]);

            RequestSecurityToken rst = new RequestSecurityToken(WSTrust13Constants.RequestTypes.Issue);

            rst.AppliesTo = appliesToEndPoint;

            rst.TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";

            // Note: When specifying the keytype, it must be a bearer type and it must be of value:
            //      "http://schemas.microsoft.com/idfx/keytype/bearer"
            // If the value WSTrust13Constants.KeyTypes.Bearer is used, an exception is thrown


            // GFIPM S2S 8.8.2.4.d
            // OnBehalfOf element containing SAML Assertion
            Microsoft.IdentityModel.Tokens.SecurityTokenElement onBehalfOfElement = new Microsoft.IdentityModel.Tokens.SecurityTokenElement(bootstrapToken);
            rst.OnBehalfOf = onBehalfOfElement;

            // GFIPM S2S 8.8.2.2 Consumer-Provider SIP conformance
            SecurityToken token = channel.Issue(rst); //, out rstr);

            return token;
        }

        private ServiceEndpoint CreateAdsEndpoint(string adsEntityId, string adsAddress)
        {
            Common.CustomTextTraceSource ts = new Common.CustomTextTraceSource("WebServiceConsumerProxy.WebServiceConsumerProxy.CreateAdsEndpoint",
              "MyTraceSource", SourceLevels.Information);

            ServiceEndpoint adsEndpoint = null;

            // Binding
            Common.AdsCustomSecuredBinding adsBindingElements = new Common.AdsCustomSecuredBinding();
            CustomBinding binding = new CustomBinding(adsBindingElements.CreateBindingElements());

            // Contract
            ContractDescription contract = ContractDescription.GetContract(typeof(Microsoft.IdentityModel.Protocols.WSTrust.IWSTrustChannelContract));

            X509Certificate2 adsCert = _trustFabric.GetAdsSigningCertificateFromEndpointAddress(adsAddress);

            ts.TraceInformation("adsAddress: " + adsAddress);
            ts.TraceInformation("Cert: " + adsCert.Subject);

            EndpointIdentity adsCertIdentity = EndpointIdentity.CreateX509CertificateIdentity(adsCert);

            adsEndpoint = new ServiceEndpoint(contract, binding, new EndpointAddress(new Uri(adsAddress), adsCertIdentity));

            return adsEndpoint;
        }
    }
}
