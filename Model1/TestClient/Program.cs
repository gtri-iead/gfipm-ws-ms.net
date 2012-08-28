using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.IO;
using Common;

using CommercialVehicleCollisionWebservice;

namespace TestClient
{
    class Program
    {
        static byte[] OpenImage(string imageFile)
        {
            string filePath = imageFile;

            // read the file and return the byte[]s
            using (FileStream fs = new FileStream(filePath, FileMode.Open,
                                       FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                return buffer;
            }
        }

        static void SaveImage(string imageFile, byte[] data)
        {
            string filePath = imageFile;

            FileStream fs = new FileStream(filePath,
                FileMode.Create, FileAccess.Write);

            fs.Write(data, 0, data.Length);

            fs.Close();
        }

        static byte[] RandomBufferOriginal(int size)
        {
            byte[] data = new byte[size];

            //Random r = new Random(Environment.TickCount);
            //r.NextBytes(data);

            //if ( data != null )
            //    return data;

            //MemoryStream memStream = new MemoryStream(data);

            //byte[] buffer = new byte[8192];

            int BUFFER_SIZE = 8192;
            int len = 0;
            int left = size;
            int total = 0;

            //while ((len = memStream.Read(buffer, 0, buffer.Length)) < memStream.Length)
            do
            {
                if (left > BUFFER_SIZE)
                {
                    len = BUFFER_SIZE;
                }
                else
                {
                    len = left;
                }

                for (int i = 0; i < len; i++)
                {
                    data[total + i] = (byte)('A' + (total + i) % 26);
                }

                total += len;

                left -= BUFFER_SIZE;
            } while (left > 0);

            return data;
        }

        static byte[] RandomBuffer(int size)
        {
            byte[] data = new byte[size];

            MemoryStream memStream = new MemoryStream(data);

            byte[] buffer = new byte[8192];

            int BUFFER_SIZE = 8192;
            int len = 0;
            int left = size;
            int total = 0;

            //while ((len = memStream.Read(buffer, 0, buffer.Length)) < memStream.Length)
            //do
            {
                if (left > BUFFER_SIZE)
                {
                    len = BUFFER_SIZE;
                }
                else
                {
                    len = left;
                }

                for (int i = 0; i < len; i++)
                {
                    data[total + i] = (byte)('A' + (total + i) % 26);
                }

                total += len;

                left -= BUFFER_SIZE;
            } while (left > 0) ;

            return data;
        }

        static void Main(string[] args)
        {
            CommercialVehicleCollisionPortTypeClient client = null;

            try
            {
                //string result = "Random String ";

                //return result;

                // WSP Client Proxy
                //MasterHelloWorldClient client = new MasterHelloWorldClient();

                // Use the 'client' variable to call operations on the service.
                //result = client.HelloWorld(result);

                System.Net.ServicePointManager.ServerCertificateValidationCallback
                    += new System.Net.Security.RemoteCertificateValidationCallback(ServiceCertificateValidator);

                //client = new CommercialVehicleCollisionPortTypeClient("CommercialVehicleCollisionWebservice.CommercialVehicleCollisionPortType-HTTPS");
                client = new CommercialVehicleCollisionPortTypeClient("Standard-ws2007HttpBinding");
                
                client.Endpoint.Contract.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;
                
                //System.ServiceModel.Description.ContractDescription cd = client.Endpoint.Contract;
                //cd.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;


                // This needs to be done here since the identityVerifier applies to the client
                WspCustomSecuredBinding binding = client.Endpoint.Binding as WspCustomSecuredBinding;

                if (binding != null)
                {
                    SecurityBindingElement sbe = (SecurityBindingElement)binding.SecurityBindingElement;
                    if (sbe != null)
                    {
                        sbe.LocalClientSettings.IdentityVerifier = new CustomIdentityVerifier();
                    }
                }

                // Use the 'client' variable to call operations on the service.
                CommercialVehicleCollisionDocumentType doc = client.getDocument("AnId");

                Console.WriteLine("DocID: " + doc.DocumentFileControlID);
                Console.WriteLine("Incident Text: " + doc.IncidentText);

                foreach (string vin in doc.InvolvedVehicleVIN)
                {
                    Console.WriteLine("Vehicle VIN: " + vin);
                }


                //// Upload a file
                //string imageFile = "C:\\temp\\MtomFile\\java.jpg";
                //byte[] photoUpload = OpenImage(imageFile);

                //string photoId = client.uploadPhoto(photoUpload);


                //// download the file
                //int size = 20000000;
                ////int size = 20000;
                //byte[] photoDownload = client.downloadData(size);

                //Console.WriteLine("\tDownload size = " + photoDownload.Length);


                //byte[] data = photoDownload;
                ////byte[] data = RandomBuffer(size);

                //MemoryStream memStream = new MemoryStream(data);

                //byte[] buffer = new byte[8192];

                //int len = 0;

                //int total = 0;
                //while ((len = memStream.Read(buffer, 0, buffer.Length)) > 0)
                //{
                //    for (int i = 0; i < len; i++)
                //    {
                //        if (buffer[i] != (byte)('A' + (total + i) % 26))
                //        {
                //            Console.WriteLine("\tFAIL: Generated data is different");
                //        }
                //    }

                //    total += len;
                //}

                //if (size != data.Length)
                //{
                //    Console.WriteLine("\tFAIL: Generated data is different SIZE");
                //}

                //SaveImage("C:\\temp\\MtomFile\\javaDload.jpg", photoDownload);


                Console.WriteLine("\n\nPress Any Key to end...");
                Console.ReadKey();

                // Always close the client.
                //client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("\tInner Error: " + e.InnerException.Message);
                }
            }
            finally
            {
                client.Close();
            }
        }

        static bool ServiceCertificateValidator(object sender, X509Certificate cert, X509Chain chain,
            System.Net.Security.SslPolicyErrors error)
        {
            X509Certificate2 x509Cert = cert as X509Certificate2;

            CustomTextTraceSource ts = new CustomTextTraceSource("ClientTest.Program.ServiceCertificateValidator",
              "MyTraceSource", SourceLevels.Information);
            
            ts.TraceInformation("Subject Name: " + x509Cert.SubjectName.Name);
            ts.TraceInformation("error: " + error.ToString());

            // Check if there were any errors
            if (error == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }
           
            return ValidateCertificate(x509Cert);
        }

        private static bool ValidateCertificate(X509Certificate2 inputCertificate)
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

        private static bool CompareCertificates(X509Certificate2 inputCertificate, X509Certificate2 certificate)
        {
            if (inputCertificate.Thumbprint == certificate.Thumbprint)
            {
                return true;
            }

            return false;
        }
    }
}
