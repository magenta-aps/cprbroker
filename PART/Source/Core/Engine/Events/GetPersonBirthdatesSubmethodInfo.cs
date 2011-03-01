using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Events
{
    /// <summary>
    /// Sub method for GetPersonBirthdates
    /// </summary>
    public class GetPersonBirthdatesSubmethodInfo : SubMethodInfo<IDataChangeEventManager, Schemas.Part.Events.PersonBirthdate[]>
    {
        Guid? PersonUuidToStartAfter;
        int MaxCount;

        public GetPersonBirthdatesSubmethodInfo(Guid? personUuidToStartAfter, int maxCount)
        {
            this.PersonUuidToStartAfter = personUuidToStartAfter;
            this.MaxCount = maxCount;
        }

        public override CprBroker.Schemas.Part.Events.PersonBirthdate[] RunMainMethod(IDataChangeEventManager prov)
        {
            return prov.GetPersonBirthdates(PersonUuidToStartAfter, MaxCount);
        }
    }
}
