using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.IdentityModel.Tokens;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;

namespace WeatherStationService
{
    public class SymmetricPolicyExtensionsBehaviorElement : BehaviorExtensionElement
    {

        public SymmetricPolicyExtensionsBehaviorElement()

            : base()
             
        { }



        public override Type BehaviorType
        {

            get { return typeof(SymmetricPolicyExtensionsBehavior); }

        }



        protected override object CreateBehavior()
        {

            return new SymmetricPolicyExtensionsBehavior();

        }

    }

}
