using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

using System.ServiceModel.Channels;
using  System.ServiceModel.Security.Tokens;
using System.IdentityModel.Tokens;
using System.ServiceModel.Dispatcher;
using System.IO;

namespace WeatherStationService
{
    public class SymmetricPolicyExtensionsBehavior : IEndpointBehavior
    {

        public void Validate(ServiceEndpoint endpoint)
        {
            StreamWriter file = new StreamWriter("c:\\temp\\WeatherStationService.SymmetricPolicyExtensionsBehavior - Validate.txt", true);
            file.WriteLine("_________________________________________");
            file.WriteLine("DateTime: " + DateTime.Now.ToString());

            if (endpoint != null)
            {
                file.WriteLine("EndPoint: " + endpoint.Name);
                if (endpoint.Behaviors != null)
                {
                    foreach (IEndpointBehavior epb in endpoint.Behaviors)
                    {
                        file.WriteLine("EndPoint Behavior: " + epb.ToString());
                    }
                }
            }


            file.Close();

            //ClientCredentials cc = new ClientCredentials();
            //ClientRuntime cr = new ClientRuntime();
            //cc.ApplyClientBehavior(endpoint, cr);

            //CustomBinding binding = new CustomBinding("CustomSecureBinding_BackendService");
            CustomBinding binding = new CustomBinding();

            SymmetricSecurityBindingElement secBindingElement = new SymmetricSecurityBindingElement();
            
            //WS2007HttpBinding stsBinding = new WS2007HttpBinding("wssuntBinding");
            CustomBinding stsBinding = new CustomBinding("ADS-CustomSecureTransport");
            

            IssuedSecurityTokenParameters issuedTokenParameters = new IssuedSecurityTokenParameters("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",
                new EndpointAddress("https://ha50idp:8543/ADS-STS/Issue.svc"), stsBinding);

            issuedTokenParameters.KeyType = SecurityKeyType.AsymmetricKey;

            // 256?
            issuedTokenParameters.KeySize = 128;

            issuedTokenParameters.IssuerMetadataAddress = new EndpointAddress("https://ha50idp:8543/ADS-STS/Issue.svc/mex");

            issuedTokenParameters.RequireDerivedKeys = false;

            issuedTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;

            //
            X509SecurityTokenParameters protectTokenParameters = new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Thumbprint, SecurityTokenInclusionMode.Never);

            protectTokenParameters.RequireDerivedKeys = false;

            protectTokenParameters.InclusionMode = SecurityTokenInclusionMode.Never;
            //protectTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
            
            secBindingElement.ProtectionTokenParameters = protectTokenParameters;

            secBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Lax;
            
            //secBindingElement.EndpointSupportingTokenParameters.Signed.Add(issuedTokenParameters); 
            secBindingElement.EndpointSupportingTokenParameters.Signed.Add(issuedTokenParameters); 
            
            secBindingElement.EndpointSupportingTokenParameters.Endorsing.Add(protectTokenParameters);

            secBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12;

            binding.Elements.Add(secBindingElement);

            binding.Elements.Add(new MtomMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8));

            //HttpsTransportBindingElement httpsBinding = new HttpsTransportBindingElement();
            HttpTransportBindingElement httpsBinding = new HttpTransportBindingElement();
            
            binding.Elements.Add(httpsBinding);
            
            endpoint.Binding = binding;
        }
        
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            StreamWriter file = new StreamWriter("c:\\temp\\Common.SymmetricPolicyExtensionsBehavior - AddBindingParameters.txt", true);
            file.WriteLine("_________________________________________");
            file.WriteLine("DateTime: " + DateTime.Now.ToString());

            file.Close();
        }



        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)

        {
            StreamWriter file = new StreamWriter("c:\\temp\\Common.SymmetricPolicyExtensionsBehavior - ApplyClientBehavior.txt", true);
            file.WriteLine("_________________________________________");
            file.WriteLine("DateTime: " + DateTime.Now.ToString());

            file.Close();
        }



        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            StreamWriter file = new StreamWriter("c:\\temp\\Common.SymmetricPolicyExtensionsBehavior - ApplyDispatchBehavior.txt", true);
            file.WriteLine("_________________________________________");
            file.WriteLine("DateTime: " + DateTime.Now.ToString());

            file.Close();
        }

    }
}
