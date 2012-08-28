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
using System.Text;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
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
using CommercialVehicleCollisionClientProxy;
using WebServiceConsumerProxy;

namespace WebServiceConsumer
{
    [ServiceBehavior(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0")]
    public class WebServiceConsumer : ICommercialVehicleCollisionPortType
    {
        Common.SslServerCertificateValidator _sslServerCertValidator = null;

        public WebServiceConsumer()
        {
            _sslServerCertValidator = new SslServerCertificateValidator();
        }

        #region ICommercialVehicleCollisionPortType Members

        public getDocumentResponse getDocument(getDocumentRequest request)
        {

            Common.CustomTextTraceSource ts = new Common.CustomTextTraceSource("WebServiceConsumer.WebServiceConsumer.getDocument",
                "MyTraceSource", SourceLevels.Information);

            ICommercialVehicleCollisionPortTypeChannel channel = GetClientProxyChannel();
            try
            {            
                // Use the 'client' variable to call operations on the service.
                getDocumentResponse resp = channel.getDocument(request);

                channel.Close();

                return resp;
            }
            catch (FaultException)
            {
                channel.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                channel.Abort();
                throw;
            }
            catch (Exception e)
            {
                ts.TraceInformation("Exception: " + e.Message);
                if (e.InnerException != null)
                {
                    ts.TraceInformation("InnerException: " + e.InnerException.Message);
                }

                channel.Abort();

                throw;
            }
        }

        public uploadPhotoResponse uploadPhoto(uploadPhotoRequest request)
        {
            Common.CustomTextTraceSource ts = new Common.CustomTextTraceSource("WebServiceConsumer.WebServiceConsumer.uploadPhoto",
                "MyTraceSource", SourceLevels.Information);

            ICommercialVehicleCollisionPortTypeChannel channel = GetClientProxyChannel();

            try
            {
                uploadPhotoResponse resp = channel.uploadPhoto(request);

                channel.Close();

                return resp;
            }
            catch (FaultException)
            {
                channel.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                channel.Abort();
                throw;
            }
            catch (Exception e)
            {
                ts.TraceInformation("Exception: " + e.Message);
                if (e.InnerException != null)
                {
                    ts.TraceInformation("InnerException: " + e.InnerException.Message);
                }

                channel.Abort();

                throw;
            }
        }

        public downloadDataResponse downloadData(downloadDataRequest request)
        {
            Common.CustomTextTraceSource ts = new Common.CustomTextTraceSource("WebServiceConsumer.WebServiceConsumer.downloadData",
                   "MyTraceSource", SourceLevels.Information);

            ICommercialVehicleCollisionPortTypeChannel channel = GetClientProxyChannel();

            try
            {
                downloadDataResponse resp = channel.downloadData(request);

                channel.Close();

                return resp;
            }
            catch (FaultException)
            {
                channel.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                channel.Abort();
                throw;
            }
            catch (Exception e)
            {
                ts.TraceInformation("Exception: " + e.Message);
                if (e.InnerException != null)
                {
                    ts.TraceInformation("InnerException: " + e.InnerException.Message);
                }

                channel.Abort();

                throw;
            }
        }

        #endregion

        private ICommercialVehicleCollisionPortTypeChannel GetClientProxyChannel()
        {
            string wspEndpoint = ConfigurationManager.AppSettings["WspEndpointName"];
            
            WebServiceConsumerProxy<ICommercialVehicleCollisionPortTypeChannel> wscProxy 
                = new WebServiceConsumerProxy<ICommercialVehicleCollisionPortTypeChannel>();
            
            SecurityToken callerToken = GetCallerToken();
            string appliesTo = ConfigurationManager.AppSettings["WspAppliesTo"];
            
            return wscProxy.CreateChannelWithIssuedToken(callerToken, appliesTo, wspEndpoint);
        }

        private ICommercialVehicleCollisionPortType GetClientChannel()
        {
            string wspEndpoint = ConfigurationManager.AppSettings["WspEndpointName"];

            WebServiceConsumerProxy<ICommercialVehicleCollisionPortType> wscProxy
                = new WebServiceConsumerProxy<ICommercialVehicleCollisionPortType>();

            SecurityToken callerToken = GetCallerToken();
            string appliesTo = ConfigurationManager.AppSettings["WspAppliesTo"];

            return wscProxy.CreateChannelWithIssuedToken(callerToken, appliesTo, wspEndpoint);
        }

        private SecurityToken GetCallerToken()
        {
            IClaimsPrincipal principal = Thread.CurrentPrincipal as IClaimsPrincipal;

            SecurityToken callerToken = principal.Identities[0].BootstrapToken;

            return callerToken;
        }

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
    }
}
