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
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;

namespace WebServiceConsumerWebHost
{
    public class CommercialVehicleInspectorBehaviorElement : BehaviorExtensionElement
    {
        public CommercialVehicleInspectorBehaviorElement()
            : base()
        { 
        }

        public override Type BehaviorType
        {
            get { return typeof(CommercialVehicleMessageInspectorBehavior); }

        }
        
        protected override object CreateBehavior()
        {
            return new CommercialVehicleMessageInspectorBehavior();
        }

        ///////////////////// TEST 

        //[ConfigurationProperty("useHttpsTransport")]
        //public bool UseHttpsTransport
        //{
        //    get
        //    {
        //        return (bool)base["useHttpsTransport"];
        //    }
        //    set
        //    {
        //        base["useHttpsTransport"] = value;
        //    }
        //}

        //ConfigurationPropertyCollection properties = null;
        //protected override ConfigurationPropertyCollection Properties
        //{
        //    get
        //    {
        //        if (this.properties == null)
        //        {
        //            ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection();
        //            propertys.Add(new ConfigurationProperty("useHttpsTransport", typeof(bool), null, ConfigurationPropertyOptions.IsRequired));
        //            this.properties = propertys;
        //        }
        //        return this.properties;
        //    }
        //}

        //////////////////// END TEST
    }
}
