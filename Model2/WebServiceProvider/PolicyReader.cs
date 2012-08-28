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
using System.Xml.Linq;
using Microsoft.IdentityModel.Claims;
using Common;

namespace CommercialVehicleCollisionWebServiceProvider
{
    class AuthorizationPolicyLoader
    {
        Expression<Func<IClaimsPrincipal, bool>> DefaultPolicy = (icp) => false;
        delegate bool EqualClaimDelegate(IClaimsPrincipal p, string claimType, string claimValue);
        delegate bool LessThanOrEqualClaimDelegate(IClaimsPrincipal p, string claimType, string claimValue, string claimValueType);

        /// <summary>
        /// Delegate that checks if any ClaimsIdentity associated with the given principal has the claim specified
        /// </summary>
        EqualClaimDelegate ClaimEquals = delegate(IClaimsPrincipal p, string claimType, string claimValue)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.PolicyReader.EqualClaimDelegate",
               "MyTraceSource", SourceLevels.Information);

            ts.TraceInformation("claimType: {0}", claimType);
            ts.TraceInformation("claimValue: {0}", claimValue);

            foreach (IClaimsIdentity ci in p.Identities)
            {
                ts.TraceInformation("Claims for Identity: {0}", ci.Name);
                
                foreach (Claim c in ci.Claims)
                {
                    ts.TraceInformation("claimType: {0}", c.ClaimType);
                    ts.TraceInformation("claimValue: {0}", c.Value);

                    if (c.ClaimType == claimType && c.Value == claimValue)
                    {
                        ts.TraceInformation("** Claim FOUND!");

                        bool claimAvailable = p.Identities.Any(s =>
                            s.Claims.Any(cl => cl.ClaimType == claimType &&
                                cl.ValueType == ClaimValueTypes.String &&
                                cl.Value == claimValue));

                        ts.TraceInformation("* claimAvailable = " + claimAvailable.ToString());

                        return true;
                    }
                }
            }

            ts.TraceInformation("** Claim NOT FOUND!");
            
