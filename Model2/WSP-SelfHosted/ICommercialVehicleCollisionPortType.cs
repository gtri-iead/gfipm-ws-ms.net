#define SignOnly

namespace CommercialVehicleCollisionWebservice
{
    using System;
    using System.ServiceModel;
    using System.Net.Security;
    

#if SignOnly 
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0", 
        ConfigurationName="ICommercialVehicleCollisionPortType", ProtectionLevel = ProtectionLevel.Sign)]
    public interface ICommercialVehicleCollisionPortType
    {
        
        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message getDocumentRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:getDocumentRequest", ReplyAction="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:getDocumentResponse", ProtectionLevel = ProtectionLevel.Sign)]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        getDocumentResponse getDocument(getDocumentRequest request);
        
        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message uploadPhotoRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:uploadPhotoRequest", ReplyAction="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:uploadPhotoResponse", ProtectionLevel = ProtectionLevel.Sign)]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        uploadPhotoResponse uploadPhoto(uploadPhotoRequest request);
        
        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message downloadDataRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:downloadDataRequest", ReplyAction="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:downloadDataResponse", ProtectionLevel = ProtectionLevel.Sign)]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        downloadDataResponse downloadData(downloadDataRequest request);
    }
#else
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0",
        ConfigurationName = "ICommercialVehicleCollisionPortType", ProtectionLevel = ProtectionLevel.EncryptAndSign)]
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
}
