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
    using System.Linq;
    using System.Text;
    using System.Configuration;
    using System.Security.Principal;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.IdentityModel.Configuration;
    using Microsoft.IdentityModel.SecurityTokenService;
    using Microsoft.IdentityModel.Protocols.WSTrust;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Tokens.Saml2;
    using Microsoft.IdentityModel.Protocols.XmlSignature;
    using System.IdentityModel.Tokens;
    using System.Web.Configuration;
    using System.IO;
    using System.Diagnostics;
    using System.Xml;
    using System.ServiceModel;
    
    using GfipmCryptoTrustFabric;
    using Common;

    public class CustomSecurityTokenService : SecurityTokenService
    {
        private GfipmCryptoTrustFabric _trustFabric = null;

        static readonly string SigningCertificateName = WebConfigurationManager.AppSettings["SigningCertificateName"];

        // This needs to be configurable to allow for other Relying Parties (WSPs)
        static readonly string EncryptingCertificateName = WebConfigurationManager.AppSettings["EncryptingCertificateName"];

        public CustomSecurityTokenService(SecurityTokenServiceConfiguration configuration)
            : base(configuration)
        {
            _trustFabric = OpenTrustFabric();

            // Setup certificate the STS is going to use to sign the issued tokens
            configuration.SigningCredentials = new X509SigningCredentials(CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine, SigningCertificateName));
                        
            //configuration.SigningCredentials = new X509SigningCredentials(CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine, SigningCertificateName),
            //    "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
            //    "http://www.w3.org/2000/09/xmldsig#sha1");
        }

        private GfipmCryptoTrustFabric OpenTrustFabric()
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

            return new GfipmCryptoTrustFabric(trustFabric);
        }

        void ValidateAppliesTo(EndpointAddress appliesTo)
        {
            if (appliesTo == null || string.IsNullOrEmpty(appliesTo.Uri.ToString()))
            {
                throw new ArgumentNullException("appliesTo");
            }

            bool validAppliesTo = false;

            // GFIPM S2S 8.8.2.5.b
            string entityID = _trustFabric.GetWspEntityIdFromEndpointAddress(appliesTo.Uri.ToString());

            if (!string.IsNullOrEmpty(entityID))
            {
                validAppliesTo = true;
            }

            if (!validAppliesTo)
            {                
                throw new InvalidRequestException(String.Format("The 'appliesTo' address '{0}' is not in the GFIPM Cryptographic Trust Fabric.", 
                    appliesTo.Uri.OriginalString));
            }
        }

        protected override Scope GetScope(IClaimsPrincipal principal, RequestSecurityToken request)
        {
            // Create the scope using the request AppliesTo address, the STS signing certificate and the encryptingCredentials for the RP.
            if (request.AppliesTo == null || request.AppliesTo.Uri == null)
            {
                throw new InvalidRequestException("Cannot determine the AppliesTo address from the RequestSecurityToken.");
            }

            ValidateIssuer(request);

            ValidateAudienceRestriction(principal, request);

            ValidateAppliesTo(request.AppliesTo);

            X509Certificate2 signingCert = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine,
                WebConfigurationManager.AppSettings["SigningCertificateName"]);

            SecurityKeyIdentifier signingSki = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[] { 
                new X509SecurityToken(signingCert).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>() });

            X509SigningCredentials signingCredentialss = new X509SigningCredentials(signingCert, signingSki);

            Scope scope = new Scope(request.AppliesTo.Uri.OriginalString, signingCredentialss);

            // The assertion is not encrypted
            scope.TokenEncryptionRequired = false;

            return scope;
        }
        
        protected override IClaimsIdentity GetOutputClaimsIdentity(IClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            // Create new identity and copy content of the caller's identity into it (including the existing delegate chain)
            IClaimsIdentity outputIdentity = new ClaimsIdentity();

            if (request.OnBehalfOf != null)
            {
                // Validate Token returns this ClaimsIdentityCollection
                IClaimsIdentity onBehalfOfSubject = request.OnBehalfOf.GetSubject()[0];

                // Find the last delegate in the OnBehalfOf identity
                IClaimsIdentity lastDelegate = onBehalfOfSubject;
                while (lastDelegate.Actor != null)
                {
                    lastDelegate = lastDelegate.Actor;
                }

                // Put the caller's identity as the last delegate to the ActAs identity
                lastDelegate.Actor = GetWscClaimsIdentity(principal.Identities[0]);

                // pass-through claims to output identity
                CopyClaims(onBehalfOfSubject, outputIdentity);
            }
            
            return outputIdentity;
        }

        //protected override RequestSecurityTokenResponse GetResponse(RequestSecurityToken request, 
        //    Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor tokenDescriptor)
        //{
        //    CustomAdsTextTraceSource ts = new CustomAdsTextTraceSource("IdpAds.CustomSecurityTokenService.GetResponse",
        //        "AdsTraceSource", SourceLevels.Information);

        //    RequestSecurityTokenResponse rstr = base.GetResponse(request, tokenDescriptor);

        //    // Need to add the RequestSecurityToken's OnBehalfOf element to RequestSecurityTokenResponse
        //    SecurityTokenElement onBehalfOfElement = request.OnBehalfOf;

        //    // GFIPM S2S 8.8.2.8.d
        //    // serialize the OnBehalfOf element to the RSTR element
        //    rstr.Properties.Add("OnBehalfOf", onBehalfOfElement.SecurityTokenXml.OuterXml);
                        
        //    return rstr;
        //}

        private IClaimsIdentity GetWscClaimsIdentity(IClaimsIdentity wscIdentity)
        {
            Claim[] claims = (from c in wscIdentity.Claims
                              where c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/thumbprint"
                              select c).ToArray<Claim>();

            // must be only one
            Claim x509ThumbprintClaim = claims[0];

            // convert to a hex string since its the standard .Net way to handle x509 thumbprints
            string x509HexThumbprint = Base64Util.FromBase64ToHex(x509ThumbprintClaim.Value);

            // get the entityId from the certificate thumbprint
            // GFIPM S2S 8.8.2.6.d
            string entityID = _trustFabric.GetWscEntityIdFromX509Thumprint(x509HexThumbprint);

            IClaimsIdentity claimsIdentity = new ClaimsIdentity();

            claimsIdentity.Claims.Add(new Claim(ClaimTypes.Name, entityID));

            string now = XmlConvert.ToString(DateTime.UtcNow, DateTimeFormats.Generated);
            claimsIdentity.Claims.Add(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.UtcNow, DateTimeFormats.Generated)));

            return claimsIdentity;
        }


        private void ValidateAudienceRestriction(IClaimsPrincipal principal, RequestSecurityToken request)
        {
            SecurityToken token = request.OnBehalfOf.GetSecurityToken();

            Saml2SecurityToken saml2Token = token as Saml2SecurityToken;

            if (saml2Token != null)
            {
                // GFIPM S2S 8.8.2.5.a.i
                bool authenticatedWscIsPartOfAudience = false;
                foreach (Saml2AudienceRestriction audienceRestricition in saml2Token.Assertion.Conditions.AudienceRestrictions)
                {
                    foreach (Uri audience in audienceRestricition.Audiences)
                    {
                        string assertionAudience = audience.AbsoluteUri.ToLower();

                        // TODO: Check how the audience is set in the USER STS, and change that 
                        //       to use the WSC entityId. Then with the Principal's identity,
                        //       get the WCF certificate and then from the CTF get the ID.
                        if (assertionAudience.Contains(principal.Identity.Name.ToLower()))
                        {
                            authenticatedWscIsPartOfAudience = true;
                            break;
                        }
                    }
                }

                if (!authenticatedWscIsPartOfAudience)
                {
                    throw new InvalidOperationException("The WSC is not a member of the OnBehalfOf assertion's audience.");
                }
            }
        }

        private void ValidateIssuer(RequestSecurityToken request)
        {
            SecurityToken token = request.OnBehalfOf.GetSecurityToken();
                        
            Saml2SecurityToken saml2Token = token as Saml2SecurityToken;

            if (saml2Token != null)
            {
                // GFIPM S2S 8.8.2.5.a.ii
                if (this.SecurityTokenServiceConfiguration.TokenIssuerName.ToLower() != saml2Token.Assertion.Issuer.Value.ToLower())
                {
                    throw new InvalidOperationException("The OnBehalfOf element Issuer's name does not match the IPD of this ADS.");
                }
            }
        }

        #region Deprecate
        //public override RequestSecurityTokenResponse Issue(IClaimsPrincipal principal, RequestSecurityToken request)
        //{            
        //    CustomAdsTextTraceSource ts = new CustomAdsTextTraceSource("IdpAds.CustomSecurityTokenService.Issue",
        //        "AdsTraceSource", SourceLevels.Information);
            
        //    ts.TraceInformation("request.RequestType: " + request.RequestType);
        //    ts.TraceInformation("request.TokenType: " + request.TokenType);
        //    ts.TraceInformation("request.KeyType: " + request.KeyType);
        //    ts.TraceInformation("request.KeySizeInBits: " + request.KeySizeInBits);
            
        //    ts.TraceInformation("Principal Name: " + principal.Identity.Name);

        //    SecurityToken token = request.OnBehalfOf.GetSecurityToken();

        //    ts.TraceInformation("OnBehalfOf Token Type: " + token.GetType().ToString());

        //    Saml2SecurityToken saml2Token = token as Saml2SecurityToken;

        //    if (saml2Token != null)
        //    {
        //        ts.TraceInformation("OnBehalfOf Issuer: " + saml2Token.Assertion.Issuer.Value);

        //        // GFIPM S2S 8.8.2.5.a.i
        //        bool authenticatedWscIsPartOfAudience = false;
        //        foreach( Saml2AudienceRestriction audienceRestricition in saml2Token.Assertion.Conditions.AudienceRestrictions)
        //        {
        //            foreach( Uri audience in audienceRestricition.Audiences)
        //            {
        //                ts.TraceInformation("OnBehalfOf Audience: " + audience.AbsoluteUri);

        //                string assertionAudience = audience.AbsoluteUri.ToLower();
        //                ts.TraceInformation("OnBehalfOf Audience: " + audience.AbsoluteUri);
        //                ts.TraceInformation("Principal: " + principal.Identity.Name.ToLower());

        //                // TODO: Check how the audience is set in the USER STS, and change that 
        //                //       to use the WSC entityId. Then with the Principal's identity,
        //                //       get the WCF certificate and then from the CTF get the ID.
        //                if (assertionAudience.Contains(principal.Identity.Name.ToLower()))
        //                {
        //                    authenticatedWscIsPartOfAudience = true;
        //                    break;
        //                }
        //            }
        //        }

        //        if (!authenticatedWscIsPartOfAudience)
        //        {
        //            throw new InvalidOperationException("The WSC is not a member of the OnBehalfOf assertion's audience.");
        //        }

        //        // GFIPM S2S 8.8.2.5.a.ii
        //        if (this.SecurityTokenServiceConfiguration.TokenIssuerName.ToLower() != saml2Token.Assertion.Issuer.Value.ToLower())
        //        {
        //            throw new InvalidOperationException("The OnBehalfOf element Issuer's name does not match the IPD of this ADS.");
        //        }
        //    }

        //    RequestSecurityTokenResponse rstr = base.Issue(principal, request);
                        
        //    return rstr;
        //}
        #endregion

        protected override void ValidateRequest(RequestSecurityToken request)
        {
            if (request == null)
            {
                throw new InvalidRequestException("The request is null");
            }

            // default to bearer key type
            request.KeyType = string.IsNullOrEmpty(request.KeyType) ? "http://schemas.microsoft.com/idfx/keytype/bearer" : request.KeyType;

            if (StringComparer.Ordinal.Equals(request.KeyType, "http://schemas.microsoft.com/idfx/keytype/bearer"))
            {
                if (request.KeySizeInBits.HasValue)
                {
                    if (request.KeySizeInBits.Value > 0)
                    {
                        string errorMsg = string.Format("The request has a KeySize '{0}' that is greater than 0, which, when specified in the request is the required value for sender-vouches assertions.",
                            request.KeySizeInBits.Value);
                        throw new InvalidRequestException( errorMsg );
                    }
                }
                else
                {
                    // the key size for an assertion with Sender-vouches confirmation must be 0
                    request.KeySizeInBits = new int?(0);
                }
            }

            base.ValidateRequest(request);
        }

        /// <summary>
        /// Do a deep-copy of IClaimsIdentity except the issuer.
        /// </summary>
        /// <param name="srcIdentity">Source Identity.</param>
        /// <param name="dstIdentity">Destination Identity.</param>
        private void CopyClaims(IClaimsIdentity srcIdentity, IClaimsIdentity dstIdentity)
        {
            foreach (Claim claim in srcIdentity.Claims)
            {
                // We don't copy the issuer because it is not needed in this case. The STS always issues claims
                // using its own identity.
                Claim newClaim = new Claim(claim.ClaimType, claim.Value, claim.ValueType);

                // copy all claim properties
                foreach (string key in claim.Properties.Keys)
                {
                    newClaim.Properties.Add(key, claim.Properties[key]);
                }

                // add claim to the destination identity
                dstIdentity.Claims.Add(newClaim);
            }

            // Recursively copy claims from the source identity delegates
            if (srcIdentity.Actor != null)
            {
                dstIdentity.Actor = new ClaimsIdentity();
                CopyClaims(srcIdentity.Actor, dstIdentity.Actor);
            }
        }
    }
}
