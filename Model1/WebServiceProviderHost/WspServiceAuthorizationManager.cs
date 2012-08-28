//
// Copyright (c) 2012, Georgia Institute of Technology. All Rights Reserved.
// This code was developed by Georgia Tech Research Institute (GTRI) under
// a grant from the U.S. Dept. of Justice, Bureau of Justice Assistance.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;

using Common;

namespace WebServiceProviderHost
{
    public class WspServiceAuthorizationManager : ServiceAuthorizationManager
    {
        Dictionary<string, Func<AuthorizationContext, bool>> _policies = new Dictionary<string, Func<AuthorizationContext, bool>>();
        PolicyReader _policyReader = new PolicyReader();
        
        public WspServiceAuthorizationManager() : base()
        {
            LoadAuthorizationPolicies();
        }
        
        /// <summary>
        /// Loads the policies from policies store
        /// </summary>
        void LoadAuthorizationPolicies()
        {
            Expression<Func<AuthorizationContext, bool>> policyExpression;

            CustomTextTraceSource ts = new CustomTextTraceSource("WebServiceProviderHost.WspServiceAuthorizationManager.LoadAuthorizationPolicies",
                "MyTraceSource", SourceLevels.Information);

            IEnumerable<XElement> nodes = GetWspAuthorizationPolicy();

            ts.TraceInformation("IEnumerable<XElement> Node Count: {0}", nodes.Count() );

            foreach (XElement xElem in nodes)
            {
                // Initialize the policy store
                XmlDictionaryReader reader = XmlDictionaryReader.CreateDictionaryReader(new XmlTextReader(new StringReader(xElem.ToString())));
                reader.MoveToContent();

                string action = reader.GetAttribute("action");

                ts.TraceInformation("Node Action: {0}", action);

                policyExpression = _policyReader.ReadPolicy(reader);

                // Compile the policy expression into a function
                Func<AuthorizationContext, bool> policy = policyExpression.Compile();
                                
                // Insert the policy function into the policy cache
                _policies[action] = policy;
            }
        }
                
        private IEnumerable<XElement> GetWspAuthorizationPolicy()
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("WebServiceProviderHost.WspServiceAuthorizationManager.GetWspAuthorizationPolicy",
               "MyTraceSource", SourceLevels.Information);

            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebServiceProviderHost.WspAuthorizationPolicyStore.xml");

                XElement root = XElement.Load(stream);

                return root.Elements();
            }
            catch (Exception e)
            {
                ts.TraceInformation("Error: " + e.Message);
                if (e.InnerException != null)
                {
                    ts.TraceInformation("\tInner Error: " + e.InnerException.Message);
                }

                throw;
            }
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("WebServiceProviderHost.WspServiceAuthorizationManager.CheckAccessCore",
                "MyTraceSource", SourceLevels.Information);

            bool grantAccess = false;

            // Extract the action URI from the OperationContext. Match this against the claims
            // in the AuthorizationContext.
            string action = operationContext.RequestContext.RequestMessage.Headers.Action;

            AuthorizationContext context = operationContext.ServiceSecurityContext.AuthorizationContext;

            ts.TraceInformation("Primary Identity: " + operationContext.ServiceSecurityContext.PrimaryIdentity.Name);
            ts.TraceInformation("Action: " + action);

            if (_policies.ContainsKey(action))
            {
                ts.TraceInformation("Action available");

                grantAccess = _policies[action](context);
            }
            
            return grantAccess;
        }
    }
}
