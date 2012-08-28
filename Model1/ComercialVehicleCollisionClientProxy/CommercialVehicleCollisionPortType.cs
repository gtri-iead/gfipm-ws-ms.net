
using CommercialVehicleCollisionServiceContract;
using CommercialVehicleCollisionModel;

namespace ComercialVehicleCollisionClientProxy
{
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICommercialVehicleCollisionPortTypeChannel : ICommercialVehicleCollisionPortType, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CommercialVehicleCollisionPortTypeClient : System.ServiceModel.ClientBase<ICommercialVehicleCollisionPortType>, ICommercialVehicleCollisionPortType
    {
        
        public CommercialVehicleCollisionPortTypeClient()
        {
        }
        
        public CommercialVehicleCollisionPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public CommercialVehicleCollisionPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CommercialVehicleCollisionPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CommercialVehicleCollisionPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getDocumentResponse ICommercialVehicleCollisionPortType.getDocument(getDocumentRequest request)
        {
            return base.Channel.getDocument(request);
        }
        
        public CommercialVehicleCollisionDocumentType getDocument(string DocumentFileControlID)
        {
            getDocumentRequest inValue = new getDocumentRequest();
            inValue.DocumentFileControlID = DocumentFileControlID;
            getDocumentResponse retVal = ((ICommercialVehicleCollisionPortType)(this)).getDocument(inValue);
            return retVal.CommercialVehicleCollisionDocument;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        uploadPhotoResponse ICommercialVehicleCollisionPortType.uploadPhoto(uploadPhotoRequest request)
        {
            return base.Channel.uploadPhoto(request);
        }
        
        public string uploadPhoto(byte[] Photo)
        {
            uploadPhotoRequest inValue = new uploadPhotoRequest();
            inValue.Photo = Photo;
            uploadPhotoResponse retVal = ((ICommercialVehicleCollisionPortType)(this)).uploadPhoto(inValue);
            return retVal.PhotoControlID;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        downloadDataResponse ICommercialVehicleCollisionPortType.downloadData(downloadDataRequest request)
        {
            return base.Channel.downloadData(request);
        }
        
        public byte[] downloadData(int Size)
        {
            downloadDataRequest inValue = new downloadDataRequest();
            inValue.Size = Size;
            downloadDataResponse retVal = ((ICommercialVehicleCollisionPortType)(this)).downloadData(inValue);
            return retVal.Data;
        }
    }    
}
