namespace CommercialVehicleCollisionWebservice
{
    using System;
    using System.ServiceModel;
    using System.Net.Security;
    using CommercialVehicleCollisionServiceContract;

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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    [System.Xml.Serialization.XmlRootAttribute("getDocumentRequest", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    public partial class GetDocumentRequestType
    {
        
        private string documentFileControlIDField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0")]
        public string DocumentFileControlID
        {
            get
            {
                return this.documentFileControlIDField;
            }
            set
            {
                this.documentFileControlIDField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    [System.Xml.Serialization.XmlRootAttribute("getDocumentResponse", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    public partial class GetDocumentResponseType
    {
        
        private CommercialVehicleCollisionDocumentType commercialVehicleCollisionDocumentField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0")]
        public CommercialVehicleCollisionDocumentType CommercialVehicleCollisionDocument
        {
            get
            {
                return this.commercialVehicleCollisionDocumentField;
            }
            set
            {
                this.commercialVehicleCollisionDocumentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    [System.Xml.Serialization.XmlRootAttribute("uploadPhotoRequest", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    public partial class UploadPhotoRequestType
    {
        
        private byte[] photoField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0", DataType="base64Binary")]
        public byte[] Photo
        {
            get
            {
                return this.photoField;
            }
            set
            {
                this.photoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    [System.Xml.Serialization.XmlRootAttribute("uploadPhotoResponse", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    public partial class UploadPhotoResponseType
    {
        
        private string photoControlIDField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0")]
        public string PhotoControlID
        {
            get
            {
                return this.photoControlIDField;
            }
            set
            {
                this.photoControlIDField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    [System.Xml.Serialization.XmlRootAttribute("downloadDataRequest", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    public partial class downloadDataRequestType
    {
        
        private int sizeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0")]
        public int Size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    [System.Xml.Serialization.XmlRootAttribute("downloadDataResponse", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    public partial class downloadDataResponseType
    {
        
        private byte[] dataField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0", DataType="base64Binary")]
        public byte[] Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }
    }
}
