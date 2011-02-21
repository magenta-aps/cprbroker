using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.DataProviders
{
    public class GetDataProviderListFacadeMethodInfo : GenericFacadeMethodInfo<DataProviderType[]>
    {
        public GetDataProviderListFacadeMethodInfo(string appToken, string userToken)
            : base(appToken, userToken)
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

        public override BasicOutputType<DataProviderType[]> Aggregate(object[] results)
        {
            return new BasicOutputType<DataProviderType[]>()
            {
                Item = (DataProviderType[])results[0]
            };
        }
    }
}
