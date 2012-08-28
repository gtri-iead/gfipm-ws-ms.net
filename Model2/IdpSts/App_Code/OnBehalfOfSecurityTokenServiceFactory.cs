
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.IdentityModel.Protocols.WSTrust;
using System.ServiceModel;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using IdentityProviderSts;
using Microsoft.IdentityModel.Tokens.Saml11;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.IO;
using System.Web.Configuration;

/// <summary>
/// Summary description for ActAsSecurityTokenServiceFactory
/// </summary>
public class OnBehalfOfSecurityTokenServiceFactory : WSTrustServiceHostFactory
{
    public OnBehalfOfSecurityTokenServiceFactory()
    {
        StreamWriter file = new StreamWriter("c:\\temp\\IdentityProviderSts.OnBehalfOfSecurityTokenServiceFactory - OnBehalfOfSecurityTokenServiceFactory.txt", true);
        file.WriteLine("_________________________________________");
        file.WriteLine("DateTime: " + DateTime.Now.ToString());

        file.WriteLine("SigningCertificateName:" + WebConfigurationManager.AppSettings[CommonConfigurationSettings.SigningCertificateName]);
        file.Close();
    }

    public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
    {
        StreamWriter file = new StreamWriter("c:\\temp\\IdentityProviderSts.OnBehalfOfSecurityTokenServiceFactory - CreateServiceHost.txt", true);
        file.WriteLine("_________________________________________");
        file.WriteLine("DateTime: " + DateTime.Now.ToString());

        file.WriteLine("constructorString:" + constructorString);
        file.Close();


        SecurityTokenServiceConfiguration config = new SecurityTokenServiceConfiguration("https://ha50idp:8544/IDP-STS/Issue.svc");

        //Uri baseUri = baseAddresses.FirstOrDefault(a => a.Scheme == "https");
        //if (baseUri == null)
        //    throw new InvalidOperationException("The STS should be hosted under https");

        //config.TrustEndpoints.Add(new ServiceHostEndpointConfiguration(typeof(IWSTrust13SyncContract), GetCertificateCredentialsBinding(), baseUri + ""));
        
        // Set the STS implementation class type
        config.SecurityTokenService = typeof(CustomSecurityTokenService);

        // Create a security token handler collection and then provide with a SAML11 security token
        // handler and set the Audience restriction to Never
        SecurityTokenHandlerCollection onBehalfOfHandlers = new SecurityTokenHandlerCollection();
        Saml2SecurityTokenHandler onBehalfOfTokenHandler = new Saml2SecurityTokenHandler();
        
        onBehalfOfHandlers.Add(onBehalfOfTokenHandler);
        //onBehalfOfHandlers.Add(userNameTokenHandler);
        onBehalfOfHandlers.Configuration.AudienceRestriction.AudienceMode = AudienceUriMode.Never;

        // Set the appropriate issuer name registry
        //onBehalfOfHandlers.Configuration.IssuerNameRegistry = new IdentityProviderIssuerNameRegistry();

        // Set the token handlers collection
        config.SecurityTokenHandlerCollectionManager[SecurityTokenHandlerCollectionManager.Usage.OnBehalfOf] = onBehalfOfHandlers;

//        WindowsUserNameSecurityTokenHandler userNameTokenHandler = new WindowsUserNameSecurityTokenHandler();
//        config.SecurityTokenHandlerCollectionManager[SecurityTokenHandlerCollectionManager.Usage.Default].Add(userNameTokenHandler);
        
        WSTrustServiceHost host = new WSTrustServiceHost(config, baseAddresses);        
        return host;
    }
}
