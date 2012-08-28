using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;
using System.IdentityModel.Tokens;

namespace WSP_SelfHosted
{
    public class WspCustomSecuredBinding : Binding
    {
        private bool _httpsTransport = true;
        private SecurityBindingElement _securityBindingElement = null;

        public SecurityBindingElement SecurityBindingElement
        {
            get { return _securityBindingElement; }
            //set { _securityBindingElement = value; }
        }

        private ReliableSessionBindingElement _reliableSessionBindingElement = null;
        private MtomMessageEncodingBindingElement _mtomEncodingBindingElement = null;
        private HttpTransportBindingElement _httpTransportBindingElement = null;
        private HttpsTransportBindingElement _httpsTransportBindingElement = null;
        
        public WspCustomSecuredBinding()
        {
            Initialize();
        }

        public WspCustomSecuredBinding(bool httpsTransport)
        {
            _httpsTransport = httpsTransport;

            Initialize();
        }


        private void Initialize()
        {
            _reliableSessionBindingElement = CreateReliableSessionBindingElement();

            _securityBindingElement = CreateSecurityBindingElement();
                        
            _mtomEncodingBindingElement = CreateMtomEncodingBindingElement();
                        
            _httpsTransportBindingElement = CreateHttpsTransportBindingElement() as HttpsTransportBindingElement;

            _httpTransportBindingElement = CreateHttpTransportBindingElement() as HttpTransportBindingElement;
        }

        private MtomMessageEncodingBindingElement CreateMtomEncodingBindingElement()
        {
            MtomMessageEncodingBindingElement mtomEncodingBindingElement = new MtomMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8);
            //mtomEncodingBindingElement.MaxBufferSize = 20000000;
            //mtomEncodingBindingElement.MaxReadPoolSize = 20000000;
            //mtomEncodingBindingElement.MaxWritePoolSize = 20000000;

            //mtomEncodingBindingElement.ReaderQuotas.MaxStringContentLength = 200000000;
            //mtomEncodingBindingElement.ReaderQuotas.MaxArrayLength = 200000000;
            //mtomEncodingBindingElement.ReaderQuotas.MaxBytesPerRead = 200000000;
            //mtomEncodingBindingElement.ReaderQuotas.MaxDepth = 200000000;
            //mtomEncodingBindingElement.ReaderQuotas.MaxNameTableCharCount = 200000000;

            return mtomEncodingBindingElement;
        }

        private ReliableSessionBindingElement CreateReliableSessionBindingElement()
        {
            // create and configure reliable session
            ReliableSessionBindingElement reliableSession = new ReliableSessionBindingElement();
            reliableSession.Ordered = true;

            return reliableSession;
        }


        private TransportBindingElement CreateHttpsTransportBindingElement()
        {
            HttpsTransportBindingElement transportBindingElement = new HttpsTransportBindingElement();

            // When set to true, the IIS Site application must have the SSL require certificate set
            transportBindingElement.RequireClientCertificate = false;

            transportBindingElement.MaxBufferSize = 524288;
            transportBindingElement.MaxReceivedMessageSize = 200000000;
            transportBindingElement.MaxBufferSize = 200000000;

            return transportBindingElement;
        }

        private TransportBindingElement CreateHttpTransportBindingElement()
        {
            HttpTransportBindingElement transportBindingElement = new HttpTransportBindingElement();

            // When set to true, the IIS Site application must have the SSL require certificate set
            //transportBindingElement.RequireClientCertificate = false;

            transportBindingElement.MaxBufferSize = 200000000;
            transportBindingElement.MaxReceivedMessageSize = 200000000;
            transportBindingElement.MaxBufferSize = 200000000;

            return transportBindingElement;
        }

        private TransportSecurityBindingElement CreateTransportBindingElement()
        {
            TransportSecurityBindingElement transportSecurityBindingElement = new TransportSecurityBindingElement();

            transportSecurityBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256Sha256;
            transportSecurityBindingElement.IncludeTimestamp = true;
            transportSecurityBindingElement.KeyEntropyMode = SecurityKeyEntropyMode.CombinedEntropy;
            transportSecurityBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12;
            transportSecurityBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Lax;

            return transportSecurityBindingElement;
        }

