using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using Microsoft.IdentityModel.Tokens;


namespace WSP_SelfHosted
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:9000/Model2SelfHostedWSP");

            ServiceHost serviceHost = new ServiceHost(typeof(CommercialVehicleCollisionWSPImpl.CommercialVehicleCollisionWebService), baseAddress);

            FederatedServiceCredentials.ConfigureServiceHost(serviceHost);


            MutualCertificateAsymmerticCustomBinding bindingFactory = new MutualCertificateAsymmerticCustomBinding(false);

            CustomBinding binding = bindingFactory.CustomBinding;

            ServiceEndpoint endpnt = serviceHost.AddServiceEndpoint(typeof(CommercialVehicleCollisionServiceContract.ICommercialVehicleCollisionPortType),
                binding, "CommercialVehicleCollisionWebservice");

            serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My,
                           X509FindType.FindByThumbprint, "bc 26 0b 4d 79 29 05 a3 c8 e4 0f d9 0f 1c 47 a4 15 fa be d5");

            serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            serviceHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator 
                = new CommercialVehicleCollisionWebservice.CustomX509CertificateValidator() as X509CertificateValidator;

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            serviceHost.Description.Behaviors.Add(smb);

            serviceHost.Open();

            Console.WriteLine("running...");

            Console.ReadLine();
        }
    }
}
