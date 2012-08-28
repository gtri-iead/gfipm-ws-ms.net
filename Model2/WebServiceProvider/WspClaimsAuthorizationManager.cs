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
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using Microsoft.IdentityModel.Claims;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Common;

namespace CommercialVehicleCollisionWebServiceProvider
{
    /// <summary>
    /// Simple ClaimsAuthorizationManager implementation that reads policy information from the .config file
    /// </summary>
    public class WspClaimsAuthorizationManager : ClaimsAuthorizationManager
    {
        private Dictionary<ResourceAction, Func<IClaimsPrincipal, bool>> _authzPolicies = null;
            //= new Dictionary<ResourceAction, Func<IClaimsPrincipal, bool>>();
        private AuthorizationPolicyLoader _authzPolicyLoader = new AuthorizationPolicyLoader();
                
        public WspClaimsAuthorizationManager()
        {
            Initialize();
        }

        /// <summary>
        /// Creates a new instance of the WspClaimsAuthorizationManager
        /// </summary>
        /// <param name="nodes">XmlNodeList containing the policy information 
        /// loaded from the config file</param>
        public WspClaimsAuthorizationManager(XmlNodeList nodes)
        {
            Initialize();

            if (nodes != null && nodes.Count > 0)
            {
               LoadPolicies(nodes);
            }
        }

        private void Initialize()
        {
            _authzPolicies = new Dictionary<ResourceAction, Func<IClaimsPrincipal, bool>>();
        }

        private XElement GetXElement(XmlNode node)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);
            return xDoc.Root;
        }

        /// <summary>
        /// Loads the policies from config file
        /// </summary>
        private void LoadPolicies(XmlNodeList nodes)
        {
            Expression<Func<IClaimsPrincipal, bool>> policyExpression;

            foreach (XmlNode node in nodes)
            {
                if (node == null || node.OuterXml.Length == 0)
                {
                    continue;
                }

                XElement elem = GetXElement(node);

                // Initialize the policy store
                XmlDictionaryReader reader = XmlDictionaryReader.CreateDictionaryReader(new XmlTextReader(new StringReader(node.OuterXml)));
                reader.MoveToContent();

                string resource = reader.GetAttribute("resource");
                string action = reader.GetAttribute("action");

                policyExpression = _authzPolicyLoader.LoadPolicy(reader);

                // Compile the policy expression into a function
                Func<IClaimsPrincipal, bool> policy = policyExpression.Compile();

                // Insert the policy function into the policy cache
                _authzPolicies[new ResourceAction(resource, action)] = policy;
            }
        }

        #region ClaimsAuthorizationManager Members

        /// <summary>
        /// Checks if the principal specified in the authorization context is authorized to perform action specified in the authorization context 
        /// on the specified resoure
        /// </summary>
        /// <param name="pec">Authorization context</param>
        /// <returns>true if authorized, false otherwise</returns>
        public override bool CheckAccess(AuthorizationContext authorizationContext)
        {
            bool grantAccess = false;

            ResourceAction ra = new ResourceAction(authorizationContext.Resource.First<Claim>().Value,
                authorizationContext.Action.First<Claim>().Value);
            if (_authzPolicies.ContainsKey(ra))
            {
                grantAccess = _authzPolicies[ra](authorizationContext.Principal);
            }

            return grantAccess;
        }
    
        #endregion
    }
}
