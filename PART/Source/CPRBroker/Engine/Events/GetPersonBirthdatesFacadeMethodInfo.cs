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
        }

        public override bool IsValidInput(ref BasicOutputType<PersonBirthdate[]> invalidInputReturnValue)
        {
            return MaxCount > 0;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[] { new GetPersonBirthdatesSubmethodInfo(PersonUuidToStartAfter, MaxCount) };
        }
    }
}
