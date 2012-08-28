

namespace IdentityProviderSts
{
    using System;
    using System.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Tokens;
    using System.Security.Cryptography.X509Certificates;
    using System.IO;
    using System.Diagnostics;
    using Common;

    /// <summary>
    /// IssuerNameRegistry that validates the incoming token in RST.
    /// </summary>
    public class IdentityProviderIssuerNameRegistry : IssuerNameRegistry
    {
        /// <summary>
        /// Overrides the base class. Validates the given issuer token. For a incoming SAML token
        /// the issuer token is the Certificate that signed the SAML token.
        /// </summary>
        /// <param name="securityToken">Issuer token to be validated.</param>
        /// <returns>Friendly name representing the Issuer.</returns>
        public override string GetIssuerName(SecurityToken securityToken)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("IdentityProviderSts.IdentityProviderIssuerNameRegistry.GetIssuerName",
                "MyTraceSource", SourceLevels.Information);

            X509SecurityToken x509Token = securityToken as X509SecurityToken;
            if (x509Token != null)
            {
                // Warning: This sample does a simple compare of the Issuer Certificate
                // to a subject name. This is not appropriate for production use. 
                // Check your validation policy and authenticate issuers based off the policy.
                
                string commonName = x509Token.Certificate.GetNameInfo(X509NameType.SimpleName, false);

                ts.TraceInformation("Certificate CN: " + commonName);

                //if (String.Equals(x509Token.Certificate.SubjectName.Name, "O=CA for Ref GFIPM, E=ca@gfipm.net, C=US, S=GA, CN=Reference GFIPM Federation") ||
                //    String.Equals(x509Token.Certificate.SubjectName.Name, "O=CISA, C=US, S=GA, CN=cisaidp.swbs.gtri.gatech.edu"))
                //if (String.Equals(x509Token.Certificate.SubjectName.Name, "O=CISA, C=US, S=GA, CN=cisaidp.swbs.gtri.gatech.edu"))
                if (String.Equals(commonName.ToUpper(), "HA50IDP"))
                {
                    return x509Token.Certificate.SubjectName.Name;
                }
            }

            ts.TraceInformation("Untrusted issuer");

            throw new SecurityTokenException("Untrusted issuer.");
        }
    }
}
