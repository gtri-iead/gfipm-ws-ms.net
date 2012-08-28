namespace CommercialVehicleCollisionModel
{
    
    
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
}
