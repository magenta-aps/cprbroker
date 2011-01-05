using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    //TODO: Modify to never return null even if input is wrong
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

        public override bool IsValidInput()
        {
            if (input == null || input.UUID == null && input.UUID.Count == 0)
            {
                return false;
            }
            if (input.UUID
                .Any((uuid) => string.IsNullOrEmpty(uuid))
                )
            {
                return false;
            }
            return true;
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
            //TODO: Could fail if Input.UUID is null
            SubMethodInfos = Array.ConvertAll<string, SubMethodInfo>
            (
                input.UUID.ToArray(),
                (pUUID) => new ReadSubMethodInfo(
                    inputUuidToPersonIdentifierMap[pUUID],
                    LaesInputType.Create(pUUID, input),
                    (cpr) => Manager.Part.GetPersonUuid(UserToken, ApplicationToken, cpr),
                    LocalDataProviderUsageOption.UseFirst)
           );
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
