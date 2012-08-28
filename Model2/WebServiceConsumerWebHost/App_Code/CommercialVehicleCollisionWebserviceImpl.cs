
using System;
using System.Text;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Threading;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSTrust;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Common;

using CommercialVehicleCollisionModel;
using CommercialVehicleCollisionServiceContract;
using ComercialVehicleCollisionClientProxy;


namespace CommercialVehicle_WSC
{
    public class WebServiceConsumer : ICommercialVehicleCollisionPortType
    {
        #region ICommercialVehicleCollisionPortType Members

        public getDocumentResponse getDocument(getDocumentRequest request)
        {
            ICommercialVehicleCollisionPortTypeChannel channel = null;
            StreamWriter file = null;
            try
            {
                file = new StreamWriter("c:\\temp\\CommercialVehicle_WSC.WebServiceConsumer - getDocument.txt", true);
                file.WriteLine("_________________________________________");
                file.WriteLine("DateTime: " + DateTime.Now.ToString());

                if (request != null)
                {
                    file.WriteLine("Doc Request: " + request.DocumentFileControlID);
                }
                else
                {
                    file.WriteLine("Doc Request: NULL");
                }

                #region Delete???
                //IClaimsPrincipal principal = Thread.CurrentPrincipal as IClaimsPrincipal;

                //SecurityToken callerToken = principal.Identities[0].BootstrapToken;


                //// We expect only one identity, which will contain the bootstrap token.            
                //if (principal != null && principal.Identities.Count == 1)
                //{
                //    callerToken = principal.Identities[0].BootstrapToken;
                //}

                //ChannelFactory<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel> factory
                //    = new ChannelFactory<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService");
                //factory.ConfigureChannelFactory();

                //// Create and setup channel to talk to the backend service
                ////CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel channel = null;

                //// Setup the ActAs to point to the caller's token so that we perform a delegated call to the backend service
                //// on behalf of the original caller.
                //channel = factory.CreateChannelOnBehalfOf<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel>(callerToken);
                #endregion

                bool useSenderVouchesEndpoint = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSenderVouchesEndpoint"]);

                if (useSenderVouchesEndpoint)
                {
                    string senderVouchesBinding = ConfigurationManager.AppSettings["SenderVouchesEndpointBinding"];
                    file.WriteLine("Endpoint Binding: " + senderVouchesBinding);
                    //channel = CreateChannelOnBehalfOf<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel>(senderVouchesBinding);
                    channel = CreateChannelWithIssuedToken<ICommercialVehicleCollisionPortTypeChannel>(senderVouchesBinding);
                }
                else
                {
                    channel = CreateChannelWithIssuedToken<ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService");
                }


                //channel = CreateChannelWithIssuedToken<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService");


                // Test
                //System.Threading.Thread.Sleep(25000);
                

                // Use the 'client' variable to call operations on the service.
                getDocumentResponse resp = channel.getDocument(request);

                return resp;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Server exception: " + e.ToString());

                file.WriteLine("Exception: " + e.Message);
                if (e.InnerException != null)
                {
                    file.WriteLine("InnerException: " + e.InnerException.Message);
                }

                channel.Abort();

                throw;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
                if (channel != null)
                {
                    channel.Close();
                }
            }
        }

        public uploadPhotoResponse uploadPhoto(uploadPhotoRequest request)
        {
            ICommercialVehicleCollisionPortTypeChannel channel = null;

            try
            {
                //IClaimsPrincipal principal = Thread.CurrentPrincipal as IClaimsPrincipal;

                //SecurityToken callerToken = principal.Identities[0].BootstrapToken;


                //// We expect only one identity, which will contain the bootstrap token.            
                //if (principal != null && principal.Identities.Count == 1)
                //{
                //    callerToken = principal.Identities[0].BootstrapToken;
                //}

                //ChannelFactory<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel> factory
                //    = new ChannelFactory<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService");
                //factory.ConfigureChannelFactory();

                //// Create and setup channel to talk to the backend service
                //CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel channel = null;

                // Setup the ActAs to point to the caller's token so that we perform a delegated call to the backend service
                // on behalf of the original caller.
                //channel = factory.CreateChannelOnBehalfOf<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel>(callerToken);

                bool useSenderVouchesEndpoint = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSenderVouchesEndpoint"]);

                if (useSenderVouchesEndpoint)
                {
                    channel = CreateChannelWithIssuedToken<ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService-SenderVouches");
                }
                else
                {
                    channel = CreateChannelWithIssuedToken<ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService");
                }

                //channel = CreateChannelWithIssuedToken<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService");

                uploadPhotoResponse resp = channel.uploadPhoto(request);

                // Use the 'client' variable to call operations on the service.
                //string photoId = client.uploadPhoto(request.Photo);
                //uploadPhotoResponse resp = new uploadPhotoResponse(photoId);

                // Always close the client.
                //client.Close();

                return resp;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Server exception: " + ex.ToString());

                channel.Abort();

                throw;
            }
            finally
            {
                channel.Close();
            }
        }

