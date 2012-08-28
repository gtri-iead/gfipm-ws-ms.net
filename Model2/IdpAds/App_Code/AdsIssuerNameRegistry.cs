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


namespace IdpAds
{
    using System;
    using System.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Tokens;
    using System.Security.Cryptography.X509Certificates;
    using System.IO;
    using System.Diagnostics;
    using Common;

    /// <summary>
    /// IssuerNameRegistry that validates the incoming token in RST.ActAs parameter.
    /// </summary>
    public class IdpAdsIssuerNameRegistry : IssuerNameRegistry
    {                        
        /// <summary>
        /// Overrides the base class. Validates the given issuer token. For a incoming SAML token
        /// the issuer token is the Certificate that signed the SAML token.
        /// </summary>
        /// <param name="securityToken">Issuer token to be validated.</param>
        /// <returns>Friendly name representing the Issuer.</returns>
        public override string GetIssuerName(SecurityToken securityToken)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("IdpAds.IdpAdsIssuerNameRegistry.GetIssuerName",
                "MyTraceSource", SourceLevels.Information);

            //TraceSource ts = new TraceSource("System.ServiceModel");

            X509SecurityToken x509Token = securityToken as X509SecurityToken;
            if (x509Token != null)
            {
                // Warning: This sample does a simple compare of the Issuer Certificate
                // to a subject name. This is not appropriate for production use. 
                // Check your validation policy and authenticate issuers based off the policy.

                string commonName = x509Token.Certificate.GetNameInfo(X509NameType.SimpleName, false);

                ts.TraceInformation("Certificate CN: " + commonName);

                // TODO: Why this is different in the 
                if (CertificateUtil.ValidateCertificate(StoreName.TrustedPeople, StoreLocation.LocalMachine, x509Token.Certificate))
                {
                    ts.TraceInformation("Certificate VALID");

                    return x509Token.Certificate.SubjectName.Name;
                }
            }

            ts.TraceInformation("Untrusted issuer");

            throw new SecurityTokenException("Untrusted issuer.");
        }
    }
}
