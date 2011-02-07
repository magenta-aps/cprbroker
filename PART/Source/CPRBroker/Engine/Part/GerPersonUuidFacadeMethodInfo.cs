using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Part
{
    public class GerPersonUuidFacadeMethodInfo : FacadeMethodInfo<Guid>
    {
        public string Input;

        public GerPersonUuidFacadeMethodInfo(string input, string appToken, string userToken, bool appTokenRequired)
            : base(appToken, userToken, appTokenRequired)
        {
            Input = input;
        }

        public override void Initialize()
        {

            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<IPartPersonMappingDataProvider,Guid>()
                {
                    Method = (prov)=>prov.GetPersonUuid(Input),
                    LocalDataProviderOption = LocalDataProviderUsageOption.UseFirst,
                    FailOnDefaultOutput=true,
                    UpdateMethod=uuid=>Local.UpdateDatabase.UpdatePersonUuid(Input,uuid),
                }
            };
        }

    }
}
