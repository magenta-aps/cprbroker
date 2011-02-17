using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class GerPersonUuidFacadeMethodInfo : FacadeMethodInfo<GetUuidOutputType>
    {
        public string Input;

        public GerPersonUuidFacadeMethodInfo(string input, string appToken, string userToken)
            : base(appToken, userToken)
        {
            Input = input;
        }

        public override void Initialize()
        {

            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<IPartPersonMappingDataProvider,Guid?>()
                {
                    Method = (prov)=>prov.GetPersonUuid(Input),
                    LocalDataProviderOption = LocalDataProviderUsageOption.UseFirst,
                    FailOnDefaultOutput=true,
                    FailIfNoDataProvider=true,
                    UpdateMethod=uuid=>Local.UpdateDatabase.UpdatePersonUuid(Input,uuid.Value),
                }
            };
        }

        public override GetUuidOutputType Aggregate(object[] results)
        {
            return new GetUuidOutputType()
            {
                StandardRetur = new StandardReturType()
                {
                    FejlbeskedTekst = "",
                    StatusKode = ""
                },
                UUID = results[0].ToString()
            };
        }
    }
}
