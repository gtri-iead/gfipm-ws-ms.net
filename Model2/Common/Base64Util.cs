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

namespace Common
{
    public class Base64Util
    {
        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);

            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;
        }

        public static string EncodeTo64(byte[] toEncode)
        {
            string returnValue = System.Convert.ToBase64String(toEncode);
            return returnValue;
        }

        public static string DecodeFrom64(byte[] encodedData)
        {
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedData);

            return returnValue;
        }

        public static string FromBase64ToHex(string base64String)
        {
            byte[] base64Buf = Convert.FromBase64String(base64String);
            string hexString = BitConverter.ToString(base64Buf).Replace("-", string.Empty); ;

            return hexString;
        }
    }
}
