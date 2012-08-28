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
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;

using Common;

namespace WebServiceProviderHost
{
    class PolicyReader
    {
        static Expression<Func<AuthorizationContext, bool>> DefaultPolicy = (icp) => false;
        delegate bool HasClaimDelegate(AuthorizationContext p, string claimType, string claimValue);

        /// <summary>
        /// Delegate that checks if any ClaimsIdentity associated with the given principal has the claim specified
        /// </summary>
        static HasClaimDelegate HasClaim = delegate(AuthorizationContext p, string claimType, string claimValue)
        {
            return p.ClaimSets.Any(s =>
                s.ContainsClaim(new Claim(claimType, claimValue, Rights.PossessProperty)) );
        };

      
        /// <summary>
        /// Creates an instance of the policy reader
        /// </summary>
        public PolicyReader()
        {
        }

        /// <summary>
        /// Read the policy as a LINQ expression
        /// </summary>
        /// <param name="reader">XmlDictionaryReader for the policy Xml</param>
        /// <returns></returns>
        public Expression<Func<AuthorizationContext, bool>> ReadPolicy(XmlDictionaryReader reader)
        {
            if (reader.Name != "policy")
            {
                throw new InvalidOperationException("Invalid policy document");
            }

            reader.Read();

            // Instantiate a parameter for the IClaimsPrincipal so it can be evaluated against
            // each claim.
            ParameterExpression subject = Expression.Parameter(typeof(AuthorizationContext), "subject");

            Expression<Func<AuthorizationContext, bool>> result = ReadNode(reader, subject);
            reader.ReadEndElement();

            return result;
        }

        /// <summary>
        /// Read the policy node
        /// </summary>
        /// <param name="reader">XmlDictionaryReader for the policy Xml</param>
        /// <param name="subject">IClaimsPrincipal subject</param>
        /// <returns>A LINQ expression created from the policy</returns>
        private Expression<Func<AuthorizationContext, bool>> ReadNode(XmlDictionaryReader reader, ParameterExpression subject)
        {
            Expression<Func<AuthorizationContext, bool>> policyExpression;

            if (!reader.IsStartElement())
            {
                throw new InvalidOperationException("Invalid Policy format.");
            }

            switch (reader.Name)
            {
                case "and":
                    policyExpression = ReadAnd(reader, subject);
                    break;
                case "or":
                    policyExpression = ReadOr(reader, subject);
                    break;
                case "claim":
                    policyExpression = ReadClaim(reader);
                    break;
                default:
                    policyExpression = DefaultPolicy;
                    break;
            }

            return policyExpression;
        }

        /// <summary>
        /// Read the claim node
        /// </summary>
        /// <param name="reader">XmlDictionaryReader of the policy Xml</param>
        /// <returns>A LINQ expression created from the claim node</returns>
        private Expression<Func<AuthorizationContext, bool>> ReadClaim(XmlDictionaryReader reader)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.PolicyReader.ReadClaim",
                "MyTraceSource", SourceLevels.Information);

            string claimType = reader.GetAttribute("claimType");
            string claimValue = reader.GetAttribute("claimValue");
            string claimValueType = reader.GetAttribute("claimValueType");
            string claimValueOperation = reader.GetAttribute("claimValueOperation");

            ts.TraceInformation("Claim: <{0}, {1}, {2}, {3}>", claimType, claimValue, claimValueType, claimValueOperation);

            Expression<Func<AuthorizationContext, bool>> hasClaim = (icp) => HasClaim(icp, claimType, claimValue);

            reader.Read();

            return hasClaim;
        }

        /// <summary>
        /// Read the Or Node
        /// </summary>
        /// <param name="reader">XmlDictionaryReader of the policy Xml</param>
        /// <param name="subject">IClaimsPrincipal subject</param>
        /// <returns>A LINQ expression created from the Or node</returns>
        private Expression<Func<AuthorizationContext, bool>> ReadOr(XmlDictionaryReader reader, ParameterExpression subject)
        {
            reader.Read();

            BinaryExpression lambda1 = Expression.OrElse(
                    Expression.Invoke(ReadNode(reader, subject), subject),
                    Expression.Invoke(ReadNode(reader, subject), subject));

            reader.ReadEndElement();

            Expression<Func<AuthorizationContext, bool>> lambda2 = Expression.Lambda<Func<AuthorizationContext, bool>>(lambda1, subject);
            return lambda2;
        }

        /// <summary>
        /// Read the And Node
        /// </summary>
        /// <param name="reader">XmlDictionaryReader of the policy Xml</param>
        /// <param name="subject">IClaimsPrincipal subject</param>
        /// <returns>A LINQ expression created from the And node</returns>
        private Expression<Func<AuthorizationContext, bool>> ReadAnd(XmlDictionaryReader reader, ParameterExpression subject)
        {
            reader.Read();

            BinaryExpression lambda1 = Expression.AndAlso(
                    Expression.Invoke(ReadNode(reader, subject), subject),
                    Expression.Invoke(ReadNode(reader, subject), subject));

            reader.ReadEndElement();

            Expression<Func<AuthorizationContext, bool>> lambda2 = Expression.Lambda<Func<AuthorizationContext, bool>>(lambda1, subject);
            return lambda2;
        }
    }
}
