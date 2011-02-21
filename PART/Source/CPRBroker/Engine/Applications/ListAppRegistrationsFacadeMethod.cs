using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.DAL.Applications;

namespace CprBroker.Engine
{
    public class ListAppRegistrationsFacadeMethod : GenericFacadeMethodInfo<ApplicationType[]>
    {

        public ListAppRegistrationsFacadeMethod(string appToken, string userToken)
            : base(appToken, userToken)
        {
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<IApplicationManager,ApplicationType[]>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= prov=>prov.ListAppRegistration(),
                    UpdateMethod=null,
                }
            };
        }

        public override StandardReturType ValidateInput()
        {
            return StandardReturType.OK();
        }

        public override BasicOutputType<ApplicationType[]> Aggregate(object[] results)
        {
            return BasicOutputType<ApplicationType[]>.CreateAsOKFromFirstResult(results);
        }
    }
}
