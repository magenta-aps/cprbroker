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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/")]
    [System.Xml.Serialization.XmlRootAttribute("BirthdateSubscription", Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/", IsNullable=false)]
    public partial class BirthdateSubscriptionType : SubscriptionType {
        
        private System.Nullable<int> ageYearsField;
        
        private int priorDaysField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> AgeYears {
            get {
                return this.ageYearsField;
            }
            set {
                this.ageYearsField = value;
            }
        }
        
        /// <remarks/>
        public int PriorDays {
            get {
                return this.priorDaysField;
            }
            set {
                this.priorDaysField = value;
            }
        }
    }
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BirthdateSubscriptionType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ChangeSubscriptionType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/")]
    public abstract partial class SubscriptionType {
        
        private string subscriptionIdField;
        
        private string applicationTokenField;
        
        private ChannelBaseType notificationChannelField;
        
        private SubscriptionPersonsType personsField;
        
        /// <remarks/>
        public string SubscriptionId {
            get {
                return this.subscriptionIdField;
            }
            set {
                this.subscriptionIdField = value;
            }
        }
        
        /// <remarks/>
        public string ApplicationToken {
            get {
                return this.applicationTokenField;
            }
            set {
                this.applicationTokenField = value;
            }
        }
        
        /// <remarks/>
        public ChannelBaseType NotificationChannel {
            get {
                return this.notificationChannelField;
            }
            set {
                this.notificationChannelField = value;
            }
        }
        
        /// <remarks/>
        public SubscriptionPersonsType Persons {
            get {
                return this.personsField;
            }
            set {
                this.personsField = value;
            }
        }
    }
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(FileShareChannelType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(WebServiceChannelType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/")]
    public abstract partial class ChannelBaseType {
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/")]
    public partial class SubscriptionPersonsType {
        
        private object itemField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Criteria", typeof(SoegObjektType))]
        [System.Xml.Serialization.XmlElementAttribute("ForAllPersons", typeof(bool))]
        [System.Xml.Serialization.XmlElementAttribute("PersonUuids", typeof(PersonUuidsType))]
        public object Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/")]
    public partial class PersonUuidsType {
        
        private string[] uUIDField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UUID", Namespace="urn:oio:dkal:1.0.0")]
        public string[] UUID {
            get {
                return this.uUIDField;
            }
            set {
                this.uUIDField = value;
            }
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/")]
    [System.Xml.Serialization.XmlRootAttribute("ChangeSubscription", Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/", IsNullable=false)]
    public partial class ChangeSubscriptionType : SubscriptionType {
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/")]
    [System.Xml.Serialization.XmlRootAttribute("WebServiceChannel", Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/", IsNullable=false)]
    public partial class WebServiceChannelType : ChannelBaseType {
        
        private string webServiceUrlField;
        
        /// <remarks/>
        public string WebServiceUrl {
            get {
                return this.webServiceUrlField;
            }
            set {
                this.webServiceUrlField = value;
            }
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/")]
    [System.Xml.Serialization.XmlRootAttribute("FileShareChannel", Namespace="http://rep.oio.dk/cpr.dk/xml/schemas/2009/07/01/", IsNullable=false)]
    public partial class FileShareChannelType : ChannelBaseType {
        
        private string pathField;
        
        /// <remarks/>
        public string Path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
            }
        }
    }
}