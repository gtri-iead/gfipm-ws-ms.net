namespace CommercialVehicleCollisionModel
{
    using System;
    using System.ServiceModel;
    using System.Net.Security;
    

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getDocumentResponse", 
        WrapperNamespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsWrapped=true)]
    public partial class getDocumentResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0", Order=0)]
        public CommercialVehicleCollisionDocumentType CommercialVehicleCollisionDocument;
        
        public getDocumentResponse()
        {
        }
        
        public getDocumentResponse(CommercialVehicleCollisionDocumentType CommercialVehicleCollisionDocument)
        {
            this.CommercialVehicleCollisionDocument = CommercialVehicleCollisionDocument;
        }
    }
}
