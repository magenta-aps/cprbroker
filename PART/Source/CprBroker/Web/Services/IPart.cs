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
    [ServiceContract(Namespace = "http://dk.itst")]
    [XmlSerializerFormat]
    public interface IPart
    {
        [OperationContract]
        LaesOutputType Read(ApplicationHeader applicationHeader, LaesInputType LaesInput);

        [OperationContract]
        LaesOutputType RefreshRead(ApplicationHeader applicationHeader, LaesInputType LaesInput);

        [OperationContract]
        ListOutputType1 List(ApplicationHeader applicationHeader, ListInputType ListInput);

        [OperationContract]
        SoegOutputType Search(ApplicationHeader applicationHeader, SoegInputType1 SoegInput);

        [OperationContract]
        GetUuidOutputType GetUuid(ApplicationHeader applicationHeader, string cprNumber);
    }
}
