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
using System.Web;
using Microsoft.IdentityModel.Protocols.WSTrust;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Web.Configuration;

using Common;

namespace IdpAds
{
    public class AdsSecurityTokenServiceFactory : WSTrustServiceHostFactory
    {
        public AdsSecurityTokenServiceFactory()
        {
        }

        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            // <!-- IssuerName Configuration - ha50idpm2 -->
            string idpEntityId = WebConfigurationManager.AppSettings["IdpEntityId"];
            CustomSecurityTokenServiceConfiguration config = new CustomSecurityTokenServiceConfiguration(idpEntityId);

            // Create a security token handler collection and then provide with a SAML2 security token
            // handler and set the Audience restriction to Never
            SecurityTokenHandlerCollection onBehalfOfHandlers = new SecurityTokenHandlerCollection();
            OnBehalfOfSaml2SecurityTokenHandler onBehalfOfTokenHandler = new OnBehalfOfSaml2SecurityTokenHandler();

            onBehalfOfHandlers.Add(onBehalfOfTokenHandler);

            // Do not process the Audience in the incoming OnBehalfOf token since this token
            // is not for authenticating with the ADS
            onBehalfOfHandlers.Configuration.AudienceRestriction.AudienceMode = AudienceUriMode.Never;

            // Set the appropriate issuer name registry
            onBehalfOfHandlers.Configuration.IssuerNameRegistry = new IdpAdsIssuerNameRegistry();

            // Set the token handlers collection
            config.SecurityTokenHandlerCollectionManager[SecurityTokenHandlerCollectionManager.Usage.OnBehalfOf] = onBehalfOfHandlers;
            
            WSTrustServiceHost host = new WSTrustServiceHost(config, baseAddresses);

            host.Description.Endpoints[0].Contract.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;

            return host;
        }        
    }
}
