using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Common
{
    public class WrappedSaml2AssertionKeyIdentifierClause : SamlAssertionKeyIdentifierClause
    {
        // Fields
        private Saml2AssertionKeyIdentifierClause _clause;

        // Methods
        public WrappedSaml2AssertionKeyIdentifierClause(Saml2AssertionKeyIdentifierClause clause) : base(clause.Id)
        {
            this._clause = clause;
        }

        public override SecurityKey CreateKey()
        {
            return this._clause.CreateKey();
        }

        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            return this._clause.Matches(keyIdentifierClause);
        }

        // Properties
        public override bool CanCreateKey
        {
            get
            {
                return this._clause.CanCreateKey;
            }
        }

        public Saml2AssertionKeyIdentifierClause WrappedClause
        {
            get
            {
                return this._clause;
            }
        }
    }

}
