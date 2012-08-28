using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace TestClient
{
    public class CustomIdentityVerifier : IdentityVerifier
    {
        // Code to be added.
        public override bool CheckAccess(EndpointIdentity identity, AuthorizationContext authContext)
        {
            StreamWriter file = new StreamWriter("c:\\temp\\TestClient.CustomIdentityVerifier - CheckAccess.txt", true);
            file.WriteLine("_________________________________________");
            file.WriteLine("DateTime: " + DateTime.Now.ToString());

            bool returnvalue = false;

            foreach (ClaimSet claimset in authContext.ClaimSets)
            {
                foreach (Claim claim in claimset)
                {
                    file.WriteLine("claim.ClaimType: " + claim.ClaimType);
                    file.WriteLine("\tclaim.Right: " + claim.Right);
                    file.WriteLine("\t\tclaim.Resource: " + claim.Resource.ToString());

                    if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/x500distinguishedname")
                    {
                        X500DistinguishedName name = (X500DistinguishedName)claim.Resource;
                        file.WriteLine("X500DistinguishedName: " + name.Name);

                        //if (name.Name.Contains(((OrgEndpointIdentity)identity).OrganizationClaim))
                        //if ("CN=zpatlittrs239.ittl.gtri.org" == name.Name)
                        if ("E=gfipm-support@lists.gatech.edu, CN=HA50WSP, O=Hawaii Five0, L=Honolulu, S=Hawaii, C=US" == name.Name)
                        {
                            file.WriteLine("\tClaim Type: {0}", claim.ClaimType);
                            file.WriteLine("\tRight: {0}", claim.Right);
                            file.WriteLine("\tResource: {0}", claim.Resource);
                            file.WriteLine();
                            returnvalue = true;
                        }
                    }
                }

            }

            file.Close();

            return returnvalue;
        }

        public override bool TryGetIdentity(EndpointAddress reference, out EndpointIdentity identity)
        {
            return IdentityVerifier.CreateDefault().TryGetIdentity(reference, out identity);
        }
    }
}
