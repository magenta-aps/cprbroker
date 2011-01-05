using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class ListFacadeMethodInfo : FacadeMethodInfo<ListOutputType1>
    {
        public ListInputType input;
        public Dictionary<string, PersonIdentifier> inputUuidToPersonIdentifierMap = null;

        private ListFacadeMethodInfo()
        { }

        public ListFacadeMethodInfo(ListInputType inp, string appToken, string userToken, bool appTokenRequired)
            : base(appToken, userToken, appTokenRequired)
        {
            input = inp;
            this.InitializationMethod = new Action(InitializationMethod);
        }

        public override void Initialize()
        {
            inputUuidToPersonIdentifierMap = new Dictionary<string, PersonIdentifier>();
            foreach (var inputPersonUuid in input.UUID)
            {
                var personIdentifier = DAL.Part.PersonMapping.GetPersonIdentifier(new Guid(inputPersonUuid));
                //TODO: Do not throw exception, instead, return null. This affects the following delegates too
                if (personIdentifier == null)
                {
                    throw new Exception(TextMessages.UuidNotFound);
                }
                inputUuidToPersonIdentifierMap.Add(inputPersonUuid, personIdentifier);
            }
        }

        public override ListOutputType1 Aggregate(object[] results)
        {
            return new ListOutputType1()
            {
                LaesResultat = new List<LaesResultatType>
                (
                    Array.ConvertAll<object, LaesResultatType>
                    (
                        results,
                        (s) => (s is RegistreringType1) ? new LaesResultatType() { Item = s as RegistreringType1 } : null
                    )
                ),
                //TODO: Fill this StandardRetur object
                StandardRetur = StandardReturType.Create("", "")
            };
        }

    }
}
