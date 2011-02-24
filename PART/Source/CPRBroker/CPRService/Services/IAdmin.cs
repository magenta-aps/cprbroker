using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Web.Services
{
    // NOTE: If you change the interface name "IAdmin" here, you must also update the reference to "IAdmin" in Web.config.
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IAdmin
    {
        [OperationContract]
        BasicOutputType<ApplicationType> RequestAppRegistration(ApplicationHeader applicationHeader,string ApplicationName);

        [OperationContract]
        BasicOutputType<bool> ApproveAppRegistration(ApplicationHeader applicationHeader,string ApplicationToken);

        [OperationContract]
        BasicOutputType<ApplicationType[]> ListAppRegistrations(ApplicationHeader applicationHeader);

        [OperationContract]
        BasicOutputType<bool> UnregisterApp(ApplicationHeader applicationHeader,string ApplicationToken);
        
        [OperationContract]
        BasicOutputType<ServiceVersionType[]> GetCapabilities(ApplicationHeader applicationHeader);

        [OperationContract]
        BasicOutputType<bool> IsImplementing(ApplicationHeader applicationHeader,string serviceName, string serviceVersion);
        
        [OperationContract]
        BasicOutputType<DataProviderType[]> GetDataProviderList(ApplicationHeader applicationHeader);

        [OperationContract]
        BasicOutputType<bool> SetDataProviderList(ApplicationHeader applicationHeader,DataProviderType[] DataProviders);
        
        [OperationContract]
        BasicOutputType<bool> Log(ApplicationHeader applicationHeader,string Text);

    }
}
