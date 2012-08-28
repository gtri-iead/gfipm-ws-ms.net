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
using System.Xml;
using Microsoft.IdentityModel.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace Common
{
    public class ClaimsUtil
    {
        public static void LogClaimsPrincipal(IClaimsPrincipal cp, string fromMethod)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource(fromMethod, "MyTraceSource", System.Diagnostics.SourceLevels.Information);

            try
            {
                ts.TraceEvent(TraceEventType.Information, 1, "Number of Identities for Main Principal {0} is {1} ", cp.Identity.Name, cp.Identities.Count);

                logPrincipal(cp, ts);         
            }
            catch (Exception e)
            {
                ts.TraceInformation("Exception:" + e.Message);
                if (e.InnerException != null)
                {
                    ts.TraceInformation("\tInnerException:" + e.InnerException.Message);
                }
            }
        }

        public static void LogClaimsIdentity(IClaimsIdentity ci, string fromMethod)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource(fromMethod, "MyTraceSource", System.Diagnostics.SourceLevels.Information);

            try
            {
                ts.TraceInformation("Method {0}", fromMethod);

                logClaimsIdentity(ci, ts);
            }
            catch (Exception e)
            {
                ts.TraceInformation("Exception:" + e.Message);
                if (e.InnerException != null)
                {
                    ts.TraceInformation("\tInnerException:" + e.InnerException.Message);
                }
            }
       }

        private static void logClaimsIdentity(IClaimsIdentity ci, CustomTextTraceSource ts)
        {
            ts.TraceInformation("claimsIdentity: " + ci.Name);
            foreach (Claim c in ci.Claims)
            {
                ts.TraceInformation("Claim: <{0}, {1}>", c.ClaimType, c.Value);
            }

            // delegate
            if ( ci.Actor != null )
            {
                ts.TraceInformation("\nDelegate:");
                logClaimsIdentity(ci.Actor, ts);
            }
        }

        const string CLAIM_FORMAT_STRING = "{0, -5} {1, -15} {2}";

        // print a compact display of the supplied claim
        private static void logClaim(Claim c, CustomTextTraceSource ts)
        {
            string claimType = c.ClaimType;
            string value = c.Value;

            ts.TraceInformation(CLAIM_FORMAT_STRING, c.Subject, claimType, value);

            if (claimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/thumbprint")
            {
                //value = Base64Util.DecodeFrom64(value);

                X509Certificate2 cert = CertificateUtil.GetCertificateByThumbprint(StoreName.TrustedPeople, StoreLocation.LocalMachine, value);
                //X509Certificate2 cert = CertificateUtil.GetCertificateByCommonName(StoreName.TrustedPeople, StoreLocation.LocalMachine, "HA50WSC");
                //X509Certificate2 cert = System.ServiceModel.EndpointIdentity.CreateX509CertificateIdentity((X509Certificate2)c.Subject.Actor);

                if (cert != null)
                {
                    ts.TraceInformation("X509Certificate2: " + cert.Subject);
                    //file.WriteLine("X509Certificate2: " + cert.Thumbprint);
                    //file.WriteLine("X509Certificate2 H: " + cert.GetCertHashString());
                    //file.WriteLine("X509Certificate2 H64: " + cert.GetCertHash().ToString());
                    //file.WriteLine("X509Certificate2 D: " + Base64Util.DecodeFrom64(cert.Thumbprint));

                    //file.WriteLine("X509Certificate2 D: " + Base64Util.EncodeTo64(cert.GetCertHash()));
                }
                else
                {
                    ts.TraceInformation("X509Certificate2: " + "NULL");
                }

            }


        }


        //private static void logClaimsIdentity(IClaimsIdentity ci, CustomTextTraceSource ts)
        //{
        //    ts.TraceInformation("__________________________________");
        //    ts.TraceInformation(CLAIM_FORMAT_STRING, "SUBJECT", "CLAIM_TYPE", "RESOURCE");

        //    // display claims for subject of this claimset
        //    foreach (Claim c in ci.Claims)
        //    {
        //        logClaim(c, ts);

                
        //        ts.TraceInformation("ISSUED BY: ");

        //        // display claims for issuer of this claimset
        //        if (null == c.Issuer)
        //        {
        //            ts.TraceInformation("null");
        //        }
        //        else if (object.ReferenceEquals(c, c.Issuer))
        //        {
        //            // self-asserted claims (issuer is the same as subject)
        //            ts.TraceInformation("self");
        //        }
        //        else
        //        {
        //            ts.TraceInformation(c.Issuer);
        //        }
        //    }
        //}

        private static void logPrincipal(IClaimsPrincipal cp, CustomTextTraceSource ts)
        {
            //Console.WriteLine();
            //Console.WriteLine(CLAIM_FORMAT_STRING, "RIGHT", "CLAIM_TYPE", "RESOURCE");

            // display claims for subject of this claimset
            foreach (IClaimsIdentity ci in cp.Identities)
            {
                ts.TraceInformation("Identity: {0}", ci.Name);
                ts.TraceInformation("NameClaimType: {0}", ci.NameClaimType);
                logClaimsIdentity(ci, ts);
            }
        }

        private static void CheckAccessPrivate(AuthorizationContext context, CustomTextTraceSource ts)
        {
            ts.TraceInformation("SimpleAuthorizationManager:");

            ts.TraceInformation("\nSubject: {0}\n", context.Principal.Identity.Name);


            logPrincipal(context.Principal, ts);


            ts.TraceInformation("Actions:");
            foreach (var action in context.Action)
            {
                ts.TraceInformation(" {0}", action.ClaimType);
                ts.TraceInformation(" {0}\n", action.Value);
            }

            ts.TraceInformation("Resources:");
            foreach (var resource in context.Resource)
            {
                ts.TraceInformation(" {0}", resource.ClaimType);
                ts.TraceInformation(" {0}\n", resource.Value);
            }
        }
    }
}
