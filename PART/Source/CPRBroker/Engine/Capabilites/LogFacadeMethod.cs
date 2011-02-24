using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    public class LogFacadeMethod : GenericFacadeMethodInfo<bool>
    {
        string Text;

        public LogFacadeMethod(string text, string appToken, string userToken)
            : base(appToken, userToken)
        {
            Text = text;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<ILoggingDataProvider,bool>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= (prov)=>prov.Log(Text),
                    UpdateMethod=null
                }
            };
        }
    }
}
