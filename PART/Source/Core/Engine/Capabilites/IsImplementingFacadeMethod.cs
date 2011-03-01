using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    /// <summary>
    /// Facade method for IsImplementing
    /// </summary>
    public class IsImplementingFacadeMethod : GenericFacadeMethodInfo<bool>
    {
        string MethodName;
        string Version;

        public IsImplementingFacadeMethod(string appToken, string userToken, string methodName, string version)
            : base(appToken, userToken)
        {
            MethodName = methodName;
            Version = version;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<IVersionManager,bool>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=false,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= (prov)=>prov.IsImplementing(MethodName,Version),
                    UpdateMethod=null
                }
            };
        }

        public override bool IsValidResult(bool output)
        {
            return true;
        }
    }
}
