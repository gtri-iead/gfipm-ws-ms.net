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

namespace CommercialVehicleCollisionClient
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.Windows.Forms;
    using CommercialVehicleCollisionClient.Properties;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Reflection;

    using CommercialVehicleCollisionModel;
    using CommercialVehicleCollisionServiceContract;
    using CommercialVehicleCollisionClientProxy;

    public partial class UserAgentForm : Form
    {
        public UserAgentForm()
        {
            this.InitializeComponent();
        }
        
        private byte[] OpenImage(string imageFile)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageFile))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                return buffer;
            }
        }

        private void SaveImage(string imageFile, byte[] data)
        {
            string filePath = imageFile;

            FileStream fs = new FileStream(filePath,
                FileMode.Create, FileAccess.Write);

            fs.Write(data, 0, data.Length);

            fs.Close();
        }

        private byte[] RandomBufferOriginal(int size)
        {
            byte[] data = new byte[size];
            
            int BUFFER_SIZE = 8192;
            int len = 0;
            int left = size;
            int total = 0;

            
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

        private byte[] RandomBuffer(int size)
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
        
        private void GetDocument_Click(object sender, EventArgs e)
        {
            Common.SslServerCertificateValidator certValidator = new Common.SslServerCertificateValidator();

            CommercialVehicleCollisionPortTypeClient client = null;

            using (UserCredentialsDialog dialog = new UserCredentialsDialog())
            {
                dialog.Caption = "Connect to the WSC Service";
                dialog.Message = "Enter your credentials";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.SaveChecked)
                    {
                        dialog.ConfirmCredentials(true);
                    }

                    client = new CommercialVehicleCollisionPortTypeClient("CommercialVehicleCollisionFrontendService");


                    ////Set the Custom IdentityVerifier
                    //ssbe.LocalClientSettings.IdentityVerifier = new Common.CustomIdentityVerifier();

                    client.Endpoint.Binding.SendTimeout = TimeSpan.FromMinutes(30);
                    client.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMinutes(30);

                    client.ClientCredentials.UserName.UserName = dialog.User;
                    client.ClientCredentials.UserName.Password = dialog.PasswordToString();
                                        
                    // HA50IDP
                    //client.ClientCredentials.ServiceCertificate.SetDefaultCertificate(StoreLocation.LocalMachine, StoreName.TrustedPeople,
                    //    X509FindType.FindByThumbprint, "65 c7 27 37 93 8a 2f 24 4b 77 ff 72 0a 2a ed 48 4b 26 ef 82");

                    // CUREIDP
                    //client.ClientCredentials.ServiceCertificate.SetDefaultCertificate(StoreLocation.LocalMachine, StoreName.TrustedPeople,
                    //    X509FindType.FindByThumbprint, "a8 3a f6 ad f6 cd 7d f3 bf ca 5f 23 26 15 8f 7e ad 37 8b 66");
                    
                    try
                    {
                        ResultListBox.Items.Clear();

                        // Use the 'client' variable to call operations on the service.
                        CommercialVehicleCollisionDocumentType doc = client.getDocument("AnId\tANother line");

                        
                        ResultListBox.Visible = true;
                        ResultListBox.Items.Add("DocID: " + doc.DocumentFileControlID);
                        
                        ResultListBox.Items.Add("Incident Text: " + doc.IncidentText);

                        foreach (string vin in doc.InvolvedVehicleVIN)
                        {
                            ResultListBox.Items.Add("Vehicle VIN: " + vin);
                        }                                                
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("\tInner Error: " + ex.InnerException.Message);
                        }

                        client.Abort();
                    }
                    finally
                    {
                        client.Close();
                    }
                }
            }
        }

        private void upLoadPhoto_Click(object sender, EventArgs e)
        {
            Common.SslServerCertificateValidator certValidator = new Common.SslServerCertificateValidator();

            CommercialVehicleCollisionPortTypeClient client = null;

            using (UserCredentialsDialog dialog = new UserCredentialsDialog())
            {
                dialog.Caption = "Connect to the WSC Service";
                dialog.Message = "Enter your credentials";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.SaveChecked)
                    {
                        dialog.ConfirmCredentials(true);
                    }

                    client = new CommercialVehicleCollisionPortTypeClient("CommercialVehicleCollisionFrontendService");
              
                    client.ClientCredentials.UserName.UserName = dialog.User;
                    client.ClientCredentials.UserName.Password = dialog.PasswordToString();

                    client.Endpoint.Binding.SendTimeout = TimeSpan.FromMinutes(30);
                    client.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMinutes(30);

                    // New symmetric binding over transport

                    // HA50IDP
                    //client.ClientCredentials.ServiceCertificate.SetDefaultCertificate(StoreLocation.LocalMachine, StoreName.TrustedPeople,
                    //    X509FindType.FindByThumbprint, "65 c7 27 37 93 8a 2f 24 4b 77 ff 72 0a 2a ed 48 4b 26 ef 82");

                    // CUREIDP
                    //client.ClientCredentials.ServiceCertificate.SetDefaultCertificate(StoreLocation.LocalMachine, StoreName.TrustedPeople,
                    //    X509FindType.FindByThumbprint, "a8 3a f6 ad f6 cd 7d f3 bf ca 5f 23 26 15 8f 7e ad 37 8b 66");

                    try
                    {
                        // Use the 'client' variable to call operations on the service.
                        ResultListBox.Items.Clear();

                        string imageFile = "CommercialVehicleCollisionClient.Net.jpg";
                        byte[] photoUpload = OpenImage(imageFile);
                        string photoId = client.uploadPhoto(photoUpload);
                                                
                        ResultListBox.Visible = true;
                        ResultListBox.Items.Add("photoId: " + photoId);           
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("\tInner Error: " + ex.InnerException.Message);
                        }

                        client.Abort();
                    }
                    finally
                    {
                        client.Close();
                    }
                }
            }
        }

        private void downloadData_Click(object sender, EventArgs e)
        {
            Common.SslServerCertificateValidator certValidator = new Common.SslServerCertificateValidator();

            CommercialVehicleCollisionPortTypeClient client = null;

            using (UserCredentialsDialog dialog = new UserCredentialsDialog())
            {
                dialog.Caption = "Connect to the WSC Service";
                dialog.Message = "Enter your credentials";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.SaveChecked)
                    {
                        dialog.ConfirmCredentials(true);
                    }

                    client = new CommercialVehicleCollisionPortTypeClient("CommercialVehicleCollisionFrontendService");

                    client.ClientCredentials.UserName.UserName = dialog.User;
                    client.ClientCredentials.UserName.Password = dialog.PasswordToString();

                    client.Endpoint.Binding.SendTimeout = TimeSpan.FromMinutes(30);
                    client.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMinutes(30);

                    try
                    {
                        ResultListBox.Items.Clear();

                        // download the file
                        int size = 20000000;
                        byte[] photoDownload = client.downloadData(size);
                                        
                        ResultListBox.Visible = true;

                        ResultListBox.Items.Add("Download size = " + photoDownload.Length);
                        
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
                                    ResultListBox.Items.Add("\tFAIL: Generated data is different");
                                }
                            }

                            total += len;
                        }

                        if (size != data.Length)
                        {
                            ResultListBox.Items.Add("\tFAIL: Generated data is different SIZE");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("\tInner Error: " + ex.InnerException.Message);
                        }
                    }
                    finally
                    {
                        client.Close();
                    }
                }
            }
        }
                
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
