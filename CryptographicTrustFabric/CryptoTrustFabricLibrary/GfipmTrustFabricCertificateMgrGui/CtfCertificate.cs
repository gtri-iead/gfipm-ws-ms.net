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
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;


namespace CryptoTrustFabricMgr
{
    public class CtfCertificate
    {
        string entityId = string.Empty;
        string type = string.Empty;
        string keyUse = string.Empty;
        X509Certificate2 cert = null;

        public string EntityId
        {
            get { return entityId; }
        }

        public string KeyUse
        {
            get { return keyUse; }
        }

        public string Type
        {
            get { return type; }
        }

        public X509Certificate2 Cert
        {
            get { return cert; }
        }

        public CtfCertificate(string entityId, string keyUse, string type, X509Certificate2 cert)
        {
            this.entityId = entityId;
            this.keyUse = keyUse;
            this.type = type;
            this.cert = cert;
        }

    }
}
