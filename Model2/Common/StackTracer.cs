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
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Common
{
    public class StackTracer
    {
        public static void TraceStack(StreamWriter file)
        {
            StackTrace stackTrace = new StackTrace();

            if (file != null && stackTrace.FrameCount > 0)
            {
                file.WriteLine("_________________________________________");

                file.WriteLine("StackTrace:");

                for (int i = 0; i < stackTrace.FrameCount; i++)
                {
                    if (stackTrace.GetFrame(i).GetMethod() != null && stackTrace.GetFrame(i).GetMethod().DeclaringType != null)
                    {
                        file.Write("\t" + stackTrace.GetFrame(i).GetMethod().DeclaringType.ToString());
                        file.Write("." + stackTrace.GetFrame(i).GetMethod().Name);

                        ParameterInfo[] parameters = stackTrace.GetFrame(i).GetMethod().GetParameters();

                        if (parameters != null && parameters.Length > 0)
                        {
                            file.Write("( ");

                            foreach (ParameterInfo pi in parameters)
                            {
                                file.Write(pi.ParameterType + " " + pi.Name + ", ");
                            }

                            file.Write(" )");
                        }

                        file.WriteLine("");
                    }
                }
                file.WriteLine("_________________________________________");
            }                   
        }

        public static void TraceStack(TraceSource file)
        {
            StackTrace stackTrace = new StackTrace();

            if (file != null && stackTrace.FrameCount > 0)
            {
                file.TraceInformation("_________________________________________");

                file.TraceInformation("StackTrace:");

                for (int i = 0; i < stackTrace.FrameCount; i++)
                {
                    string frame = string.Empty;

                    if (stackTrace.GetFrame(i).GetMethod() != null && stackTrace.GetFrame(i).GetMethod().DeclaringType != null)
                    {
                        frame = "\t" + stackTrace.GetFrame(i).GetMethod().DeclaringType.ToString() +
                            "." + stackTrace.GetFrame(i).GetMethod().Name;
                        //file.TraceInformation("." + stackTrace.GetFrame(i).GetMethod().Name);

                        ParameterInfo[] parameters = stackTrace.GetFrame(i).GetMethod().GetParameters();

                        if (parameters != null && parameters.Length > 0)
                        {
                            frame += "( ";

                            foreach (ParameterInfo pi in parameters)
                            {
                                frame += pi.ParameterType + " " + pi.Name + ", ";
                            }

                            frame += " )";
                        }

                        file.TraceInformation(frame);

                        file.TraceInformation("");
                    }
                }
                file.TraceInformation("_________________________________________");
            }
        }
    }
}
