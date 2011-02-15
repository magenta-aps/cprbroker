using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;

namespace CprBroker.Engine.DataProviders
{
    public class SetDataProvidersFacadeMethodInfo : FacadeMethodInfo<bool>
    {
        private DataProviderType[] Input;

        public SetDataProvidersFacadeMethodInfo(DataProviderType[] inp, string appToken, string userToken)
            : base(appToken, userToken)
        {
            Input = inp;
        }

        public override bool IsValidInput(ref bool invaliInputReturnValue)
        {
            if (Input == null)
            {
                return false;
            }
            foreach (var dp in Input)
            {
                if (dp == null || string.IsNullOrEmpty(dp.TypeName) || dp.Attributes == null)
                {
                    return false;
                }

                var dataProvider = Util.Reflection.CreateInstance<IExternalDataProvider>(dp.TypeName);
                if (dataProvider == null)
                {
                    return false;
                }
                var propNames = dataProvider.ConfigurationKeys;
                foreach (var propInfo in propNames)
                {
                    var propName = propInfo.Name;
                    var prop = dp.Attributes.FirstOrDefault(p => p.Name.ToLower() == propName.ToLower());
                    if (prop == null || string.IsNullOrEmpty(prop.Value))
                    {
                        return false;
                    }
                }
            }
            return true;
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