        public downloadDataResponse downloadData(downloadDataRequest request)
        {
            //CommercialVehicleCollisionPortTypeClient client = new CommercialVehicleCollisionPortTypeClient();
            ICommercialVehicleCollisionPortTypeChannel channel = null;

            try
            {
                //IClaimsPrincipal principal = Thread.CurrentPrincipal as IClaimsPrincipal;

                //SecurityToken callerToken = principal.Identities[0].BootstrapToken;

                //// We expect only one identity, which will contain the bootstrap token.            
                //if (principal != null && principal.Identities.Count == 1)
                //{
                //    callerToken = principal.Identities[0].BootstrapToken;
                //}

                //ChannelFactory<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel> factory
                //    = new ChannelFactory<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService");
                //factory.ConfigureChannelFactory();

                //// Create and setup channel to talk to the backend service
                //CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel channel = null;

                // Setup the ActAs to point to the caller's token so that we perform a delegated call to the backend service
                // on behalf of the original caller.
                //channel = factory.CreateChannelOnBehalfOf<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel>(callerToken);


                bool useSenderVouchesEndpoint = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSenderVouchesEndpoint"]);

                if (useSenderVouchesEndpoint)
                {
                    channel = CreateChannelWithIssuedToken<ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService-SenderVouches");
                }
                else
                {
                    channel = CreateChannelWithIssuedToken<ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService");
                }


                //channel = CreateChannelWithIssuedToken<CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel>("CommercialVehicleCollisionBackendService");


                // Use the 'client' variable to call operations on the service.
                //byte[] data = client.downloadData(request.Size);

                downloadDataResponse resp = channel.downloadData(request);

                // Always close the client.
                //client.Close();

                return resp;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Server exception: " + e.ToString());

                channel.Abort();

                throw;
            }
            finally
            {
                channel.Close();
            }
        }

        #endregion
                  
        // Move to utility class
        string RstrToString(RequestSecurityTokenResponse token)
        {
            WSTrust13ResponseSerializer ser = new WSTrust13ResponseSerializer();
            WSTrustSerializationContext context = new WSTrustSerializationContext();
            

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xr = XmlWriter.Create(new StringWriter(stringBuilder), new XmlWriterSettings { OmitXmlDeclaration = true });
            ser.WriteXml(token, xr, context);
            xr.Flush();

            return stringBuilder.ToString();
        }

        string RstToString(RequestSecurityToken token)
        {
            WSTrust13RequestSerializer ser = new WSTrust13RequestSerializer();
            WSTrustSerializationContext context = new WSTrustSerializationContext();


            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xr = XmlWriter.Create(new StringWriter(stringBuilder), new XmlWriterSettings { OmitXmlDeclaration = true });
            ser.WriteXml(token, xr, context);
            xr.Flush();

            return stringBuilder.ToString();
        }


