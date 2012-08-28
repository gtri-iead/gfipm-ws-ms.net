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
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Diagnostics;

using Common;


namespace IdpSts
{
    class CustomUserNameSecurityTokenHandler : UserNameSecurityTokenHandler
    {
        public override bool CanValidateToken
        {
            get
            {
                return true;
            }
        }

        public override ClaimsIdentityCollection ValidateToken(SecurityToken token)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("IdpSts.CustomUserNameSecurityTokenHandler.ValidateToken",
              "MyTraceSource", SourceLevels.Information);

            //StackTracer.TraceStack(ts);

            UserNameSecurityToken usernameToken = token as UserNameSecurityToken;
            if (usernameToken == null)
            {
                throw new ArgumentException("usernameToken", "The security token is not a valid username security token.");
            }

            // will throw if fails
            UsernameCredentialStore.AuthenticateUser(usernameToken.UserName, usernameToken.Password);
            
            ClaimsIdentityCollection identities = new ClaimsIdentityCollection();

            IClaimsIdentity claimsIdentity = new ClaimsIdentity("CustomUserNameSecurityTokenHandler");
            claimsIdentity.Claims.Add(new Claim(System.IdentityModel.Claims.ClaimTypes.Name, usernameToken.UserName));

            identities.Add(claimsIdentity);

            return identities;                      
        }
    }
}
