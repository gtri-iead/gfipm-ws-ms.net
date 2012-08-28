using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using Common;

namespace IdpAds
{

    /// <summary>
    /// Summary description for CustomCertificateValidator
    /// </summary>
    public class MyX509CertificateValidator : X509CertificateValidator
    {
        public MyX509CertificateValidator()
        {
        }

        public override void Validate(X509Certificate2 certificate)
        {
            // Check that there is a certificate.
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }


            if (CertificateUtil.ValidateCertificate(StoreName.TrustedPeople, StoreLocation.LocalMachine, certificate))
            {
                return;
            }

            throw new Exception("Certificate <" + certificate.SubjectName.Name + "> is not trusted.");
        }
    }
}