        private ServiceInterface CreateChannelWithIssuedToken<ServiceInterface>(string endpointConfigName)
        {
            IClaimsPrincipal principal = Thread.CurrentPrincipal as IClaimsPrincipal;

            SecurityToken callerToken = principal.Identities[0].BootstrapToken;

            // We expect only one identity, which will contain the bootstrap token.            
            if (principal != null && principal.Identities.Count == 1)
            {
                callerToken = principal.Identities[0].BootstrapToken;
            }

            // ADS address
            string adsAddress = "https://ha50idp:8543/ADS-STS/Issue.svc";

            EndpointAddress stsAddress = new EndpointAddress(adsAddress);

            WSTrustChannelFactory trustChannelFactory = new WSTrustChannelFactory("AssertionDelegateService");
            trustChannelFactory.WSTrustResponseSerializer = new CustomWsTrustResponseSerializer();

            WSTrustChannel channel = (WSTrustChannel)trustChannelFactory.CreateChannel();

            RequestSecurityToken rst = null;
            //RequestSecurityToken rst = new RequestSecurityToken(WSTrust13Constants.RequestTypes.Issue, WSTrust13Constants.KeyTypes.Asymmetric);

            EndpointAddress appliesTo = new EndpointAddress(ConfigurationManager.AppSettings["SenderVouchesAppliesTo"]);

            bool useSenderVouchesEndpoint = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSenderVouchesEndpoint"]);

            if (useSenderVouchesEndpoint)
            {
                // Get key type from Config

                // The beearer key type means a SAML assertion with no proof key
                rst = new RequestSecurityToken(WSTrust13Constants.RequestTypes.Issue, WSTrust13Constants.KeyTypes.Bearer);
                                
                //rst = new RequestSecurityToken(WSTrust13Constants.RequestTypes.Issue, WSTrust13Constants.KeyTypes.Symmetric);

                //rst.KeyType = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/SymmetricKey";

                //rst.KeyType = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/PublicKey";

                //rst.AppliesTo = new EndpointAddress("https://ha50wsp:8553/UserConsumerProvider/CommercialVehicleCollisionPortType-SV.svc");

                // Need to get from config
                //rst.AppliesTo = new EndpointAddress("http://ha50wsp:8081/UserConsumerProvider/CommercialVehicleCollisionPortType-SV.svc");
                rst.AppliesTo = appliesTo;


                // Investigate this
                #region Needed? Seems that this is how I can set the certificate to the Saml 2 handler in the ADS!
                X509Certificate2 wscCertificate = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine,
                    "E=ha50@wsc.net, CN=HA50WSC, O=Hawaii Five0, L=Dallas, S=GA, C=US");
                SecurityKeyIdentifierClause clause = new X509RawDataKeyIdentifierClause(wscCertificate);
                rst.UseKey = new UseKey(new SecurityKeyIdentifier(clause), new X509SecurityToken(wscCertificate));
                #endregion
            }
            else
            {
                rst = new RequestSecurityToken(WSTrust13Constants.RequestTypes.Issue, WSTrust13Constants.KeyTypes.Symmetric);
                rst.KeyType = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/SymmetricKey";
                rst.AppliesTo = appliesTo;
            }

            rst.TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";
                        
            //rst.KeyType = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/PublicKey";
            //rst.AppliesTo = new EndpointAddress("https://ha50wsp:8553/UserConsumerProvider/CommercialVehicleCollisionPortType.svc");
            


            Microsoft.IdentityModel.Tokens.SecurityTokenElement onBehalfOfElement = new Microsoft.IdentityModel.Tokens.SecurityTokenElement(callerToken);
            rst.OnBehalfOf = onBehalfOfElement;

            // TODO: Serialize the token
            WriteRSTokenToFile(rst);
                

            RequestSecurityTokenResponse rstr = null;

            // GFIPM S2S 8.8.2.2 Consumer-Provider SIP conformance
            SecurityToken token = channel.Issue(rst, out rstr);
            SecurityToken tokenInRstr = rstr.RequestedSecurityToken.SecurityToken;

            // DEBUG
            WriteTokenToFile(rstr, token, tokenInRstr);


            // Call the backend service
            ChannelFactory<ServiceInterface> factory = new ChannelFactory<ServiceInterface>(endpointConfigName);
            factory.ConfigureChannelFactory();

            // This behavior can be added in configuration. Will need a behavior conf. element class
            // This should be present during debug
            factory.Endpoint.Behaviors.Add(new CommercialVehicleMessageInspectorBehavior());

            // turn off CardSpace - we already have the token
            factory.Credentials.SupportInteractive = false;

            ServiceInterface serviceChannel = factory.CreateChannelWithIssuedToken<ServiceInterface>(token);


            return serviceChannel;
        }

