using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    public class GetCapabilitiesFacadeMethod:GenericFacadeMethodInfo<ServiceVersionType[]>
    {
        public GetCapabilitiesFacadeMethod(string appToken, string userToken)
            : base(appToken, userToken)
        { }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<IVersionManager,ServiceVersionType[]>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= (prov)=>prov.GetCapabilities(),
                    UpdateMethod=null
                }
            };

        }
    }
}
