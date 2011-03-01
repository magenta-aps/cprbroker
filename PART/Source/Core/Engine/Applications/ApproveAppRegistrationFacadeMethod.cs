using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Data.Applications;

namespace CprBroker.Engine
{
    /// <summary>
    /// Facade method for ApproveAppRegistration
    /// </summary>
    public class ApproveAppRegistrationFacadeMethod : GenericFacadeMethodInfo<bool>
    {
        string TargetApplicationToken;

        public ApproveAppRegistrationFacadeMethod(string applicationToken, string appToken, string userToken)
            : base(appToken, userToken)
        {
            TargetApplicationToken = applicationToken;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<IApplicationManager,bool>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption= LocalDataProviderUsageOption.UseFirst,
                    Method= prov=>prov.ApproveAppRegistration(TargetApplicationToken),
                    UpdateMethod=null,
                }
            };
        }

        public override StandardReturType ValidateInput()
        {
            if (string.IsNullOrEmpty(string.Format("{0}", TargetApplicationToken).Trim()))
            {
                return StandardReturType.NullInput();
            }

            using (ApplicationDataContext context = new ApplicationDataContext())
            {
                var application = context.Applications.SingleOrDefault(app => app.Token == TargetApplicationToken);
                if (application == null)
                {
                    return StandardReturType.UnknownObject("AppToken", TargetApplicationToken);
                }
            }
            return StandardReturType.OK();
        }

    }
}
