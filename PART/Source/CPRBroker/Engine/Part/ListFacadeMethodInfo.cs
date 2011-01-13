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

        public ListFacadeMethodInfo(ListInputType inp, string appToken, string userToken, bool appTokenRequired)
            : base(appToken, userToken, appTokenRequired)
        {
            input = inp;
            this.InitializationMethod = new Action(InitializationMethod);
        }

        public override bool IsValidInput(ref ListOutputType1 invaliInputReturnValue)
        {
            if (input == null || input.UUID == null || input.UUID.Length == 0)
            {
                invaliInputReturnValue = new ListOutputType1()
                {
                    StandardRetur = new ErrorCode.NullInputErrorCode().ToStandardReturn()
                };
                return false;
            }

            var invalidUuidErrors = (from uuid in input.UUID where !Util.Strings.IsGuid(uuid) select new ErrorCode.InvalidUuidErrorCode(uuid)).ToArray();
            if (invalidUuidErrors.Length > 0)
            {
                invaliInputReturnValue = new ListOutputType1()
                {
                    StandardRetur = ErrorCode.Create<ErrorCode.InvalidUuidErrorCode>(invalidUuidErrors)
                };
                return false;
            }

            var unknownUuidErrors = new List<ErrorCode.UnknownUuidErrorCode>();
            foreach (var inputPersonUuid in input.UUID)
            {
                var personIdentifier = DAL.Part.PersonMapping.GetPersonIdentifier(new Guid(inputPersonUuid));
                if (personIdentifier == null)
                {
                    unknownUuidErrors.Add(new ErrorCode.UnknownUuidErrorCode(inputPersonUuid));
                }
                else
                {
                    inputUuidToPersonIdentifierMap.Add(inputPersonUuid, personIdentifier);
                }
            }
            if (unknownUuidErrors.Count > 0)
            {
                invaliInputReturnValue = new ListOutputType1()
                {
                    StandardRetur = ErrorCode.Create<ErrorCode.UnknownUuidErrorCode>(unknownUuidErrors.ToArray())
                };
                return false;
            }

            return true;
        }

        public override void Initialize()
        {
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
                LaesResultat = Array.ConvertAll<object, LaesResultatType>
                (
                    results,
                    (s) => (s is RegistreringType1) ? new LaesResultatType() { Item = s as RegistreringType1 } : null
                ),
                //TODO: Fill this StandardRetur object
                StandardRetur = StandardReturType.Create("", "")
            };
        }

    }
}