        private ServiceInterface CreateChannelOnBehalfOf<ServiceInterface>(string endpointConfigName)
        {
            StreamWriter file = new StreamWriter("c:\\temp\\WeatherStationService.Service - CreateChannelOnBehalfOf.txt", true);
            file.WriteLine("_________________________________________");
            file.WriteLine("DateTime: " + DateTime.Now.ToString());

            file.WriteLine("endpointConfigName: " + endpointConfigName);

            IClaimsPrincipal principal = Thread.CurrentPrincipal as IClaimsPrincipal;

            SecurityToken callerToken = principal.Identities[0].BootstrapToken;
            
            // We expect only one identity, which will contain the bootstrap token.            
            if (principal != null && principal.Identities.Count == 1)
            {
                callerToken = principal.Identities[0].BootstrapToken;
            }

            ChannelFactory<ServiceInterface> factory = new ChannelFactory<ServiceInterface>(endpointConfigName);
            factory.ConfigureChannelFactory();

            // turn off CardSpace - we already have the token
            factory.Credentials.SupportInteractive = false;

            if (factory.Endpoint != null)
            {
                if (factory.Endpoint.Binding != null)
                {
                    CustomBinding cb = factory.Endpoint.Binding as CustomBinding;

                    if (cb != null)
                    {
                        if ( cb.Elements != null )
                        {
                            foreach(BindingElement be in cb.Elements)
                            {
                                file.WriteLine("BindingElement: " + be.GetType().ToString());
                            }
                        }
                        else
                        {
                            file.WriteLine("cb.Elements: " + "NULL");
                        }

                    }
                    else
                    {
                        file.WriteLine("factory.Endpoint.CustomBinding: " + "NULL");
                    }
                }
                else
                {
                    file.WriteLine("factory.Endpoint.CustomBinding: " + "NULL");
                }
            }
            else
            {
                file.WriteLine("factory.Endpoint: " + "NULL");
            }


            if (factory.Credentials.IssuedToken != null)
            {
                if ( factory.Credentials.IssuedToken.LocalIssuerAddress != null )
                {
                    file.WriteLine("IssuedToken.LocalIssuerAddress: " + factory.Credentials.IssuedToken.LocalIssuerAddress );
                }
                else
                {
                    file.WriteLine("IssuedToken.LocalIssuerAddress: " + "NULL" );
                }

                if ( factory.Credentials.IssuedToken.IssuerChannelBehaviors != null )
                {
                    file.WriteLine("IssuedToken.IssuerChannelBehaviors Count: " + factory.Credentials.IssuedToken.IssuerChannelBehaviors.Count);
                }
                else
                {
                    file.WriteLine("IssuedToken.IssuerChannelBehaviors: " + "NULL");
                }
            }
            else
            {
                file.WriteLine("IssuedToken: " + "NULL" );
            }

            // Create and setup channel to talk to the backend service
            //CommercialVehicleCollisionWebservice.ICommercialVehicleCollisionPortTypeChannel channel = null;

            // Setup the ActAs to point to the caller's token so that we perform a delegated call to the backend service
            // on behalf of the original caller.
            ServiceInterface channel = factory.CreateChannelOnBehalfOf<ServiceInterface>(callerToken);


            file.Close();

            return channel;
        }

