using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine.Part;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;

namespace CprBroker.Engine.DataProviders
{
    public class SetDataProvidersFacadeMethodInfo : GenericFacadeMethodInfo<bool>
    {
        private DataProviderType[] Input;

        public SetDataProvidersFacadeMethodInfo(DataProviderType[] inp, string appToken, string userToken)
            : base(appToken, userToken)
        {
            Input = inp;
        }

        public override StandardReturType ValidateInput()
        {
            if (Input == null)
            {
                return StandardReturType.NullInput();
            }
            foreach (var dp in Input)
            {
                if (dp == null || string.IsNullOrEmpty(dp.TypeName) || dp.Attributes == null)
                {
                    return StandardReturType.NullInput();
                }

                var dataProvider = Reflection.CreateInstance<IExternalDataProvider>(dp.TypeName);
                if (dataProvider == null)
                {
                    return StandardReturType.UnknownObject(dp.TypeName);
                }
                var propNames = dataProvider.ConfigurationKeys;
                foreach (var propInfo in propNames)
                {
                    var propName = propInfo.Name;
                    var prop = dp.Attributes.FirstOrDefault(p => p.Name.ToLower() == propName.ToLower());
                    if (prop == null || (propInfo.Required && string.IsNullOrEmpty(prop.Value)))
                    {
                        return StandardReturType.NullInput(propName);
                    }
                }
            }
            return StandardReturType.OK();
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[] 
            { 
                new SubMethodInfo<IDataProviderManager,bool>() 
                {
                    Method=prov=>prov.SetDataProviderList(Input),
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption = LocalDataProviderUsageOption.UseFirst,
                    UpdateMethod=null
                }
            };
        }
    }
}
