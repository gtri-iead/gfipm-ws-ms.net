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

namespace IdpSts
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.Web.Configuration;

    using Microsoft.IdentityModel.Claims;
    using Microsoft.IdentityModel.Configuration;
    using Microsoft.IdentityModel.Protocols.WSTrust;
    using Microsoft.IdentityModel.SecurityTokenService;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Tokens.Saml2;
    using System.IdentityModel.Tokens;
    using System.IO;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Xml;
    using Common;


    /// <summary>
    /// A custom SecurityTokenService implementation.
    /// </summary>
    public class CustomSecurityTokenService : SecurityTokenService
    {
        // TODO: Set enableAppliesToValidation to true to enable only the RP Url's specified in the ActiveClaimsAwareApps array to get a token from this STS
        static bool enableAppliesToValidation = true;

        // TODO: Add relying party Url's that will be allowed to get token from this STS

        // <!-- Endpoint Address Configuration - ha50idpm2 -->

        static readonly string[] ActiveClaimsAwareApps = { "https://ha50wscm2:8643/Model2/Service.svc", "https://curewscm2:8181/m2wsc/services/cvc" };

        /// <summary>
        /// Creates an instance of CustomSecurityTokenService.
        /// </summary>
        /// <param name="configuration">The SecurityTokenServiceConfiguration.</param>
        public CustomSecurityTokenService(SecurityTokenServiceConfiguration configuration)
            : base(configuration)
        {
        }


        //protected override SecurityTokenDescriptor CreateSecurityTokenDescriptor(RequestSecurityToken request, Scope scope)
        //{
        //    StreamWriter file = new StreamWriter("c:\\temp\\IdentityProviderSts.CustomSecurityTokenService - CreateSecurityTokenDescriptor.txt", true);
        //    file.WriteLine("_________________________________________");
        //    file.WriteLine("DateTime: " + DateTime.Now.ToString());

        //    SecurityTokenDescriptor descriptor = null;
        //    try
        //    {
        //        descriptor = base.CreateSecurityTokenDescriptor(request, scope);

        //        if (descriptor == null)
        //        {
        //            file.WriteLine("descriptor: " + "null");
        //        }

        //        if (descriptor.Subject == null)
        //        {
        //            file.WriteLine("descriptor.Subject: " + "null");
        //        }

        //        string authType = Saml2Constants.AuthenticationContextClasses.Password.ToString();
        //        DateTime now = DateTime.UtcNow;

        //        if (string.IsNullOrEmpty(authType))
        //        {
        //            file.WriteLine("authType: " + "null");
        //        }
        //        else
        //        {
        //            file.WriteLine("authType: " + authType);
        //            file.WriteLine("now: " + now.ToString());

        //            descriptor.AddAuthenticationClaims( authType, now);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        file.WriteLine("Exception: " + e.Message);
        //        if (e.InnerException != null)
        //        {
        //            file.WriteLine("InnerException: " + e.InnerException.Message);
        //        }

        //        throw;
        //    }
        //    finally
        //    {
        //        if (file != null)
        //        {
        //            file.Close();
        //        }
        //    }
        //    return descriptor;
        //}


        /// <summary>
        /// Validates appliesTo and throws an exception if the appliesTo is null or contains an unexpected address.
        /// </summary>
        /// <param name="appliesTo">The AppliesTo value that came in the RST.</param>
        /// <exception cref="ArgumentNullException">If 'appliesTo' parameter is null.</exception>
        /// <exception cref="InvalidRequestException">If 'appliesTo' is not valid.</exception>
        void ValidateAppliesTo(EndpointAddress appliesTo)
        {
            if (appliesTo == null)
            {
                throw new ArgumentNullException("appliesTo");
            }


            if (appliesTo != null)
            {
                Type type = appliesTo.GetType();
            }

            // TODO: Enable AppliesTo validation for allowed relying party Urls by setting enableAppliesToValidation to true. By default it is false.
            if (enableAppliesToValidation)
            {
                bool validAppliesTo = false;
                foreach (string rpUrl in ActiveClaimsAwareApps)
                {
                    if (appliesTo.Uri.Equals(new Uri(rpUrl)))
                    {
                        validAppliesTo = true;
                        break;
                    }
                }

                if (!validAppliesTo)
                {
                    throw new InvalidRequestException(String.Format("The 'appliesTo' address '{0}' is not valid.", appliesTo.Uri.OriginalString));
                }
            }
        }

        /// <summary>
        /// This method returns the configuration for the token issuance request. The configuration
        /// is represented by the Scope class. In our case, we are only capable of issuing a token for a
        /// single RP identity represented by the EncryptingCertificateName.
        /// </summary>
        /// <param name="principal">The caller's principal.</param>
        /// <param name="request">The incoming RST.</param>
        /// <returns>The scope information to be used for the token issuance.</returns>
        protected override Scope GetScope(IClaimsPrincipal principal, RequestSecurityToken request)
        {
            // TEMP
            //ClaimsUtil.LogClaimsPrincipal(principal, "IdpSts.CustomSecurityTokenService.GetScope");
            CustomTextTraceSource ts = new CustomTextTraceSource("IdpSts.CustomSecurityTokenService.GetScope",
                "MyTraceSource", SourceLevels.Information);

            ValidateAppliesTo(request.AppliesTo);
            
            X509Certificate2 signingCert = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine,
                WebConfigurationManager.AppSettings["SigningCertificateName"]);

            if (signingCert != null)
            {
                ts.TraceInformation("signingCert: " + signingCert.SubjectName);
                ts.TraceInformation("\tthumbPrint: " + signingCert.Thumbprint);
            }
            else
            {
                ts.TraceInformation("signingCert: NULL");
            }

            //SecurityKeyIdentifier signingSki = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[] { 
            //        new X509SecurityToken(signingCert).CreateKeyIdentifierClause<X509SubjectKeyIdentifierClause>() });

            SecurityKeyIdentifier signingSki = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[] { 
                new X509SecurityToken(signingCert).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>() });

            X509SigningCredentials signingCredentialss = new X509SigningCredentials(signingCert, signingSki);

            Scope scope = new Scope(request.AppliesTo.Uri.OriginalString, signingCredentialss);


            //string encryptingCertificateName = "";
            string encryptingCertificateName = WebConfigurationManager.AppSettings["EncryptingCertificateName"];
            if (!string.IsNullOrEmpty(encryptingCertificateName))
            {
                // Important note on setting the encrypting credentials.
                // In a production deployment, you would need to select a certificate that is specific to the RP that is requesting the token.
                // You can examine the 'request' to obtain information to determine the certificate to use. (AppliesTo)

                // Original SV
                //scope.EncryptingCredentials = new X509EncryptingCredentials(CertificateUtil.GetCertificateByCommonName(StoreName.TrustedPeople,
                //    StoreLocation.LocalMachine, request.AppliesTo.Uri.Host));

                // New HoK
                X509Certificate2 cert = CertificateUtil.GetCertificate(StoreName.TrustedPeople,
                    StoreLocation.LocalMachine, encryptingCertificateName);
                var ski = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[] 
                    { 
                        //new X509SecurityToken(cert).CreateKeyIdentifierClause<X509SubjectKeyIdentifierClause>()
                        new X509SecurityToken(cert).CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause>()
                    }
                );
                X509EncryptingCredentials encryptingCredentials = new X509EncryptingCredentials(cert, ski);

                scope.EncryptingCredentials = encryptingCredentials;
            }
            else
            {
                // If there is no encryption certificate specified, the STS will not perform encryption.
                // This will succeed for tokens that are created without keys (BearerTokens) or asymmetric keys.  Symmetric keys are
                // required to be 'wrapped' and the STS will throw.
                scope.TokenEncryptionRequired = false;

                // Symmetric keys are required to be 'wrapped' or the STS will throw, uncomment the code below to turn off proof key encryption.
                // Turning off proof key encryption is not secure and should not be used in a deployment scenario.

                scope.SymmetricKeyEncryptionRequired = false;
            }

            //ts.TraceInformation("request.RequestType: " + request.RequestType);
            //ts.TraceInformation("request.TokenType: " + request.TokenType);
            //ts.TraceInformation("request.KeyType: " + request.KeyType);
            //ts.TraceInformation("request.KeySizeInBits: " + request.KeySizeInBits);

            //ts.TraceInformation("request.SecondaryParameters.TokenType: " + request.SecondaryParameters.TokenType);
            //ts.TraceInformation("request.SecondaryParameters.KeyType: " + request.SecondaryParameters.KeyType);

            //ts.TraceInformation("Principal Name: " + principal.Identity.Name);

            //if (request.SecondaryParameters != null)
            //{
            //    if (!string.IsNullOrEmpty(request.SecondaryParameters.TokenType))
            //    {
            //        if (string.IsNullOrEmpty(request.TokenType))
            //        {
            //            request.TokenType = request.SecondaryParameters.TokenType;
            //        }
            //    }

            //    if (!string.IsNullOrEmpty(request.SecondaryParameters.KeyType))
            //    {
            //        if (string.IsNullOrEmpty(request.KeyType))
            //        {
            //            request.TokenType = request.SecondaryParameters.KeyType;
            //        }
            //    }

            //    if (StringComparer.Ordinal.Equals(request.KeyType, "http://schemas.microsoft.com/idfx/keytype/bearer"))
            //    {
            //        if (request.KeySizeInBits.HasValue)
            //        {
            //            if (request.KeySizeInBits.Value > 0)
            //            {
            //                string errorMsg = string.Format("The request has a KeySize '{0}' that is greater than 0, which, when specified in the request is the required value for sender-vouches assertions.",
            //                    request.KeySizeInBits.Value);
            //                ts.TraceInformation(errorMsg);

            //                request.KeySizeInBits = new int?(0);
            //            }
            //        }
            //        else
            //        {
            //            // the key size for an assertion with Sender-vouches confirmation must be 0
            //            request.KeySizeInBits = new int?(0);
            //        }

            //        ts.TraceInformation("KeySizeInBits: " + request.KeySizeInBits.Value.ToString());
            //    }
            //}

            //if ((StringComparer.Ordinal.Equals(request.KeyType, "http://schemas.microsoft.com/idfx/keytype/bearer") && request.KeySizeInBits.HasValue) && (request.KeySizeInBits.Value != 0))
            //{
            //    ts.TraceInformation("Still Have problems with KeySize!!!");
            //}


            return scope;
        }


        /// <summary>
        /// This method returns the claims to be issued in the token.
        /// </summary>
        /// <param name="principal">The caller's principal.</param>
        /// <param name="request">The incoming RST, can be used to obtain addtional information.</param>
        /// <param name="scope">The scope information corresponding to this request.</param>/// 
        /// <exception cref="ArgumentNullException">If 'principal' parameter is null.</exception>
        /// <returns>The outgoing claimsIdentity to be included in the issued token.</returns>
        //protected override IClaimsIdentity GetOutputClaimsIdentity(IClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        //{
        //    if (null == principal)
        //    {
        //        throw new ArgumentNullException("principal");
        //    }

        //    // Create new identity and copy content of the caller's identity into it (including the existing delegate chain)
        //    ClaimsIdentity outputIdentity = new ClaimsIdentity();

        //    switch (principal.Identity.Name.ToUpperInvariant())
        //    {
        //        case "BOB":
        //        case "ALICE":
        //            // Appendix A Assertion Format Rules  8.b
        //            var nameIdentifierClaim = new Claim(ClaimTypes.NameIdentifier, principal.Identity.Name);
        //            nameIdentifierClaim.Properties[ClaimProperties.SamlNameIdentifierFormat] = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";

        //            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        //            TextInfo textInfo = cultureInfo.TextInfo;

        //            var emailClaim = new Claim("gfipm:2.0:user:EmailAddressText", "ms01@gfipm.net");
        //            emailClaim.Properties[ClaimProperties.SamlAttributeNameFormat] = "urn:oasis:names:tc:SAML:2.0:attrname-format:uri";

        //            #region  Gfipm Metadata based User attributes

        //            outputIdentity.Claims.AddRange(new List<Claim>
        //            {
        //                //"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        //                CreateClaim(ClaimTypes.Name, principal.Identity.Name, ClaimProperties.SamlNameIdentifierFormat, "urn:oasis:names:tc:SAML:2.0:nameid-format:transient"),

        //                CreateClaim(ClaimTypes.NameIdentifier, principal.Identity.Name, ClaimProperties.SamlNameIdentifierFormat, "urn:oasis:names:tc:SAML:2.0:nameid-format:transient"),
        //                CreateClaim("gfipm:2.0:user:SurName", principal.Identity.Name.ToLowerInvariant(), ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),
                        
        //                CreateClaim("gfipm:2.0:user:GivenName", textInfo.ToTitleCase(principal.Identity.Name), ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),
        //                CreateClaim("gfipm:2.0:user:EmailAddressText", "ms01@gfipm.net", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),

        //                CreateClaim("gfipm:2.0:user:TelephoneNumber", "404-555-9876", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),

        //                CreateClaim("gfipm:2.0:user:EmployerName", "Hawaii-5-0", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),
        //                CreateClaim("gfipm:2.0:user:FederationId", "GFIPM:IDP:ExampleIDP:USER:ms01", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),

        //                CreateClaim("gfipm:2.0:user:SwornLawEnforcementOfficerIndicator", "true", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),
                        
        //                CreateClaim("gfipm:2.0:user:IdentityProviderId", "GFIPM:IDP:HA50IDP", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),
                        
        //                CreateClaim("gfipm:2.0:user:CounterTerrorismDataSelfSearchHomePrivilegeIndicator", "true", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),
                        
        //                CreateClaim("gfipm:2.0:user:CriminalIntelligenceDataSelfSearchHomePrivilegeIndicator", "false", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),

        //                CreateClaim("gfipm:2.0:user:CitizenshipCode", "US", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),

        //                CreateClaim("gfipm:2.0:user:SecurityClearanceLevelCode", "Secret", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),
        //                CreateClaim("gfipm:2.0:user:SecurityClearanceExpirationDate", "2012-05-30", ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),
        //            });
        //            #endregion
        //            // This is one way to add the authentication statement
        //            // Alternative would be to override Saml2SecurityTokenHandler's CreateAuthenticationStatement

        //            outputIdentity.Claims.Add(new Claim(ClaimTypes.AuthenticationMethod, Saml2Constants.AuthenticationContextClasses.Password.ToString()));
        //            outputIdentity.Claims.Add(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));


        //            break;
        //    }

        //    return outputIdentity;
        //}


        // Appendix A: GFIPM-Specific SAML Assertion Format Rules
        // Sect 17. <Attribute NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:uri">

        protected override IClaimsIdentity GetOutputClaimsIdentity(IClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            if (null == principal)
            {
                throw new ArgumentNullException("principal");
            }

            
            ClaimsIdentity outputIdentity = new ClaimsIdentity();

            UserAttributeStoreMgr attribStore = new UserAttributeStoreMgr();

            List<Claim> userClaims = attribStore.GetClaims(principal.Identity.Name.ToLowerInvariant());

            outputIdentity.Claims.AddRange(userClaims);

            // This is one way to add the authentication statement
            // Alternative would be to override Saml2SecurityTokenHandler's CreateAuthenticationStatement
            outputIdentity.Claims.Add(new Claim(ClaimTypes.AuthenticationMethod, Saml2Constants.AuthenticationContextClasses.PasswordProtectedTransport.ToString()));
            outputIdentity.Claims.Add(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));

            return outputIdentity;
        }


        //Claim CreateClaim(string claimType, string claimValue, string claimProperty, string claimPropertyValue)
        //{
        //    var claim = new Claim(claimType, claimValue);
        //    claim.Properties[claimProperty] = claimPropertyValue;

        //    return claim;
        //}


        protected override void ValidateRequest(RequestSecurityToken request)
        {
            if (request == null)
            {
                throw new InvalidRequestException("The request is null");
            }

            CustomTextTraceSource ts = new CustomTextTraceSource("IdpSts.CustomSecurityTokenService.ValidateRequest",
                "MyTraceSource", SourceLevels.Information);


            ts.TraceInformation("request.RequestType: " + request.RequestType);
            ts.TraceInformation("request.TokenType: " + request.TokenType);
            ts.TraceInformation("request.KeyType: " + request.KeyType);
            ts.TraceInformation("request.KeySizeInBits: " + request.KeySizeInBits);

            if (request.SecondaryParameters != null)
            {
                ts.TraceInformation("request.SecondaryParameters.TokenType: " + request.SecondaryParameters.TokenType);
                ts.TraceInformation("request.SecondaryParameters.KeyType: " + request.SecondaryParameters.KeyType);



                // default to bearer key type
                request.TokenType = string.IsNullOrEmpty(request.TokenType) ? request.SecondaryParameters.TokenType : request.TokenType;

                // default to bearer key type
                request.KeyType = string.IsNullOrEmpty(request.KeyType) ? request.SecondaryParameters.KeyType : request.KeyType;

                if (StringComparer.Ordinal.Equals(request.KeyType, "http://schemas.microsoft.com/idfx/keytype/bearer"))
                {
                    if (request.KeySizeInBits.HasValue)
                    {
                        if (request.KeySizeInBits.Value > 0)
                        {
                            string errorMsg = string.Format("The request has a KeySize '{0}' that is greater than 0, which, when specified in the request is the required value for sender-vouches assertions.",
                                request.KeySizeInBits.Value);
                            throw new InvalidRequestException(errorMsg);
                        }
                    }
                    else
                    {
                        // the key size for an assertion with Sender-vouches confirmation must be 0
                        request.KeySizeInBits = new int?(0);
                    }

                    //ts.TraceInformation("KeySizeInBits: " + request.KeySizeInBits.Value.ToString());
                }
            }

            ts.TraceInformation("request.TokenType: " + request.TokenType);
            ts.TraceInformation("request.KeyType: " + request.KeyType);
            ts.TraceInformation("request.KeySizeInBits: " + request.KeySizeInBits);

            base.ValidateRequest(request);
        }

        #region Deprecate
        public override RequestSecurityTokenResponse Issue(IClaimsPrincipal principal, RequestSecurityToken request)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("IdpSts.CustomSecurityTokenService.Issue",
                "MyTraceSource", SourceLevels.Information);


            RequestSecurityTokenResponse rstr = base.Issue(principal, request);

            return rstr;
        }
        #endregion
    }
}
