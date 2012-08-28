namespace CommercialVehicleCollisionModel
{
    using System;
    using System.ServiceModel;
    using System.Net.Security;
    

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="downloadDataResponse", WrapperNamespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsWrapped=true)]
    public partial class downloadDataResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] Data;
        
        public downloadDataResponse()
        {
        }
        
        public downloadDataResponse(byte[] Data)
        {
            this.Data = Data;
        }
    }
}
