namespace CommercialVehicleCollisionModel
{
    
    
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
}
