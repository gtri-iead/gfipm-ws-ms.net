using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.IO;

namespace CommercialVehicle_WSC
{
    class WebServiceConsumerFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHost host = new ServiceHost(serviceType, baseAddresses);

            //foreach (ServiceEndpoint se in host.Description.Endpoints)
            //{
            //    se.Behaviors.Add(new Common.ProtectionLevelBehavior(ProtectionLevel.Sign));
            //}

            return host;
        }
    }
}
