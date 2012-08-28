using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Tokens.Saml11;


namespace CommercialVehicleCollisionWebservice
{
    public class CustomSaml20SecurityTokenHandler : Saml2SecurityTokenHandler
    {
        #region Private Data
        string _gfipmEntityIdClaimType = "gfipm:2.0:entity:EntityId";
        string _issuer = "";
        #endregion

        #region CTOR
        public CustomSaml20SecurityTokenHandler()
        {
        }
        #endregion

        #region Overrides

        public override ClaimsIdentityCollection ValidateToken(SecurityToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            var samlToken = token as Saml2SecurityToken;
            if (samlToken == null)
            {
                throw new ArgumentException("token");
            }
            if (samlToken.Assertion == null)
            {
                throw new ArgumentException("token");
            }

            var assertion = samlToken.Assertion as Saml2Assertion;
            this.ValidateConditions(samlToken.Assertion.Conditions, false);

            // extract claims from token
            var identity = new ClaimsIdentity("GfipmEntity");
            ProcessStatement(assertion.Statements, identity, "Client");

            // call authentication and filtering logic
            IClaimsIdentity newIdentity;

            try
            {
                if (ValidateUser(identity, out newIdentity))
                {
                    return new ClaimsIdentityCollection(new IClaimsIdentity[] { newIdentity });
                }
                else
                {
                    throw new SecurityTokenValidationException("Authentication failed");
                }
            }
            catch (Exception ex)
            {
                throw new SecurityTokenValidationException("Security token validation failed", ex);
            }
        }

        public SamlSecurityToken CreateToken(IClaimsIdentity identity)
        {
            var description = new SecurityTokenDescriptor 
            {
                Subject = identity,
                TokenIssuerName = _issuer
            };

            //var handler = new Saml2SecurityTokenHandler();
            //return (SamlSecurityToken)handler.CreateToken(description);
            return (SamlSecurityToken)CreateToken(description);
        }

        #endregion

        // sample implementation - do not use for production ;)
        protected bool ValidateUser(ClaimsIdentity id, out IClaimsIdentity newIdentity)
        {
            newIdentity = null;
            var gfipmEntityIdClaim = id.Claims.First(c => c.ClaimType == _gfipmEntityIdClaimType);

            if (gfipmEntityIdClaim != null)
            {
                newIdentity = new ClaimsIdentity(new Claim[] 
                {
                    gfipmEntityIdClaim
                }, "GfipmEntity");

                return true;
            }

            return false;
        }
    }
}
