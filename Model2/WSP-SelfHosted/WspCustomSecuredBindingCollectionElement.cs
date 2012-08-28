using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;
using System.Configuration;

namespace WSP_SelfHosted
{
    public class WspCustomSecuredBindingCollectionElement : BindingCollectionElement
    {
        public override Type BindingType
        {
            get { return typeof(WspCustomSecuredBinding); }
        }

        public override ReadOnlyCollection<IBindingConfigurationElement> ConfiguredBindings
        {
            get
            {
                return new ReadOnlyCollection<IBindingConfigurationElement>(new List<IBindingConfigurationElement>());
            }
        }

        public override bool ContainsKey(string name)
        {
            throw new NotImplementedException();
        }

        protected override Binding GetDefault()
        {
            return new WspCustomSecuredBinding();
        }

        protected override bool TryAdd(string name, Binding binding, System.Configuration.Configuration config)
        {
            throw new NotImplementedException();
        }
    }
}
