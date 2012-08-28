using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WSP_SelfHosted
{
    public class MutualCertificateAsymmerticCustomBinding
    {
        private WspCustomSecuredBinding _secureBinding = null;

        public MutualCertificateAsymmerticCustomBinding(bool httpsTransport)
        {
            _secureBinding = new WspCustomSecuredBinding(httpsTransport);
        }

        public CustomBinding CustomBinding
        {
            get { return new CustomBinding(_secureBinding.CreateBindingElements()); }
        }
    }
}
