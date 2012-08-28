using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace GfipmCryptographicTrustFabricSerializer
{
    public class EmptySecurityTokenResolver
    {
        // Fields
        private static readonly SecurityTokenResolver _instance = SecurityTokenResolver.CreateDefaultSecurityTokenResolver(
            EmptyReadOnlyCollection<SecurityToken>.Instance, false);

        // Properties
        public static SecurityTokenResolver Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
