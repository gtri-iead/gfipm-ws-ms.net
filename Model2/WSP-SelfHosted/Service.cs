
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using Resources;
using System.Diagnostics;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;


namespace CommercialVehicleCollisionWebservice
{

    public class CommercialVehicleCollisionWebService : ICommercialVehicleCollisionPortType
    {
        #region Private

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

        private string GetAddressAsString()
        {
            RemoteEndpointMessageProperty clientEndpoint =
            OperationContext.Current.IncomingMessageProperties[
            RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            return String.Format( "{0}:{1}", clientEndpoint.Address, clientEndpoint.Port);
        }

        #endregion

        #region Public

        public virtual getDocumentResponse getDocument(getDocumentRequest request)
        {
            // throw new ArgumentOutOfRangeException("This request was out of range.");
            try
            {
                CommercialVehicleCollisionDocumentType doc = new CommercialVehicleCollisionDocumentType();

                doc.DocumentFileControlID = "12348765";

                
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendLine("Speeding");
                strBuilder.Append(" Ticket");

                doc.IncidentText = "Speeding Ticket";
                //doc.IncidentText = strBuilder.ToString();

                List<string> vins = new List<string>();

                vins.Add("VIN # 97651234");
                vins.Add("VIN # 87651234");
                vins.Add("VIN # 77651234");
                //vins.Add("Client's Address: " + GetAddressAsString());
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
                string uploadFolder = ConfigurationManager.AppSettings["UploadFolder"];

                if (String.IsNullOrEmpty(uploadFolder))
                {
                    throw new InvalidOperationException("Upload folder not present.");
                }

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }


                string _uploadFilename = ConfigurationManager.AppSettings["UploadFilePrefix"];

                string photoId = System.Guid.NewGuid().ToString();
                _uploadFilename += photoId;
                _uploadFilename += ".jpg";



                string path = Path.Combine(uploadFolder, _uploadFilename);
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
                //if ( String.IsNullOrEmpty(_uploadFilename) )
                //{
                //     throw new InvalidOperationException("No files to download. Upload a file first.");
                //}

                //string uploadFolder = ConfigurationManager.AppSettings["UploadFolder"];

                //if (String.IsNullOrEmpty(uploadFolder))
                //{
                //    throw new InvalidOperationException("Upload folder not present.");
                //}

                //if (!Directory.Exists(uploadFolder))
                //{
                //    throw new InvalidOperationException("Upload folder not present.");
                //}

                //DirectoryInfo di = new DirectoryInfo(uploadFolder);
                //FileInfo[] jpegFiles = di.GetFiles("*.jpg");
                //foreach (FileInfo fi in jpegFiles)
                //{
                //    _uploadFilename = fi.FullName;
                //    break;
                //}


                //byte[] data = OpenImage(_uploadFilename); 

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

        #endregion
    }
}
