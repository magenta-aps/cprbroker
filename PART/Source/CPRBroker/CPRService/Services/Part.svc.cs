using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Web.Services
{
    // NOTE: If you change the class name "Part" here, you must also update the reference to "Part" in Web.config.
    public class Part : IPart
    {
        private QualityHeader qualityHeader = new QualityHeader();

        public LaesOutputType Read(ApplicationHeader applicationHeader, LaesInputType input)
        {
            return Manager.Part.Read(applicationHeader.UserToken, applicationHeader.ApplicationToken, input, out qualityHeader.QualityLevel);
        }

        public LaesOutputType RefreshRead(ApplicationHeader applicationHeader, LaesInputType input)
        {
            return Manager.Part.RefreshRead(applicationHeader.UserToken, applicationHeader.ApplicationToken, input, out qualityHeader.QualityLevel);
        }

        public ListOutputType1 List(ApplicationHeader applicationHeader, ListInputType input)
        {
            return Manager.Part.List(applicationHeader.UserToken, applicationHeader.ApplicationToken, input, out qualityHeader.QualityLevel);
        }

        public SoegOutputType Search(ApplicationHeader applicationHeader, SoegInputType1 searchCriteria)
        {
            return Manager.Part.Search(applicationHeader.UserToken, applicationHeader.ApplicationToken, searchCriteria, out qualityHeader.QualityLevel);
        }

        public GetUuidOutputType GetUuid(ApplicationHeader applicationHeader, string cprNumber)
        {
            return Manager.Part.GetUuid(applicationHeader.UserToken, applicationHeader.ApplicationToken, cprNumber);
        }

    }
}
