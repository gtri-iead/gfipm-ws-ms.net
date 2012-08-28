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
using System.Diagnostics;
using System.Configuration;
using System.IO;

using CommercialVehicleCollisionModel;
using CommercialVehicleCollisionServiceContract;

namespace CommercialVehicleCollisionWebservice
{   
    [System.ServiceModel.ServiceBehaviorAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0",
        InstanceContextMode=System.ServiceModel.InstanceContextMode.PerCall, 
        ConcurrencyMode=System.ServiceModel.ConcurrencyMode.Single)]
    public class CommercialVehicleCollisionPortType : ICommercialVehicleCollisionPortType
    {
        static string _uploadFilename = string.Empty;

        // private
        void SaveImage(string imageFile, byte[] data)
        {
            string filePath = imageFile;

            FileStream fs = new FileStream(filePath,
                FileMode.Create, FileAccess.Write);

            fs.Write(data, 0, data.Length);

            fs.Close();
        }   

        byte[] OpenImage(string imageFile)
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

        byte[] RandomBuffer(int size)
        {
            byte[] data = new byte[size];

            int BUFFER_SIZE = 8192;
            int len = 0;
            int left = size;
            int total = 0;

            do
            {
                if (left > BUFFER_SIZE)
                    len = BUFFER_SIZE;
                else
                    len = left;

                for (int i = 0; i < len; i++)
                {
                    data[total + i] = (byte)('A' + (total + i) % 26);
                }

                total += len;
                left -= BUFFER_SIZE;
            } while (left > 0);

            return data;
        }

        public virtual getDocumentResponse getDocument(getDocumentRequest request)
        {
            try
            {
                CommercialVehicleCollisionDocumentType doc = new CommercialVehicleCollisionDocumentType();

                doc.DocumentFileControlID = "12348765";
                doc.IncidentText = "Speeding Ticket";
                List<string> vins = new List<string>();

                vins.Add("VIN # 97651234");
                vins.Add("VIN # 87651234");
                vins.Add("VIN # 77651234");
                doc.InvolvedVehicleVIN = vins.ToArray();

                getDocumentResponse docResponse = new getDocumentResponse(doc);

                return docResponse;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Server exception: " + e.ToString());

                throw;
            }
        }
        
        public virtual uploadPhotoResponse uploadPhoto(uploadPhotoRequest request)
        {
            try
            {
                string uploadPath = AppDomain.CurrentDomain.BaseDirectory;

                string uploadFolder = ConfigurationManager.AppSettings["UploadFolder"];

                if (String.IsNullOrEmpty(uploadFolder))
                {
                    throw new InvalidOperationException("Upload folder not present.");
                }

                string upLoadFullPath = Path.Combine(uploadPath, uploadFolder);

                if (!Directory.Exists(upLoadFullPath))
                {
                    throw new InvalidOperationException("Upload folder not present.");
                }
                
                string _uploadFilename = ConfigurationManager.AppSettings["UploadFilePrefix"];

                string photoId = System.Guid.NewGuid().ToString();
                _uploadFilename += photoId;
                _uploadFilename += ".jpg";

                string path = Path.Combine(upLoadFullPath, _uploadFilename);
                if (File.Exists(path))
                {
                    if ((File.GetAttributes(path) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(path, FileAttributes.Normal);
                    }

                    File.Delete(path);
                }

                _uploadFilename = path;

                SaveImage(_uploadFilename, request.Photo);              

                uploadPhotoResponse photoResponse = new uploadPhotoResponse(photoId);

                return photoResponse;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Server exception: " + e.ToString());

                throw;
            }
        }
        
        public virtual downloadDataResponse downloadData(downloadDataRequest request)
        {
            try
            {
                byte[] data = RandomBuffer(request.Size); 

                downloadDataResponse dataResponse = new downloadDataResponse(data);

                return dataResponse;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Server exception: " + e.ToString());

                throw;
            }
        }
    }
}