        void WriteTokenToFile(RequestSecurityTokenResponse rstr, SecurityToken token, SecurityToken tokenInRstr)
        {
            StreamWriter file = new StreamWriter("c:\\temp\\WeatherStationService.Service - WriteTokenToFile.txt", true);
            file.WriteLine("_________________________________________");
            file.WriteLine("DateTime: " + DateTime.Now.ToString());

            if (rstr != null)
            {
                file.WriteLine("RSTR:");
                file.WriteLine("AppliesTo: " + rstr.AppliesTo.Uri.ToString());
                file.WriteLine("AuthenticationType: " + rstr.AuthenticationType);

                if (token.Id != null)
                {
                    file.WriteLine("token.Id: " + token.Id);
                }
                else
                {
                    file.WriteLine("token.Id: NULL");
                    Saml2SecurityToken token2 = token as Saml2SecurityToken;

                    if (token2 != null)
                    {
                        if (token2.Id != null)
                        {
                            file.WriteLine("token2.Id: " + token2.Id);
                        }
                        else
                        {
                            file.WriteLine("token2.Id: NULL");
                        }
                    }
                    else
                    {
                        file.WriteLine("token2: NULL");
                    }
                }

                if (tokenInRstr == null)
                {
                    file.WriteLine("tokenInRstr == null");
                }
                else
                {
                    if (tokenInRstr.Id != null)
                    {
                        file.WriteLine("tokenInRstr.Id: " + tokenInRstr.Id);
                        Saml2SecurityToken tokenInRstr2 = tokenInRstr as Saml2SecurityToken;

                        if (tokenInRstr2 != null)
                        {
                            if (tokenInRstr2.Id != null)
                            {
                                file.WriteLine("tokenInRstr2.Id: " + tokenInRstr2.Id);
                            }
                            else
                            {
                                file.WriteLine("tokenInRstr2.Id: NULL");
                            }
                        }
                        else
                        {
                            file.WriteLine("tokenInRstr2: NULL");
                        }
                    }
                    else
                    {
                        file.WriteLine("tokenInRstr.Id == null");
                    }
                }


                file.WriteLine("RSTR Value:");

                file.WriteLine(RstrToString(rstr));
            }


            if (token != null)
            {

                //Saml2SecurityToken st = token as SamlSecurityToken;
                //if ( st != null )
                //{
                //    file.WriteLine("Saml 2 Token: " + st.Assertion.Subject.ToString());
                //}
                //else
                {
                    file.WriteLine("Token: " + token.ToString());
                }
            }
            else
            {
                file.WriteLine("Token: NULL");
            }

            file.Close();
        }

        void WriteRSTokenToFile(RequestSecurityToken rst)
        {
            StreamWriter file = new StreamWriter("c:\\temp\\CommercialVehicle_WSC - WriteRSTokenToFile.txt", true);
            file.WriteLine("_________________________________________");
            file.WriteLine("DateTime: " + DateTime.Now.ToString());

            if (rst != null)
            {
                file.WriteLine("RST:");
                file.WriteLine("AppliesTo: " + rst.AppliesTo.Uri.ToString());
                file.WriteLine("AuthenticationType: " + rst.AuthenticationType);

                file.WriteLine("RST Value:");

                file.WriteLine(RstToString(rst));
            }

            file.Close();
        }

        // This should be called when HTTPS from WSC to WSP is enabled!
        // Move these to COMMON!!! The same pattern is used by all clients!
        bool ServiceCertificateValidator(object sender, X509Certificate cert, X509Chain chain,
                    System.Net.Security.SslPolicyErrors error)
        {
            X509Certificate2 x509Cert = cert as X509Certificate2;

            StreamWriter file = new StreamWriter("c:\\temp\\WeatherStationService.Program - ServiceCertificateValidator.txt", true);
            file.WriteLine("_________________________________________");
            file.WriteLine("DateTime: " + DateTime.Now.ToString());

            file.WriteLine("SubjectName = " + x509Cert.SubjectName.Name);
            file.WriteLine("Error = " + error.ToString());
            file.Close();


            // Check if there were any errors
            if (error == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            
            //string commonName = x509Cert.GetNameInfo(X509NameType.SimpleName, false);

            //if (string.Compare(commonName, "HA50WSP") == 0)
            ////if (string.Compare(cert.Subject, "O=Oceania, C=US, CN=Oceania") == 0)
            //{
            //    return true;
            //}

            return ValidateCertificate(x509Cert);
        }

        private bool ValidateCertificate(X509Certificate2 inputCertificate)
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

        private bool CompareCertificates(X509Certificate2 inputCertificate, X509Certificate2 certificate)
        {
            if (inputCertificate.Thumbprint == certificate.Thumbprint)
            {
                StreamWriter file = new StreamWriter("c:\\temp\\WeatherStationService.Program - GetForecast.txt", true);
                file.WriteLine("_________________________________________");
                file.WriteLine("DateTime: " + DateTime.Now.ToString());

                file.WriteLine(inputCertificate.SubjectName.Name + " == " + certificate.SubjectName.Name);
                file.Close();

                return true;
            }

            return false;
        }      
    }
}
