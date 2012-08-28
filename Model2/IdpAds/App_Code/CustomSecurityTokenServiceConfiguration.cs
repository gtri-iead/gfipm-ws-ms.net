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

namespace IdpAds
{
    using System;
    using Microsoft.IdentityModel.Configuration;
    using System.Diagnostics;

    /// <summary>
    /// Summary description for CustomSecurityTokenServiceConfiguration
    /// </summary>
    class CustomSecurityTokenServiceConfiguration : SecurityTokenServiceConfiguration
    {
        /// <summary>
        /// Creates an instance of CustomSecurityTokenServiceConfiguration.
        /// </summary>
        public CustomSecurityTokenServiceConfiguration(string idp) : base(idp)
        {
            CustomAdsTextTraceSource ts = new CustomAdsTextTraceSource("IdpAds.CustomSecurityTokenServiceConfiguration.CustomSecurityTokenServiceConfiguration",
               "AdsTraceSource", SourceLevels.Information);

            SecurityTokenService = typeof(CustomSecurityTokenService);

            // GFIPM S2S 8.8.2.6.e Modify the "NotOnOrAfter"
            this.DefaultTokenLifetime = TimeSpan.FromSeconds(Constants.AdsAssertionValidityPeriod);

            ts.TraceInformation("DefaultTokenLifetime: " + this.DefaultTokenLifetime.TotalSeconds.ToString());
        }
    }
}
