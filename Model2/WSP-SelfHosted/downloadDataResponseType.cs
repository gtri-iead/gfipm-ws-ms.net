namespace CommercialVehicleCollisionWebservice
{
    
    
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
