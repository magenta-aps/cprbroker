using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;

namespace CprBroker.Engine.DataProviders
{
    public class GetDataProviderListFacadeMethodInfo : FacadeMethodInfo<DataProviderType[]>
    {
        public GetDataProviderListFacadeMethodInfo(string appToken, string userToken)
            : base(appToken, userToken, true)
        {
        }
        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[] { new SubMethodInfo<IDataProviderManager, DataProviderType[]>() 
            {
                FailIfNoDataProvider=true,
                FailOnDefaultOutput=true,
                LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                Method=prov=>prov.GetDataProviderList(),
                UpdateMethod=null                
            } };
        }
    }
}
