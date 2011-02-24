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
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IPart
    {
        [OperationContract]
        LaesOutputType Read(ApplicationHeader applicationHeader, LaesInputType input);

        [OperationContract]
        LaesOutputType RefreshRead(ApplicationHeader applicationHeader, LaesInputType input);

        [OperationContract]
        ListOutputType1 List(ApplicationHeader applicationHeader, ListInputType input);

        [OperationContract]
        SoegOutputType Search(ApplicationHeader applicationHeader, SoegInputType1 searchCriteria);

        [OperationContract]
        GetUuidOutputType GetUuid(ApplicationHeader applicationHeader, string cprNumber);
    }
}
