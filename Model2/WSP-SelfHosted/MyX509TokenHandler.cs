using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using Common;

namespace CommercialVehicleCollisionWebservice
{
    public class MyX509TokenHandler : X509SecurityTokenHandler
    {
        public MyX509TokenHandler()
            : base(new Common.CustomX509CertificateValidator())
        {
        }
    }
}