        private SecurityBindingElement CreateWSS11SecurityBindingElement()
        {
            AsymmetricSecurityBindingElement secBindingElement = new AsymmetricSecurityBindingElement();

            secBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Lax;

            // TEST
            //secBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256Rsa15;
            secBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;

            secBindingElement.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncrypt;
            secBindingElement.IncludeTimestamp = true;
            secBindingElement.SetKeyDerivation(false);
            secBindingElement.AllowSerializedSigningTokenOnReply = true;
            secBindingElement.RequireSignatureConfirmation = false;

            //WS2007HttpBinding stsBinding = new WS2007HttpBinding("wssuntBinding");
            //CustomBinding stsBinding = new CustomBinding("ADS-CustomSecureTransport");

            // TEMPORARILY DISABLED

            // .Net 3.5
            //string adsAddress = "http://ha50idp:8089/ADS-STS/Issue.svc";

            // .Net 4.0
            string adsAddress = "https://ha50idp:8543/ADS-STS/Issue.svc";

            //IssuedSecurityTokenParameters issuedTokenParameters = new IssuedSecurityTokenParameters("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",
            //    new EndpointAddress(adsAddress), stsBinding);

            IssuedSecurityTokenParameters issuedTokenParameters = new IssuedSecurityTokenParameters("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0");


            issuedTokenParameters.UseStrTransform = false;

            issuedTokenParameters.KeyType = SecurityKeyType.BearerKey;
            //issuedTokenParameters.KeyType = SecurityKeyType.AsymmetricKey;
            //issuedTokenParameters.KeyType = SecurityKeyType.SymmetricKey;

            // 256?
            //issuedTokenParameters.KeySize = 256;
            issuedTokenParameters.KeySize = 0;

            // .Net 3.5
            //string adsMexAddress = "http://ha50idp:8089/ADS-STS/Issue.svc/mex";

            // .Net 4.0
            //string adsMexAddress = "https://ha50idp:8543/ADS-STS/Issue.svc/mex";


            //issuedTokenParameters.IssuerMetadataAddress = new EndpointAddress(adsMexAddress);
            issuedTokenParameters.RequireDerivedKeys = false;
            issuedTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
            issuedTokenParameters.ReferenceStyle = SecurityTokenReferenceStyle.Internal;
            //issuedTokenParameters.ReferenceStyle = SecurityTokenReferenceStyle.External;

            // Claims
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:SurName"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:GivenName"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:EmailAddressText"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:TelephoneNumber"));
            //issuedTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement("gfipm:2.0:user:FederationId"));


            // THis is a test
            //secBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Add(issuedTokenParameters);

            // This is the right one
            secBindingElement.EndpointSupportingTokenParameters.Signed.Add(issuedTokenParameters);

            //secBindingElement.EndpointSupportingTokenParameters.Endorsing.Add(issuedTokenParameters);
            
            // need to put this in configuration

            //X509KeyIdentifierClauseType keyIdClauseType = X509KeyIdentifierClauseType.Any;


            X509KeyIdentifierClauseType keyIdClauseType = X509KeyIdentifierClauseType.SubjectKeyIdentifier;

            //X509KeyIdentifierClauseType keyIdClauseType = X509KeyIdentifierClauseType.IssuerSerial;

            //X509KeyIdentifierClauseType keyIdClauseType = X509KeyIdentifierClauseType.Thumbprint;

            X509SecurityTokenParameters initiatorTokenParameters = new X509SecurityTokenParameters(keyIdClauseType,
                SecurityTokenInclusionMode.AlwaysToRecipient);
            initiatorTokenParameters.RequireDerivedKeys = false;
            initiatorTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;

            initiatorTokenParameters.ReferenceStyle = SecurityTokenReferenceStyle.External;
            //initiatorTokenParameters.ReferenceStyle = (SecurityTokenReferenceStyle)X509KeyIdentifierClauseType.RawDataKeyIdentifier;
            secBindingElement.InitiatorTokenParameters = initiatorTokenParameters;

            X509SecurityTokenParameters recipientTokenParameters = new X509SecurityTokenParameters(keyIdClauseType,
                SecurityTokenInclusionMode.Never);
            recipientTokenParameters.RequireDerivedKeys = false;
            recipientTokenParameters.InclusionMode = SecurityTokenInclusionMode.Never;
            recipientTokenParameters.ReferenceStyle = SecurityTokenReferenceStyle.External;
            secBindingElement.RecipientTokenParameters = recipientTokenParameters;


            secBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12;

            return secBindingElement;
        }

        private SecurityBindingElement CreateWSS10SecurityBindingElement()
        {
            AsymmetricSecurityBindingElement secBindingElement = new AsymmetricSecurityBindingElement();

            secBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Lax;

            secBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            secBindingElement.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncrypt;
            secBindingElement.IncludeTimestamp = true;
            secBindingElement.SetKeyDerivation(false);
            secBindingElement.AllowSerializedSigningTokenOnReply = true;
            secBindingElement.RequireSignatureConfirmation = false;

            X509SecurityTokenParameters initiatorTokenParameters = new X509SecurityTokenParameters(X509KeyIdentifierClauseType.IssuerSerial,
                SecurityTokenInclusionMode.AlwaysToRecipient);
            initiatorTokenParameters.RequireDerivedKeys = false;
            initiatorTokenParameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
            secBindingElement.InitiatorTokenParameters = initiatorTokenParameters;

            X509SecurityTokenParameters recipientTokenParameters = new X509SecurityTokenParameters(X509KeyIdentifierClauseType.IssuerSerial,
                SecurityTokenInclusionMode.Never);
            recipientTokenParameters.RequireDerivedKeys = false;
            recipientTokenParameters.InclusionMode = SecurityTokenInclusionMode.Never;
            secBindingElement.RecipientTokenParameters = recipientTokenParameters;

            //secBindingElement.EndpointSupportingTokenParameters.Signed.Add(issuedTokenParameters);
            //secBindingElement.EndpointSupportingTokenParameters.Endorsing.Add(protectTokenParameters);

            //secBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12;
            secBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;

            return secBindingElement;
        }

        private SecurityBindingElement CreateSecurityBindingElement()
        {
            return CreateWSS11SecurityBindingElement();
        }

        public override string Scheme
        {
            get 
            { 
                
                if (_httpsTransport)
                {
                    return this._httpsTransportBindingElement.Scheme;
                }
                else
                {
                    return this._httpTransportBindingElement.Scheme;
                }
            }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();

            // comment out to disable ReliableMessaging
            //elements.Add(this._reliableSessionBindingElement);


            elements.Add(this._securityBindingElement);
            //elements.Add(SecurityBindingElement.CreateMutualCertificateBindingElement());
            
            // new security transport binding element
            //elements.Add(CreateTransportBindingElement());

            elements.Add(this._mtomEncodingBindingElement);

            if (_httpsTransport)
            {
                elements.Add(this._httpsTransportBindingElement);
            }
            else
            {
                elements.Add(this._httpTransportBindingElement);
            }
            
            return elements;
        }
    }
}
