//
// Copyright (c) 2012, Georgia Institute of Technology. All Rights Reserved.
// This code was developed by Georgia Tech Research Institute (GTRI) under
// a grant from the U.S. Dept. of Justice, Bureau of Justice Assistance.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace CommercialVehicleCollisionServiceContract
{
    using System;
    using System.ServiceModel;
    using System.Net.Security;
    using CommercialVehicleCollisionModel;
    

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0", 
        ConfigurationName="ICommercialVehicleCollisionPortType", ProtectionLevel = ProtectionLevel.Sign)]
    public interface ICommercialVehicleCollisionPortType
    {
        
        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message getDocumentRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:getDocumentRequest", ReplyAction="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:getDocumentResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        getDocumentResponse getDocument(getDocumentRequest request);
        
        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message uploadPhotoRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:uploadPhotoRequest", ReplyAction="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:uploadPhotoResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        uploadPhotoResponse uploadPhoto(uploadPhotoRequest request);
        
        // CODEGEN: Generating message contract since the wrapper namespace (urn:examples.com:techniques:iepd:commercialVehicleCollision:message:2.0) of message downloadDataRequest does not match the default value (urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0)
        [System.ServiceModel.OperationContractAttribute(Action="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:downloadDataRequest", ReplyAction="urn:examples.com:techniques:iepd:commercialVehicleCollision:ws:2.0:CommercialVehi" +
            "cleCollisionPortType:downloadDataResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        downloadDataResponse downloadData(downloadDataRequest request);
    }
}
