using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.IO;
using System.Diagnostics;

namespace WebServiceProvider
{
  
    public class ServiceHost : System.ServiceModel.ServiceHost
    {
        public ServiceHost()
        {
        }

        public ServiceHost(Type serviceType, params Uri[] baseAddresses)
            :
            base(serviceType, baseAddresses)
        {
        }

        public ServiceHost(object singeltonInstance, params Uri[] baseAddresses)
            :
            base(singeltonInstance, baseAddresses)
        {
        }


        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();

            try
            {

                using (StreamWriter outfile = new StreamWriter(@"c:\\temp\\ServiceHostFactory-ApplyConfig.txt"))
                {
                    outfile.WriteLine(@"ServiceHost.ApplyConfiguration()");

                    foreach (ServiceEndpoint endPoint in Description.Endpoints)
                    {
                        outfile.WriteLine(@"Endpoint: " + endPoint.Address.ToString());
                    }
                }

                Credentials.ClientCertificate.Authentication.CertificateValidationMode 
                    = System.ServiceModel.Security.X509CertificateValidationMode.Custom;

                Credentials.ClientCertificate.Authentication.CustomCertificateValidator
                    = new WscCertificateValidator("JLA");



                //StreamWriter writer = new StreamWriter("C:\\temp\\ConsumerProvider\\CP-Host.txt");
                //writer.WriteLine("No of EndPoints: " );//+ Description.Endpoints.Count.ToString());

                //writer.Close();
            }
            catch(Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message);
            }
                        
            //foreach (ServiceEndpoint endPoint in Description.Endpoints)
            //{
            //    writer.WriteLine("Endpoint: " + endPoint.Address.ToString());


            //}

            //writer.Close();
        }


    }

    public sealed class ServiceHostFactory : System.ServiceModel.Activation.ServiceHostFactory
    {
        public override System.ServiceModel.ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            return base.CreateServiceHost(constructorString, baseAddresses);
        }

        protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new WebServiceProvider.ServiceHost(serviceType, baseAddresses);
        }
    }
}