            return p.Identities.Any(s =>
                s.Claims.Any(c => c.ClaimType == claimType &&
                c.ValueType == ClaimValueTypes.String &&
                c.Value == claimValue));
        };

        /// <summary>
        /// Delegate that checks if any ClaimsIdentity associated with the given principal has the claim specified
        /// </summary>
        LessThanOrEqualClaimDelegate ClaimLessThanOrEqual = delegate(IClaimsPrincipal p, string claimType, string claimValue, string claimValueType)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.PolicyReader.LessThanOrEqualClaimDelegate",
               "MyTraceSource", SourceLevels.Information);

            ts.TraceInformation("claimType: {0}", claimType);
            ts.TraceInformation("claimValue: {0}", claimValue);

            foreach (IClaimsIdentity ci in p.Identities)
            {
                ts.TraceInformation("Claims for Identity: {0}", ci.Name);

                foreach (Claim c in ci.Claims)
                {
                    ts.TraceInformation("claimType: {0}", c.ClaimType);
                    ts.TraceInformation("claimValue: {0}", c.Value);

                    if (c.ClaimType == claimType )
                    {
                        ts.TraceInformation("** Claim FOUND!");

                        DateTime dateClaimValue = DateTime.Parse(claimValue);
                        ts.TraceInformation("*** dateClaimValue:" + dateClaimValue);

                        DateTime userDateClaimValue = DateTime.Parse(c.Value);
                        ts.TraceInformation("*** userDateClaimValue:" + userDateClaimValue);

                        bool claimAvailable = p.Identities.Any(s =>
                            s.Claims.Any(cl => cl.ClaimType == claimType &&
                                cl.ValueType == ClaimValueTypes.String &&
                                cl.Value == claimValue));

                        ts.TraceInformation("* claimAvailable = " + claimAvailable.ToString());

                        if (userDateClaimValue <= dateClaimValue)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            ts.TraceInformation("** Claim NOT FOUND!");

            return p.Identities.Any(s =>
                s.Claims.Any(c => c.ClaimType == claimType &&
                c.ValueType == ClaimValueTypes.String &&
                c.Value == claimValue));
        };


        /// <summary>
        /// Creates an instance of the policy reader
        /// </summary>
        public AuthorizationPolicyLoader()
        {
        }

        /// <summary>
        /// Read the policy as a LINQ expression
        /// </summary>
        /// <param name="rdr">XmlDictionaryReader for the policy Xml</param>
        /// <returns></returns>
        //public Expression<Func<IClaimsPrincipal, bool>> LoadPolicy(XElement elem)
        //{
        //    if (elem.Name != "policy")
        //    {
        //        throw new InvalidOperationException("Invalid policy element");
        //    }
                       

        //    //
        //    // Instantiate a parameter for the IClaimsPrincipal so it can be evaluated against
        //    // each claim constraint.
        //    // 
        //    ParameterExpression subject = Expression.Parameter(typeof(IClaimsPrincipal), "subject");

        //    //Expression<Func<IClaimsPrincipal, bool>> result = ReadNode(rdr, subject);
        //    //rdr.ReadEndElement();

        //    return result;
        //}

        public Expression<Func<IClaimsPrincipal, bool>> LoadPolicy(XmlDictionaryReader reader)
        {
            if (reader.Name != "policy")
            {
                throw new InvalidOperationException("Invalid policy document");
            }

            reader.Read();

            //
            // Instantiate a parameter for the IClaimsPrincipal so it can be evaluated against
            // each claim constraint.
            // 
            ParameterExpression subject = Expression.Parameter(typeof(IClaimsPrincipal), "subject");

            Expression<Func<IClaimsPrincipal, bool>> result = ReadNode(reader, subject);
            reader.ReadEndElement();

            return result;
        }


        /// <summary>
        /// Read the policy node
        /// </summary>
        /// <param name="rdr">XmlDictionaryReader for the policy Xml</param>
        /// <param name="subject">IClaimsPrincipal subject</param>
        /// <returns>A LINQ expression created from the policy</returns>
        private Expression<Func<IClaimsPrincipal, bool>> ReadNode(XmlDictionaryReader reader, ParameterExpression subject)
        {
            Expression<Func<IClaimsPrincipal, bool>> policyExpression;

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
        /// <param name="rdr">XmlDictionaryReader of the policy Xml</param>
        /// <returns>A LINQ expression created from the claim node</returns>
        private Expression<Func<IClaimsPrincipal, bool>> ReadClaim(XmlDictionaryReader reader)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.PolicyReader.ReadClaim",
                "MyTraceSource", SourceLevels.Information);

            string claimType = reader.GetAttribute("claimType");
            string claimValue = reader.GetAttribute("claimValue");
            string claimValueType = reader.GetAttribute("claimValueType");
            string claimValueOperation = reader.GetAttribute("claimValueOperation");

            ts.TraceInformation("Claim: <{0}, {1}, {2}, {3}>", claimType, claimValue, claimValueType, claimValueOperation);

            Expression<Func<IClaimsPrincipal, bool>> hasClaim = null;

            if (claimValueType == "Date" && claimValueOperation == "LessThanOrEqual")
            {
                hasClaim = (icp) => ClaimLessThanOrEqual(icp, claimType, claimValue, claimValueType);
            }
            else
            {
                hasClaim = (icp) => ClaimEquals(icp, claimType, claimValue);
            }

            reader.Read();

            return hasClaim;
        }

        /// <summary>
        /// Read the Or Node
        /// </summary>
        /// <param name="rdr">XmlDictionaryReader of the policy Xml</param>
        /// <param name="subject">IClaimsPrincipal subject</param>
        /// <returns>A LINQ expression created from the Or node</returns>
        private Expression<Func<IClaimsPrincipal, bool>> ReadOr(XmlDictionaryReader reader, ParameterExpression subject)
        {
            reader.Read();

            BinaryExpression lambda1 = Expression.OrElse(
                    Expression.Invoke(ReadNode(reader, subject), subject),
                    Expression.Invoke(ReadNode(reader, subject), subject));

            reader.ReadEndElement();

            Expression<Func<IClaimsPrincipal, bool>> lambda2 = Expression.Lambda<Func<IClaimsPrincipal, bool>>(lambda1, subject);
            return lambda2;
        }

        /// <summary>
        /// Read the And Node
        /// </summary>
        /// <param name="rdr">XmlDictionaryReader of the policy Xml</param>
        /// <param name="subject">IClaimsPrincipal subject</param>
        /// <returns>A LINQ expression created from the And node</returns>
        private Expression<Func<IClaimsPrincipal, bool>> ReadAnd(XmlDictionaryReader reader, ParameterExpression subject)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("CommercialVehicleCollisionWebservice.PolicyReader.ReadAnd",
                "MyTraceSource", SourceLevels.Information);

            reader.Read();

            ts.TraceInformation("Reader Element: " + reader.Name);

            ts.TraceInformation("ParameterExpression Subject: " + subject.Name);

            BinaryExpression lambda1 = Expression.AndAlso(
                    Expression.Invoke(ReadNode(reader, subject), subject),
                    Expression.Invoke(ReadNode(reader, subject), subject));

            reader.ReadEndElement();

            Expression<Func<IClaimsPrincipal, bool>> lambda2 = Expression.Lambda<Func<IClaimsPrincipal, bool>>(lambda1, subject);
            return lambda2;
        }
    }
}
