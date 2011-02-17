using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Part.Events;

namespace CprBroker.Engine.Events
{
    public class GetPersonBirthdatesFacadeMethodInfo : FacadeMethodInfo<BasicOutputType<PersonBirthdate[]>>
    {
        Guid? PersonUuidToStartAfter;
        int MaxCount;

        public GetPersonBirthdatesFacadeMethodInfo(Guid? personUuidToStartAfter, int maxCount, string appToken, string userToken)
            : base(appToken, userToken)
        {
            this.PersonUuidToStartAfter = personUuidToStartAfter;
            MaxCount = maxCount;
        }

        public override StandardReturType ValidateInput()
        {
            if (MaxCount <= 0 || MaxCount > 10000)
            {
                return StandardReturType.ValueOutOfRange(MaxCount);
            }
            return StandardReturType.OK();
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[] { new GetPersonBirthdatesSubmethodInfo(PersonUuidToStartAfter, MaxCount) };
        }

        public override BasicOutputType<PersonBirthdate[]> Aggregate(object[] results)
        {
            return new BasicOutputType<PersonBirthdate[]>()
            {
                StandardRetur = StandardReturType.OK(),
                Item = results[0] as PersonBirthdate[],
            };
        }
    }
}
