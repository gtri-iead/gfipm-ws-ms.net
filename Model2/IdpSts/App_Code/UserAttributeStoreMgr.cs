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
using System.Reflection;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using Microsoft.IdentityModel.Claims;
using System.Diagnostics;

using Common;

namespace IdpSts
{
    public class UserAttributeStoreMgr
    {
        public UserAttributeStoreMgr()
        {
        }

        public List<Claim> GetClaims(string userIdentity)
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("IdpSts.UserAttributeStoreMgr.GetClaims",
                "MyTraceSource", SourceLevels.Information);

            var root = GetUserAttributes();

            IEnumerable<XElement> entry =
                from e1 in root.Elements("User")
                where (string)e1.Attribute("userName") == userIdentity
                select e1;

            ts.TraceInformation("User Count: " + entry.Count());

            if (entry.Count() != 0)
            {
                IEnumerable<XElement> entry2 =
                from e2 in entry.Elements("Attribute")
                select e2;

                ts.TraceInformation("User Attributes Count: " + entry2.Count());

                List<Claim> userClaims = new List<Claim>();

                foreach (XElement claimElem in entry2)
                {
                    Claim claim = CreateClaim(claimElem.Attribute("URI").Value, claimElem.Attribute("Value").Value, 
                        claimElem.Attribute("claimPropertyURI").Value, claimElem.Attribute("claimProperty").Value);

                    userClaims.Add(claim);
                }

                return userClaims;
            }

            return null;
        }

        private Claim CreateClaim(string claimType, string claimValue, string claimProperty, string claimPropertyValue)
        {
            var claim = new Claim(claimType, claimValue);
            claim.Properties[claimProperty] = claimPropertyValue;

            return claim;
        }

        private XElement GetUserAttributes()
        {
            CustomTextTraceSource ts = new CustomTextTraceSource("IdpSts.UserAttributeStoreMgr.GetUserAttributes",
                "MyTraceSource", SourceLevels.Information);

            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "IdpSts.UserAttributeStore.xml");

                ts.TraceInformation("stream: " + (stream != null ? "Not NULL" : "NULL") );

                XmlReader reader = XmlReader.Create(stream);
                
                ts.TraceInformation("reader: " + (reader != null ? "Not NULL" : "NULL"));

                // Make the XmlReader interactive
                reader.MoveToContent();

                XElement root = XElement.ReadFrom(reader) as XElement;

                ts.TraceInformation("root: " + (root != null ? "Not NULL" : "NULL"));

                return root;
            }
            catch (Exception e)
            {
                ts.TraceInformation("Exception: " + e.Message);
                if (e.InnerException != null)
                {
                    ts.TraceInformation("InnerException: " + e.InnerException.Message);
                }
                throw;
            }
        }
    }
}
