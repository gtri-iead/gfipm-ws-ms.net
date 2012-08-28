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
using System.Diagnostics;

namespace Common
{

    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.IO;

    /// <summary>
    /// A utility class which helps to retrieve an x509 certificate
    /// </summary>
    public class CertificateUtil
    {
        /// <summary>
        /// Validates a certificate
        /// </summary>
        /// <param name="name">Certificate Store where to look for the certificate.</param>
        /// <param name="location">StoreLocation of the certificate.</param>
        /// <param name="subjectName">Distinguished name of the certificate subject name.</param>
        /// <returns>Instance of X509Certificate2.</returns>
        public static bool ValidateCertificate(StoreName name, StoreLocation location, X509Certificate2 inputCertificate)
        {
            //CustomTextTraceSource ts = new CustomTextTraceSource("Common.CertificateUtil.ValidateCertificate",
            //    "MyTraceSource", SourceLevels.Information);

            //ts.TraceInformation("subjectName: " + inputCertificate.SubjectName.Name);


            bool result = false;

            X509Store store = null;

            try
            {

                store = new X509Store(name, location);
                X509Certificate2Collection certificates = null;
                store.Open(OpenFlags.ReadOnly);

                //
                // Every time we call store.Certificates property, a new collection will be returned.
                //
                certificates = store.Certificates;

                for (int i = 0; i < certificates.Count; i++)
                {
                    X509Certificate2 cert = certificates[i];

                    if (CompareCertificates(inputCertificate, cert))
                    {
                        result = true;
                        break;
                    }
                }


                return result;
            }
            finally
            {
                if (store != null)
                {
                    for (int i = 0; i < store.Certificates.Count; i++)
                    {
                        X509Certificate2 cert = store.Certificates[i];
                        cert.Reset();
                    }

                    store.Close();
                }                
            }
        }

        private static bool CompareCertificates(X509Certificate2 inputCertificate, X509Certificate2 certificate)
        {
            if (inputCertificate.Thumbprint == certificate.Thumbprint)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Gets a certificate from a given store.
        /// </summary>
        /// <param name="name">Certificate Store where to look for the certificate.</param>
        /// <param name="location">StoreLocation of the certificate.</param>
        /// <param name="subjectName">Distinguished name of the certificate subject name.</param>
        /// <returns>Instance of X509Certificate2.</returns>
        public static X509Certificate2 GetCertificate(StoreName name, StoreLocation location, string subjectName)
        {
            X509Store store = new X509Store(name, location);
            X509Certificate2Collection certificates = null;
            store.Open(OpenFlags.ReadOnly);

            try
            {
                X509Certificate2 result = null;

                //
                // Every time we call store.Certificates property, a new collection will be returned.
                //
                certificates = store.Certificates;

                for (int i = 0; i < certificates.Count; i++)
                {
                    X509Certificate2 cert = certificates[i];

                    if (cert.SubjectName.Name.ToLower() == subjectName.ToLower())
                    {
                        if (result != null)
                        {
                            throw new ApplicationException(string.Format("There are multiple certificate for subject Name {0}", subjectName));
                        }

                        result = new X509Certificate2(cert);
                    }
                }

                if (result == null)
                {
                    throw new ApplicationException(string.Format("No certificate was found for subject Name {0}", subjectName));
                }

                return result;
            }
            finally
            {
                if (certificates != null)
                {
                    for (int i = 0; i < certificates.Count; i++)
                    {
                        X509Certificate2 cert = certificates[i];
                        cert.Reset();
                    }
                }

                store.Close();
            }
        }

        /// <summary>
        /// Gets a certificate from a given store for a common name.
        /// </summary>
        /// <param name="name">Certificate Store where to look for the certificate.</param>
        /// <param name="location">StoreLocation of the certificate.</param>
        /// <param name="commonName">Common name of the certificate subject name.</param>
        /// <returns>Instance of X509Certificate2.</returns>
        public static X509Certificate2 GetCertificateByCommonName(StoreName name, StoreLocation location, string commonName)
        {
            X509Store store = new X509Store(name, location);
            X509Certificate2Collection certificates = null;
            store.Open(OpenFlags.ReadOnly);

            try
            {
                X509Certificate2 result = null;

                //
                // Every time we call store.Certificates property, a new collection will be returned.
                //
                certificates = store.Certificates;

                for (int i = 0; i < certificates.Count; i++)
                {
                    X509Certificate2 cert = certificates[i];

                    string certCommonName = cert.GetNameInfo(X509NameType.SimpleName, false);

                    //if (string.Compare(commonName, "Oceania") == 0)

                    if (certCommonName.ToLower() == commonName.ToLower())
                    {
                        if (result != null)
                        {
                            throw new ApplicationException(string.Format("There are multiple certificate for common Name {0}", commonName));
                        }

                        result = new X509Certificate2(cert);
                    }
                }

                if (result == null)
                {
                    throw new ApplicationException(string.Format("No certificate was found for common Name {0}", commonName));
                }

                return result;
            }
            finally
            {
                if (certificates != null)
                {
                    for (int i = 0; i < certificates.Count; i++)
                    {
                        X509Certificate2 cert = certificates[i];
                        cert.Reset();
                    }
                }

                store.Close();
            }
        }

        /// <summary>
        /// Gets a certificate from a given store for a common name.
        /// </summary>
        /// <param name="name">Certificate Store where to look for the certificate.</param>
        /// <param name="location">StoreLocation of the certificate.</param>
        /// <param name="commonName">Common name of the certificate subject name.</param>
        /// <returns>Instance of X509Certificate2.</returns>
        public static X509Certificate2 GetCertificateByThumbprint(StoreName name, StoreLocation location, string base64EncodedThumbPrint)
        {
            X509Store store = new X509Store(name, location);
            X509Certificate2Collection certificates = null;
            store.Open(OpenFlags.ReadOnly);

            try
            {
                X509Certificate2 result = null;

                //
                // Every time we call store.Certificates property, a new collection will be returned.
                //
                certificates = store.Certificates;

                for (int i = 0; i < certificates.Count; i++)
                {
                    X509Certificate2 cert = certificates[i];

                    string certThumbprint = Base64Util.EncodeTo64(cert.GetCertHash());

                    //if (string.Compare(commonName, "Oceania") == 0)

                    if (base64EncodedThumbPrint.ToLower() == certThumbprint.ToLower())
                    {
                        if (result != null)
                        {
                            throw new ApplicationException(string.Format("There are multiple certificate for Thumbprint {0}", base64EncodedThumbPrint));
                        }

                        result = new X509Certificate2(cert);
                    }
                }

                if (result == null)
                {
                    throw new ApplicationException(string.Format("No certificate was found for Thumbprint {0}", base64EncodedThumbPrint));
                }

                return result;
            }
            finally
            {
                if (certificates != null)
                {
                    for (int i = 0; i < certificates.Count; i++)
                    {
                        X509Certificate2 cert = certificates[i];
                        cert.Reset();
                    }
                }

                store.Close();
            }
        }
    }

}
