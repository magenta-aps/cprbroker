﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CprBroker.Providers.ServicePlatform.CprService2 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://serviceplatformen.dk/xml/wsdl/soap11/CprService/1/", ConfigurationName="CprService2.CprServicePortType")]
    public interface CprServicePortType {
        
        // CODEGEN: Generating message contract since the wrapper name (forwardToCPRServiceRequest) of message forwardToCPRServiceRequest does not match the default value (forwardToCPRService)
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        CprBroker.Providers.ServicePlatform.CprService2.forwardToCPRServiceResponse forwardToCPRService(CprBroker.Providers.ServicePlatform.CprService2.forwardToCPRServiceRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://serviceplatformen.dk/xml/schemas/InvocationContext/1/")]
    public partial class InvocationContextType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string serviceAgreementUUIDField;
        
        private string userSystemUUIDField;
        
        private string userUUIDField;
        
        private string onBehalfOfUserField;
        
        private string serviceUUIDField;
        
        private string callersServiceCallIdentifierField;
        
        private string accountingInfoField;
        
        /// <remarks/>
        public string ServiceAgreementUUID {
            get {
                return this.serviceAgreementUUIDField;
            }
            set {
                this.serviceAgreementUUIDField = value;
                this.RaisePropertyChanged("ServiceAgreementUUID");
            }
        }
        
        /// <remarks/>
        public string UserSystemUUID {
            get {
                return this.userSystemUUIDField;
            }
            set {
                this.userSystemUUIDField = value;
                this.RaisePropertyChanged("UserSystemUUID");
            }
        }
        
        /// <remarks/>
        public string UserUUID {
            get {
                return this.userUUIDField;
            }
            set {
                this.userUUIDField = value;
                this.RaisePropertyChanged("UserUUID");
            }
        }
        
        /// <remarks/>
        public string OnBehalfOfUser {
            get {
                return this.onBehalfOfUserField;
            }
            set {
                this.onBehalfOfUserField = value;
                this.RaisePropertyChanged("OnBehalfOfUser");
            }
        }
        
        /// <remarks/>
        public string ServiceUUID {
            get {
                return this.serviceUUIDField;
            }
            set {
                this.serviceUUIDField = value;
                this.RaisePropertyChanged("ServiceUUID");
            }
        }
        
        /// <remarks/>
        public string CallersServiceCallIdentifier {
            get {
                return this.callersServiceCallIdentifierField;
            }
            set {
                this.callersServiceCallIdentifierField = value;
                this.RaisePropertyChanged("CallersServiceCallIdentifier");
            }
        }
        
        /// <remarks/>
        public string AccountingInfo {
            get {
                return this.accountingInfoField;
            }
            set {
                this.accountingInfoField = value;
                this.RaisePropertyChanged("AccountingInfo");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="forwardToCPRServiceRequest", WrapperNamespace="http://serviceplatformen.dk/xml/wsdl/soap11/CprService/1/", IsWrapped=true)]
    public partial class forwardToCPRServiceRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://serviceplatformen.dk/xml/schemas/InvocationContext/1/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace="http://serviceplatformen.dk/xml/schemas/InvocationContext/1/")]
        public CprBroker.Providers.ServicePlatform.CprService2.InvocationContextType InvocationContext;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://serviceplatformen.dk/xml/wsdl/soap11/CprService/1/", Order=1)]
        public string gctpMessage;
        
        public forwardToCPRServiceRequest() {
        }
        
        public forwardToCPRServiceRequest(CprBroker.Providers.ServicePlatform.CprService2.InvocationContextType InvocationContext, string gctpMessage) {
            this.InvocationContext = InvocationContext;
            this.gctpMessage = gctpMessage;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="forwardToCPRServiceResponse", WrapperNamespace="http://serviceplatformen.dk/xml/wsdl/soap11/CprService/1/", IsWrapped=true)]
    public partial class forwardToCPRServiceResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://serviceplatformen.dk/xml/wsdl/soap11/CprService/1/", Order=0)]
        public string result;
        
        public forwardToCPRServiceResponse() {
        }
        
        public forwardToCPRServiceResponse(string result) {
            this.result = result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface CprServicePortTypeChannel : CprBroker.Providers.ServicePlatform.CprService2.CprServicePortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CprServicePortTypeClient : System.ServiceModel.ClientBase<CprBroker.Providers.ServicePlatform.CprService2.CprServicePortType>, CprBroker.Providers.ServicePlatform.CprService2.CprServicePortType {
        
        public CprServicePortTypeClient() {
        }
        
        public CprServicePortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CprServicePortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CprServicePortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CprServicePortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        CprBroker.Providers.ServicePlatform.CprService2.forwardToCPRServiceResponse CprBroker.Providers.ServicePlatform.CprService2.CprServicePortType.forwardToCPRService(CprBroker.Providers.ServicePlatform.CprService2.forwardToCPRServiceRequest request) {
            return base.Channel.forwardToCPRService(request);
        }
        
        public string forwardToCPRService(CprBroker.Providers.ServicePlatform.CprService2.InvocationContextType InvocationContext, string gctpMessage) {
            CprBroker.Providers.ServicePlatform.CprService2.forwardToCPRServiceRequest inValue = new CprBroker.Providers.ServicePlatform.CprService2.forwardToCPRServiceRequest();
            inValue.InvocationContext = InvocationContext;
            inValue.gctpMessage = gctpMessage;
            CprBroker.Providers.ServicePlatform.CprService2.forwardToCPRServiceResponse retVal = ((CprBroker.Providers.ServicePlatform.CprService2.CprServicePortType)(this)).forwardToCPRService(inValue);
            return retVal.result;
        }
    }
}