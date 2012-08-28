namespace CommercialVehicleCollisionWebservice
{
    
    
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
}
