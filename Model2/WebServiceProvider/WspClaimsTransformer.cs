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
using Microsoft.IdentityModel.Claims;
using System.Diagnostics;
using System.Configuration;

using GfipmCryptoTrustFabric;
using Common;


namespace CommercialVehicleCollisionWebServiceProvider
{
    public class WspClaimsTransformer : ClaimsAuthenticationManager
    {
        private GfipmCryptoTrustFabric.GfipmCryptoTrustFabric _trustFabric = null;

        public WspClaimsTransformer() : base()
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

        public override IClaimsPrincipal Authenticate(string resourceName, IClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal != null && incomingPrincipal.Identity.IsAuthenticated)
            {
                ClaimsIdentityCollection x509Ids = GetIdentitiesFromClaim(incomingPrincipal,
                    ClaimTypes.AuthenticationMethod, AuthenticationMethods.X509);

                foreach(IClaimsIdentity x509Identity in x509Ids)
                {
                    // this is the main identity, get the entity attributes in the Trust Fabric from the X509 thumbprint
                    string x509Thumbprint = GetClaimValue(x509Identity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/thumbprint");

                    if (!string.IsNullOrEmpty(x509Thumbprint))
                    {
                        string x509HexThumbprint = Base64Util.FromBase64ToHex(x509Thumbprint);

                        List<EntityAttribute> entityAttributes = _trustFabric.GetWscEntityAttributesFromX509Thumprint(x509HexThumbprint);

                        // now add the antity attributes to the identity
                        foreach (EntityAttribute entityAttribute in entityAttributes)
                        {
                            x509Identity.Claims.Add(new Claim(entityAttribute.AttributeType, entityAttribute.AttributeValue));
                        }
                    }
                }
            }

            return incomingPrincipal;
        }

        private string GetClaimValue(IClaimsIdentity incomingIdentity, string claimType)
        {
            string claimValue = string.Empty;
            foreach (Claim c in incomingIdentity.Claims)
            {
                if (c.ClaimType == claimType)
                {
                    claimValue = c.Value;
                    break;
                }
            }

            return claimValue;
        }

        private ClaimsIdentityCollection GetIdentitiesFromClaim(IClaimsPrincipal incomingPrincipal, string claimType, string claimValue)
        {
            ClaimsIdentityCollection ids = new ClaimsIdentityCollection();

            foreach ( IClaimsIdentity identity in incomingPrincipal.Identities )
            {
                foreach (Claim c in identity.Claims)
                {
                    if (c.ClaimType == claimType && c.Value == claimValue)
                    {
                        ids.Add(identity);
                    }
                }
            }

            return ids;
        }

        #region original
        //public override IClaimsPrincipal Authenticate(string resourceName, IClaimsPrincipal incomingPrincipal)
        //{
        //    if (incomingPrincipal != null && incomingPrincipal.Identity.IsAuthenticated)
        //    {
        //        ClaimsUtil.LogClaimsPrincipal(incomingPrincipal, "CommercialVehicleCollisionWebservice.WspClaimsTransformer.Authenticate");

        //        CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.WspClaimsTransformer.Authenticate",
        //            "MyTraceSource", SourceLevels.Information);

        //        IClaimsIdentity x509Identity = GetIdentityFromClaim(incomingPrincipal,
        //            Microsoft.IdentityModel.Claims.ClaimTypes.AuthenticationMethod, AuthenticationMethods.X509);

        //        if (x509Identity != null)
        //        {
        //            // this is the main identity, get the entity attributes in the Trust Fabric from the X509 thumbprint
        //            string x509Thumbprint = GetClaimValue(x509Identity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/thumbprint");
        //            string x509HexThumbprint = Base64Util.FromBase64ToHex(x509Thumbprint);

        //            ts.TraceInformation("x509ThumbPrint: <{0}, {1}>", x509Thumbprint, x509HexThumbprint);

        //            List<EntityAttribute> entityAttributes = _trustFabric.EntityAttributesFromSigningCertificateHexThumbprint(x509HexThumbprint);

        //            // now add the antity attributes to the identity
        //            foreach (EntityAttribute entityAttribute in entityAttributes)
        //            {
        //                x509Identity.Claims.Add(new Claim(entityAttribute.AttributeType, entityAttribute.AttributeValue));
        //            }
        //        }
        //    }

        //    return incomingPrincipal;
        //}

        //private string GetClaimValue(IClaimsIdentity incomingIdentity, string claimType)
        //{
        //    Claim[] claims = (from c in incomingIdentity.Claims
        //                      where c.ClaimType == claimType
        //                      select c).ToArray<Claim>();

        //    return claims[0].Value;
        //}

        //private IClaimsIdentity GetIdentityFromClaim(IClaimsPrincipal incomingPrincipal, string claimType, string claimValue)
        //{
        //    CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.WspClaimsTransformer.GetIdentityForClaimType",
        //        "MyTraceSource", SourceLevels.Information);

        //    ts.TraceInformation("Requesting Claim: <{0}, {1}>", claimType, claimValue);

        //    ts.TraceInformation("Incoming Principal Idenitites: {0}", incomingPrincipal.Identities.Count);

        //    foreach (IClaimsIdentity identity in incomingPrincipal.Identities)
        //    {
        //        ts.TraceInformation("Identity Name: {0}", identity.Name);
        //        ts.TraceInformation("Identity Claims: {0}", identity.Claims.Count);

        //        foreach (Claim c in identity.Claims)
        //        {
        //            ts.TraceInformation("Claim: <{0}, {1}>", c.ClaimType, c.Value);

        //            if (c.ClaimType == claimType && c.Value == claimValue)
        //            {
        //                ts.TraceInformation("Found Identity");

        //                return identity;
        //            }
        //        }
        //    }

        //    ts.TraceInformation("Identity NOT Found");

        //    return null;
        //}
        #endregion
    }
}
