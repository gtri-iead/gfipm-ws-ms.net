using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Common;

namespace CommercialVehicle_WSC
{
    public class CustomX509CertificateValidator : X509CertificateValidator
    {
        public CustomX509CertificateValidator()
        {
            StreamWriter file = new StreamWriter("c:\\temp\\WeatherStationService.CustomX509CertificateValidator - CustomX509CertificateValidator.txt", true);
            file.WriteLine("_________________________________________");
            file.WriteLine("DateTime: " + DateTime.Now.ToString());
            file.WriteLine("CTOR");
            file.Close();
        }

        public override void Validate(X509Certificate2 certificate)
        {
            StreamWriter file = null;

            try
            {
                file = new StreamWriter("c:\\temp\\WeatherStationService.CustomX509CertificateValidator - Validate.txt", true);
                file.WriteLine("_________________________________________");
                file.WriteLine("DateTime: " + DateTime.Now.ToString());

                // Check that there is a certificate.
                if (certificate == null)
                {
                    file.WriteLine("certificate: " + "NULL");

                    throw new ArgumentNullException("certificate");
                }

                file.WriteLine("certificate.IssuerName.Name: " + certificate.IssuerName.Name);
                file.WriteLine("certificate.SubjectName: " + certificate.SubjectName.Name);

                //if (CertificateUtil.ValidateCertificate(StoreName.TrustedPeople, StoreLocation.LocalMachine, certificate))
                //{
                //    file.WriteLine("VALID: " + "TRUE");
                //}
                //else
                //{
                //    file.WriteLine("VALID: " + "FALSE");
                //}

                //StackTracer.TraceStack(file);

                if (CertificateUtil.ValidateCertificate(StoreName.TrustedPeople, StoreLocation.LocalMachine, certificate))
                {
                    file.WriteLine("VALID: " + "TRUE");
                    return;
                }

                file.WriteLine("VALID: " + "FALSE");

            }
            catch (Exception e)
            {
                file.WriteLine("Exception: " + e.Message);
                if (e.InnerException != null)
                {
                    file.WriteLine("Inner Exception: " + e.InnerException.Message);
                }
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }

            // Only accept self-issued certificates
            throw new Exception("Certificate <" + certificate.SubjectName.Name + "> is not trusted.");
        }
    }
}
