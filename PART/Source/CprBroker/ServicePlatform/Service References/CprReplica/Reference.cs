﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CprBroker.Providers.ServicePlatform.CprReplica {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://serviceplatformen.dk/xml/wsdl/soap11/CprService/2/", ConfigurationName="CprReplica.CprServicePortType")]
    public interface CprServicePortType {
        
        // CODEGEN: Generating message contract since the wrapper name (callGCTPCheckServiceRequest) of message callGCTPCheckServiceRequest does not match the default value (callGCTPCheckService)
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        CprBroker.Providers.ServicePlatform.CprReplica.callGCTPCheckServiceResponse callGCTPCheckService(CprBroker.Providers.ServicePlatform.CprReplica.callGCTPCheckServiceRequest request);
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
    [System.ServiceModel.MessageContractAttribute(WrapperName="callGCTPCheckServiceRequest", WrapperNamespace="http://serviceplatformen.dk/xml/wsdl/soap11/CprService/2/", IsWrapped=true)]
    public partial class callGCTPCheckServiceRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://serviceplatformen.dk/xml/schemas/InvocationContext/1/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Namespace="http://serviceplatformen.dk/xml/schemas/InvocationContext/1/")]
        public CprBroker.Providers.ServicePlatform.CprReplica.InvocationContextType InvocationContext;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://serviceplatformen.dk/xml/wsdl/soap11/CprService/2/", Order=1)]
        public string gctpMessage;
        
        public callGCTPCheckServiceRequest() {
        }
        
        public callGCTPCheckServiceRequest(CprBroker.Providers.ServicePlatform.CprReplica.InvocationContextType InvocationContext, string gctpMessage) {
            this.InvocationContext = InvocationContext;
            this.gctpMessage = gctpMessage;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="callGCTPCheckServiceResponse", WrapperNamespace="http://serviceplatformen.dk/xml/wsdl/soap11/CprService/2/", IsWrapped=true)]
    public partial class callGCTPCheckServiceResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://serviceplatformen.dk/xml/wsdl/soap11/CprService/2/", Order=0)]
        public string result;
        
        public callGCTPCheckServiceResponse() {
        }
        
        public callGCTPCheckServiceResponse(string result) {
            this.result = result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface CprServicePortTypeChannel : CprBroker.Providers.ServicePlatform.CprReplica.CprServicePortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CprServicePortTypeClient : System.ServiceModel.ClientBase<CprBroker.Providers.ServicePlatform.CprReplica.CprServicePortType>, CprBroker.Providers.ServicePlatform.CprReplica.CprServicePortType {
        
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
        CprBroker.Providers.ServicePlatform.CprReplica.callGCTPCheckServiceResponse CprBroker.Providers.ServicePlatform.CprReplica.CprServicePortType.callGCTPCheckService(CprBroker.Providers.ServicePlatform.CprReplica.callGCTPCheckServiceRequest request) {
            return base.Channel.callGCTPCheckService(request);
        }
        
        public string callGCTPCheckService(CprBroker.Providers.ServicePlatform.CprReplica.InvocationContextType InvocationContext, string gctpMessage) {
            CprBroker.Providers.ServicePlatform.CprReplica.callGCTPCheckServiceRequest inValue = new CprBroker.Providers.ServicePlatform.CprReplica.callGCTPCheckServiceRequest();
            inValue.InvocationContext = InvocationContext;
            inValue.gctpMessage = gctpMessage;
            CprBroker.Providers.ServicePlatform.CprReplica.callGCTPCheckServiceResponse retVal = ((CprBroker.Providers.ServicePlatform.CprReplica.CprServicePortType)(this)).callGCTPCheckService(inValue);
            return retVal.result;
        }
    }
}