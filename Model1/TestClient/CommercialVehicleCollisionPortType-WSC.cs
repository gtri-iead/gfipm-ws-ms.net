//#define SignOnly

using System;
using System.ServiceModel;
using System.Net.Security;

namespace CommercialVehicleCollisionWebservice
{


#if SignOnly    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0",
        ConfigurationName = "ICommercialVehicleCollisionPortType", ProtectionLevel = ProtectionLevel.Sign)]
    public interface ICommercialVehicleCollisionPortType
    {
        
        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message getDocumentRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:getDocumentRequest", ReplyAction="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:getDocumentResponse", ProtectionLevel=ProtectionLevel.Sign)]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        getDocumentResponse getDocument(getDocumentRequest request);

        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message uploadPhotoRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:uploadPhotoRequest", ReplyAction = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:uploadPhotoResponse", ProtectionLevel = ProtectionLevel.Sign)]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        uploadPhotoResponse uploadPhoto(uploadPhotoRequest request);

        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message downloadDataRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:downloadDataRequest", ReplyAction = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:downloadDataResponse", ProtectionLevel = ProtectionLevel.Sign)]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        downloadDataResponse downloadData(downloadDataRequest request);
    }
#else
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0",
        ConfigurationName = "ICommercialVehicleCollisionPortType")]
    public interface ICommercialVehicleCollisionPortType
    {

        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message getDocumentRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:getDocumentRequest", ReplyAction = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:getDocumentResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        getDocumentResponse getDocument(getDocumentRequest request);

        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message uploadPhotoRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:uploadPhotoRequest", ReplyAction = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:uploadPhotoResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        uploadPhotoResponse uploadPhoto(uploadPhotoRequest request);

        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message downloadDataRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:downloadDataRequest", ReplyAction = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:downloadDataResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        downloadDataResponse downloadData(downloadDataRequest request);
    }

#endif
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0")]
    public partial class CommercialVehicleCollisionDocumentType
    {
        
        private string documentFileControlIDField;
        
        private string incidentTextField;
        
        private string[] involvedVehicleVINField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string IncidentText
        {
            get
            {
                return this.incidentTextField;
            }
            set
            {
                this.incidentTextField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("InvolvedVehicleVIN", Order=2)]
        public string[] InvolvedVehicleVIN
        {
            get
            {
                return this.involvedVehicleVINField;
            }
            set
            {
                this.involvedVehicleVINField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getDocumentRequest", 
        WrapperNamespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsWrapped=true/*, ProtectionLevel=ProtectionLevel.Sign*/)]
    public partial class getDocumentRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0", Order=0)]
        public string DocumentFileControlID;
        
        public getDocumentRequest()
        {
        }
        
        public getDocumentRequest(string DocumentFileControlID)
        {
            this.DocumentFileControlID = DocumentFileControlID;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getDocumentResponse",
        WrapperNamespace = "urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsWrapped = true/*, ProtectionLevel = ProtectionLevel.Sign*/)]
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="uploadPhotoRequest", WrapperNamespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsWrapped=true)]
    public partial class uploadPhotoRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] Photo;
        
        public uploadPhotoRequest()
        {
        }
        
        public uploadPhotoRequest(byte[] Photo)
        {
            this.Photo = Photo;
        }
    }
    
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="downloadDataRequest", WrapperNamespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsWrapped=true)]
    public partial class downloadDataRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0", Order=0)]
        public int Size;
        
        public downloadDataRequest()
        {
        }
        
        public downloadDataRequest(int Size)
        {
            this.Size = Size;
        }
    }
    
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
    
    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    //[System.Xml.Serialization.XmlRootAttribute("getDocumentRequest", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    //public partial class GetDocumentRequestType
    //{
        
    //    private string documentFileControlIDField;
        
    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0")]
    //    public string DocumentFileControlID
    //    {
    //        get
    //        {
    //            return this.documentFileControlIDField;
    //        }
    //        set
    //        {
    //            this.documentFileControlIDField = value;
    //        }
    //    }
    //}
    
    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    //[System.Xml.Serialization.XmlRootAttribute("getDocumentResponse", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    //public partial class GetDocumentResponseType
    //{
        
    //    private CommercialVehicleCollisionDocumentType commercialVehicleCollisionDocumentField;
        
    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0")]
    //    public CommercialVehicleCollisionDocumentType CommercialVehicleCollisionDocument
    //    {
    //        get
    //        {
    //            return this.commercialVehicleCollisionDocumentField;
    //        }
    //        set
    //        {
    //            this.commercialVehicleCollisionDocumentField = value;
    //        }
    //    }
    //}
    
    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    //[System.Xml.Serialization.XmlRootAttribute("uploadPhotoRequest", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    //public partial class UploadPhotoRequestType
    //{
        
    //    private byte[] photoField;
        
    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0", DataType="base64Binary")]
    //    public byte[] Photo
    //    {
    //        get
    //        {
    //            return this.photoField;
    //        }
    //        set
    //        {
    //            this.photoField = value;
    //        }
    //    }
    //}
    
    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    //[System.Xml.Serialization.XmlRootAttribute("uploadPhotoResponse", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    //public partial class UploadPhotoResponseType
    //{
        
    //    private string photoControlIDField;
        
    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0")]
    //    public string PhotoControlID
    //    {
    //        get
    //        {
    //            return this.photoControlIDField;
    //        }
    //        set
    //        {
    //            this.photoControlIDField = value;
    //        }
    //    }
    //}
    
    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    //[System.Xml.Serialization.XmlRootAttribute("downloadDataRequest", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    //public partial class downloadDataRequestType
    //{
        
    //    private int sizeField;
        
    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0")]
    //    public int Size
    //    {
    //        get
    //        {
    //            return this.sizeField;
    //        }
    //        set
    //        {
    //            this.sizeField = value;
    //        }
    //    }
    //}
    
    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("WscfGen", "1.0.0.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0")]
    //[System.Xml.Serialization.XmlRootAttribute("downloadDataResponse", Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0", IsNullable=false)]
    //public partial class downloadDataResponseType
    //{
        
    //    private byte[] dataField;
        
    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:2.0", DataType="base64Binary")]
    //    public byte[] Data
    //    {
    //        get
    //        {
    //            return this.dataField;
    //        }
    //        set
    //        {
    //            this.dataField = value;
    //        }
    //    }
    //}
}
