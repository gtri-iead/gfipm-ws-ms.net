namespace CommercialVehicleCollisionModel
{
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="uploadPhotoResponse", WrapperNamespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsWrapped=true)]
    public partial class uploadPhotoResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0", Order=0)]
        public string PhotoControlID;
        
        public uploadPhotoResponse()
        {
        }
        
        public uploadPhotoResponse(string PhotoControlID)
        {
            this.PhotoControlID = PhotoControlID;
        }
    }
}
