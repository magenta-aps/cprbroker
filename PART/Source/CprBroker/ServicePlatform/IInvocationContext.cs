using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.ServicePlatform
{
    public interface IInvocationContext
    {
        string ServiceAgreementUUID { get; set; }

        string UserSystemUUID { get; set; }

        string UserUUID { get; set; }

        string OnBehalfOfUser { get; set; }

        string ServiceUUID { get; set; }

        string CallersServiceCallIdentifier{ get; set; }

        string AccountingInfo { get; set; }
    }
}

namespace CprBroker.Providers.ServicePlatform.CprReplica
{
    public partial class InvocationContextType : IInvocationContext
    {

    }
}

namespace CprBroker.Providers.ServicePlatform.CprSubscriptionService {
    public partial class InvocationContextType : IInvocationContext
    {

    }
}