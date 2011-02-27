using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    public class RequestAppRegistrationFacadeMethod : GenericFacadeMethodInfo<ApplicationType>
    {
        string ApplicationName;
        public RequestAppRegistrationFacadeMethod(string appName, string appToken, string userToken)
            : base(appToken, userToken)
        {
            ApplicationName = appName;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<IApplicationManager,ApplicationType>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= prov=>prov.RequestAppRegistration(ApplicationName),
                    UpdateMethod=null,
                }
            };
        }

        public override StandardReturType ValidateInput()
        {
            if (string.IsNullOrEmpty(string.Format("{0}", ApplicationName).Trim()))
            {
                return StandardReturType.NullInput();
            }
            return StandardReturType.OK();
        }

    }
}
