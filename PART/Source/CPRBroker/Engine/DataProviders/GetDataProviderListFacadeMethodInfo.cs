﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.DataProviders
{
    public class GetDataProviderListFacadeMethodInfo : FacadeMethodInfo<BasicOutputType<DataProviderType[]>>
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
    }
}
