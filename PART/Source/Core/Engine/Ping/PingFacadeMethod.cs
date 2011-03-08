using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Ping
{
    public class PingFacadeMethod:GenericFacadeMethodInfo<bool>
    {
        public PingFacadeMethod(string appToken, string userToken)
            : base(appToken, userToken)
        {
 
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]{
                new SubMethodInfo<IPingDataProvider,bool>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=false,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= (prov)=>prov.Ping(),
                    UpdateMethod=null
                }
            };
        }

    }
}
