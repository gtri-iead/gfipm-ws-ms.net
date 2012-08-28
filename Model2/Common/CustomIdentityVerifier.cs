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
using System.ServiceModel;
using System.ServiceModel.Security;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Common
{
    // TODO: Specialize this class to look for X509Certificate Identity verification and use it in all WSC's
    // Create another one for the client to IDP STS connection
    public class CustomIdentityVerifier : IdentityVerifier
    {
        // Code to be added.
        public override bool CheckAccess(EndpointIdentity identity, AuthorizationContext authContext)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("Common.CustomIdentityVerifier.CheckAccess",
                "MyTraceSource", SourceLevels.Information);

            bool returnvalue = false;

            try
            {
                foreach (ClaimSet claimset in authContext.ClaimSets)
                {
                    foreach (Claim claim in claimset)
                    {
                        ts.TraceInformation("claim.ClaimType: " + claim.ClaimType);
                        ts.TraceInformation("\tclaim.Right: " + claim.Right);
                        ts.TraceInformation("\t\tclaim.Resource: " + claim.Resource.ToString());
                                            
                        if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/x500distinguishedname")
                        {
                            X500DistinguishedName name = (X500DistinguishedName)claim.Resource;
                            ts.TraceInformation("X500DistinguishedName: " + name.Name);
                        
                            // TODO:  The identity of the CURE Idp will fail here; is it valid to check the this 
                            // certificate against the TruestedPeople store???
                            
                            //if ("E=gfipm-support@lists.gatech.edu, CN=HA50WSP, O=Hawaii Five0, L=Honolulu, S=Hawaii, C=US" == name.Name ||
                            //    "E=ha50@idp.net, CN=HA50IDP, O=Hawaii Five0, L=Dallas, S=GA, C=US" == name.Name)
                            //{
                            //    ts.TraceInformation("\tClaim Type: {0}", claim.ClaimType);
                            //    ts.TraceInformation("\tRight: {0}", claim.Right);
                            //    ts.TraceInformation("\tResource: {0}", claim.Resource);
                            //    returnvalue = true;
                            //}
                        }

                        if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/thumbprint")
                        {
                            //ts.TraceInformation("Thumbprint: " + claim.Resource);

                            //string myString = System.Text.Encoding.ASCII.GetString((byte[])claim.Resource);
                            string myString = Base64Util.EncodeTo64((byte[])claim.Resource);
                            ts.TraceInformation("myString: " + myString);                            

                            string x509HexThumbprint = Base64Util.FromBase64ToHex(myString);
                            
                            //string x509HexThumbprint = Base64Util.FromBase64ToHex(Base64Util.DecodeFrom64((byte[])claim.Resource));

                            ts.TraceInformation("Thumbprint: " + x509HexThumbprint);
                            ts.TraceInformation("\tClaim Type: {0}", claim.ClaimType);
                            ts.TraceInformation("\tRight: {0}", claim.Right);

                            // TODO:  The identity of the CURE Idp will fail here; is it valid to check the this 
                            // certificate against the TruestedPeople store???


                            X509Certificate2 cert = CertificateUtil.GetCertificateByThumbprint(StoreName.TrustedPeople, StoreLocation.LocalMachine, myString);

                            if (cert != null)
                            {                                
                                ts.TraceInformation("Certificate: {0}", cert.Subject);
                                returnvalue = true;
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                ts.TraceInformation("Exception: " + e.Message);
                if (e.InnerException != null)
                {
                    ts.TraceInformation("InnerException: " + e.InnerException.Message);
                }
            }

            return returnvalue;
        }

        public override bool TryGetIdentity(EndpointAddress reference, out EndpointIdentity identity)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("Common.CustomIdentityVerifier.TryGetIdentity",
                            "MyTraceSource", SourceLevels.Information);

            ts.TraceInformation("Reference:  " + reference.Uri.ToString());
            ts.TraceInformation("Claim Type: " + reference.Identity.IdentityClaim.ClaimType);
            ts.TraceInformation("Identity:  " + reference.Identity.ToString());

            return IdentityVerifier.CreateDefault().TryGetIdentity(reference, out identity);
        }
    }
}
