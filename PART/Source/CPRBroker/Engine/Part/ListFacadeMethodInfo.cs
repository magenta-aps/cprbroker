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
        public Dictionary<string, PersonIdentifier> inputUuidToPersonIdentifierMap = new Dictionary<string, PersonIdentifier>();

        private ListFacadeMethodInfo()
        { }

        public ListFacadeMethodInfo(ListInputType inp, string appToken, string userToken)
            : base(appToken, userToken)
        {
            input = inp;
            this.InitializationMethod = new Action(InitializationMethod);
        }

        public override StandardReturType ValidateInput()
        {
            if (input == null || input.UUID == null || input.UUID.Length == 0)
            {
                return StandardReturType.NullInput();
            }

            foreach (var uuid in input.UUID)
            {
                if (string.IsNullOrEmpty(uuid))
                {
                    return StandardReturType.NullInput();
                }
            }

            var invalidUuids = (from uuid in input.UUID where !Util.Strings.IsGuid(uuid) select uuid).ToArray();
            if (invalidUuids.Length > 0)
            {
                return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, String.Join(",", invalidUuids));
            }

            var unknownUuidErrors = new List<String>();
            foreach (var inputPersonUuid in input.UUID)
            {
                var personIdentifier = DAL.Part.PersonMapping.GetPersonIdentifier(new Guid(inputPersonUuid));
                if (personIdentifier == null)
                {
                    unknownUuidErrors.Add("uuid " + inputPersonUuid + "valid but not found");
                }
                else
                {
                    inputUuidToPersonIdentifierMap.Add(inputPersonUuid, personIdentifier);
                }
            }
            if (unknownUuidErrors.Count > 0)
            {
                return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, String.Join(",", unknownUuidErrors.ToArray()));
            }

            return StandardReturType.OK();
        }

        public override void Initialize()
        {
            SubMethodInfos = Array.ConvertAll<string, SubMethodInfo>
            (
                input.UUID.ToArray(),
                (pUUID) => new ReadSubMethodInfo(
                    inputUuidToPersonIdentifierMap[pUUID],
                    LaesInputType.Create(pUUID, input),
                    LocalDataProviderUsageOption.UseFirst)
           );
        }

        public override ListOutputType1 Aggregate(object[] results)
        {
            return new ListOutputType1()
            {
                LaesResultat = Array.ConvertAll<object, LaesResultatType>
                (
                    results,
                    (s) => (s is RegistreringType1) ? new LaesResultatType() { Item = s as RegistreringType1 } : null
                ),
                //TODO: Fill this StandardRetur object
                StandardRetur = StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED)
            };
        }

    }
}
