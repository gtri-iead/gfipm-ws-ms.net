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
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.IO;

namespace Common
{
    public class CustomX509CertificateValidator : X509CertificateValidator
    {
        public CustomX509CertificateValidator()
        {
        }

        public override void Validate(X509Certificate2 certificate)
        {
            if (ValidateCertificate(certificate))
            {
                return;
            }

            throw new Exception("Certificate <" + certificate.SubjectName.Name + "> is not trusted.");
        }

        private bool ValidateCertificate(X509Certificate2 inputCertificate)
        {
            bool valid = false;

            X509Store x509Store = null;

            try
            {
                x509Store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine);

                x509Store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certs = x509Store.Certificates;

                foreach (X509Certificate2 cert in certs)
                {
                    if (CompareCertificates(inputCertificate, cert))
                    {
                        valid = true;
                        break;
                    }
                }
            }
            finally
            {
                if (x509Store != null)
                {
                    x509Store.Close();
                }
            }

            return valid;
        }

        private bool CompareCertificates(X509Certificate2 inputCertificate, X509Certificate2 certificate)
        {
            if (inputCertificate.Thumbprint == certificate.Thumbprint)
            {
                return true;
            }

            return false;
        }
    }
}
