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
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;

using GfipmCryptoTrustFabric;
using Common;

namespace WebServiceProviderHost
{
    public class WspAuthorizationPolicy : IAuthorizationPolicy
    {
        private string _id = string.Empty;
        private GfipmCryptoTrustFabric.GfipmCryptoTrustFabric _trustFabric = null;
        private DefaultClaimSet _issuerClaimSet = null;

        public static string IssuerName = "https://ha50wspm1:8443/Model1/WspAuthorizationPolicy/Issuer";
   
        public WspAuthorizationPolicy()
        {
            _id = Guid.NewGuid().ToString();

            _issuerClaimSet = CreateIssuer();

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

        private DefaultClaimSet CreateIssuer()
        {
            Claim issuerNameClaim = Claim.CreateNameClaim(IssuerName);
            Claim[] claims = new Claim[] { issuerNameClaim };

            return new DefaultClaimSet(claims);
        }
          
        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            bool bRet = false;

            CustomAuthState customAuthState = null;

            // If state is null, then this method has not been called before, so 
            // set up a custom state.
            if (state == null)
            {
                customAuthState = new CustomAuthState();
                state = customAuthState;
            }
            else
            {
                customAuthState = (CustomAuthState)state;
            }

            // If claims have not been added yet...
            if (!customAuthState.ClaimsAdded)
            {
                // helpful class for processing certificates
                X509CertificateClaimSet certClaimSet = null;

                // look for the client's certificate
                foreach (ClaimSet cs in evaluationContext.ClaimSets)
                {
                    certClaimSet = cs as X509CertificateClaimSet;

                    // As of this writing, calling evaluationContext.AddClaimSets
                    // directly modifies the ClaimSets collection we are enumerating,
                    // which will result in an exception, so we break out of this loop
                    // before adding our new ClaimSet
                    if (null != certClaimSet)
                    {
                        break;
                    }
                }

                if (null != certClaimSet)
                {
                    // note how we can get access to the client certificate here,
                    // so if you already know how to program certs in .NET, you're all set
                    string clientName = certClaimSet.X509Certificate.Subject;
                    string x509ThumbPrint = certClaimSet.X509Certificate.Thumbprint;

                    // map the user's name onto a set of claims that represent WSC's entity attributes
                    ClaimSet newClaimSet = LookupClaimsForWsc(clientName, x509ThumbPrint);
                    evaluationContext.AddClaimSet(this, newClaimSet);

                    // Record that claims have been added.
                    customAuthState.ClaimsAdded = true;

                    bRet = true;
                }
            }
            else
            {
                bRet = true;
            }

            return bRet;
        }

        // Look up entity attributes based on the client's certificate thumbprint
        // and map into a ClaimSet
        private ClaimSet LookupClaimsForWsc(string clientName, string x509ThumbPrint)
        {
            List<Claim> claimList = new List<Claim>();

            // preserve the client's name (subject) as an identity claim
            claimList.Add(new Claim(ClaimTypes.Name, clientName, Rights.Identity));

            List<EntityAttribute> entityAttributes =
                _trustFabric.GetWscEntityAttributesFromX509Thumprint(x509ThumbPrint);

            foreach (EntityAttribute ea in entityAttributes)
            {
                // add entity attributes as PossessProperty claims
                claimList.Add(new Claim(ea.AttributeType, ea.AttributeValue, Rights.PossessProperty));
            }
            
            return new DefaultClaimSet(Issuer, claimList);
        }

        public ClaimSet Issuer
        {
            get { return _issuerClaimSet; }
        }

        public string Id
        {
            get { return _id; }
        }

        // internal class for authentication state
        class CustomAuthState
        {
            bool bClaimsAdded;

            public CustomAuthState()
            {
                bClaimsAdded = false;
            }

            public bool ClaimsAdded
            {
                get { return bClaimsAdded; }
                set { bClaimsAdded = value; }
            }
        }
    }
}
