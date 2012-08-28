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

namespace IdpSts
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Web.Configuration;
    using Microsoft.IdentityModel.Configuration;
    using Microsoft.IdentityModel.SecurityTokenService;
    using System.IO;
    using Common;

    /// <summary>
    /// A custom SecurityTokenServiceConfiguration implementation.
    /// </summary>
    public class CustomSecurityTokenServiceConfiguration : SecurityTokenServiceConfiguration
    {
        /// <summary>
        /// CustomSecurityTokenServiceConfiguration constructor.
        /// </summary>
        public CustomSecurityTokenServiceConfiguration() 
        {
            this.TokenIssuerName = WebConfigurationManager.AppSettings[CommonConfigurationSettings.IssuerName];

            this.SecurityTokenService = typeof(CustomSecurityTokenService);
        }
    }
}
