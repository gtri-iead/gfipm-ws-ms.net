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
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace Common
{
    public class CustomX509CertificateValidator : X509CertificateValidator
    {
        public CustomX509CertificateValidator()
        {
        }

        public override void Validate(X509Certificate2 certificate)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("Common.CustomX509CertificateValidator.Validate",
                "MyTraceSource", SourceLevels.Information);

            // Check that there is a certificate.
            if (certificate == null)
            {
                ts.TraceInformation("null certificate");
                throw new ArgumentNullException("certificate");
            }

            if (CertificateUtil.ValidateCertificate(StoreName.TrustedPeople, StoreLocation.LocalMachine, certificate))
            {
                ts.TraceInformation("Certificate <" + certificate.SubjectName.Name + "> is trusted.");
                return;
            }

        
            // Only accept self-issued certificates
            ts.TraceInformation("Certificate <" + certificate.SubjectName.Name + "> is not trusted.");

            throw new Exception("Certificate <" + certificate.SubjectName.Name + "> is not trusted.");
        }
    }
}
