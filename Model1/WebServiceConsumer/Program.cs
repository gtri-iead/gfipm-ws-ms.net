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
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Configuration;
using System.ServiceModel.Security;
using System.Threading;
using System.Reflection;

using ComercialVehicleCollisionClientProxy;
using CommercialVehicleCollisionModel;
using CommercialVehicleCollisionServiceContract;
using Common;

namespace WebServiceConsumer
{
    class Program
    {
        static byte[] OpenImage(string imageFile)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageFile) )
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
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
       
        static void Main(string[] args)
        {
            CommercialVehicleCollisionPortTypeClient client = null;

            try
            {

                // Wrap this inside a PermissiveCErtificatePolicy or similar class 
                System.Net.ServicePointManager.ServerCertificateValidationCallback
                    += new System.Net.Security.RemoteCertificateValidationCallback(ServiceCertificateValidator);

                string endpointName = ConfigurationManager.AppSettings["EndPointName"];

                client = new CommercialVehicleCollisionPortTypeClient(endpointName);

                // Use the 'client' variable to call operations on the service.
                CommercialVehicleCollisionDocumentType doc = client.getDocument("AnId");

                Console.WriteLine("DocID: " + doc.DocumentFileControlID);
                Console.WriteLine("Incident Text: " + doc.IncidentText);

                foreach (string vin in doc.InvolvedVehicleVIN)
                {
                    Console.WriteLine("Vehicle VIN: " + vin);
                }

                Console.WriteLine("Press a key...");
                Console.ReadKey();


                // Upload a file
                Console.WriteLine("\nUpload File");

                string imageFile = "WebServiceConsumer.Net.jpg";
                byte[] photoUpload = OpenImage(imageFile);

                string photoId = client.uploadPhoto(photoUpload);

                Console.WriteLine("photoId: " + photoId);

                Console.WriteLine("Press a key...");
                Console.ReadKey();
                

                // download binary data
                
                int size = 20000000;
                byte[] photoDownload = client.downloadData(size);

                Console.WriteLine("\tDownload size = " + photoDownload.Length);


                byte[] data = photoDownload;
                
                MemoryStream memStream = new MemoryStream(data);

                byte[] buffer = new byte[8192];

                int len = 0;

                int total = 0;
                while ((len = memStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        if (buffer[i] != (byte)('A' + (total + i) % 26))
                        {
                            Console.WriteLine("\tFAIL: Generated data is different");
                            break;
                        }
                    }

                    total += len;
                }

                if (size != data.Length)
                {
                    Console.WriteLine("\tFAIL: Generated data SIZE is different");
                }
                else
                {
                    Console.WriteLine("Download Test Passed.");
                }

                Console.WriteLine("Press a key...");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("\tInner Error: " + e.InnerException.Message);
                }

                client.Abort();
            }
            finally
            {
                client.Close();
            }
        }

        static bool ServiceCertificateValidator(object sender, X509Certificate cert, X509Chain chain, 
            System.Net.Security.SslPolicyErrors error)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("WebServiceConsumer.Program.ServiceCertificateValidator",
                "MyTraceSource", SourceLevels.Information);
            
            X509Certificate2 x509Cert = cert as X509Certificate2;

            ts.TraceInformation("Error: " + error.ToString());

            
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
