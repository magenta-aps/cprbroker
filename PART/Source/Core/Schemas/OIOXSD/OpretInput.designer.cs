//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace CprBroker.Schemas.Part {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(OpretInputType1))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:oio:sagdok:2.0.0")]
    [System.Xml.Serialization.XmlRootAttribute("OpretInput", Namespace="urn:oio:sagdok:2.0.0", IsNullable=false)]
    public partial class OpretInputType {
        
        private string brugervendtNoegleTekstField;
        
        private string noteTekstField;
        
        private VirkningType[] virkningField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="urn:oio:sagdok:1.0.0")]
        public string BrugervendtNoegleTekst {
            get {
                return this.brugervendtNoegleTekstField;
            }
            set {
                this.brugervendtNoegleTekstField = value;
            }
        }
        
        /// <remarks/>
        public string NoteTekst {
            get {
                return this.noteTekstField;
            }
            set {
                this.noteTekstField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Virkning")]
        public VirkningType[] Virkning {
            get {
                return this.virkningField;
            }
            set {
                this.virkningField = value;
            }
        }
    }
}