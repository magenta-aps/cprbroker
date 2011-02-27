using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class GerPersonUuidFacadeMethodInfo : FacadeMethodInfo<GetUuidOutputType, string>
    {
        public string Input;

        public GerPersonUuidFacadeMethodInfo(string input, string appToken, string userToken)
            : base(appToken, userToken)
        {
            Input = input;
        }

        public override StandardReturType ValidateInput()
        {
            if (string.IsNullOrEmpty(Input))
            {
                return StandardReturType.NullInput();
            }

            var pattern = @"\A\d{10}\Z";
            if (!System.Text.RegularExpressions.Regex.Match(Input, pattern).Success)
            {
                return StandardReturType.InvalidCprNumber(Input);
            }

            long val;
            if (!long.TryParse(Input, out val))
            {
                return StandardReturType.InvalidCprNumber(Input);
            }

            if (!Util.Strings.PersonNumberToDate(Input).HasValue)
            {
                return StandardReturType.InvalidCprNumber(Input);
            }

            return StandardReturType.OK();
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

        public override string Aggregate(object[] results)
        {
            return results[0].ToString();
        }
    }
}
