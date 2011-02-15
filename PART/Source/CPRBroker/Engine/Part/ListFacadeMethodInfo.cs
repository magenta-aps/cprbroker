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

        public override bool IsValidInput(ref ListOutputType1 invalidInputReturnValue)
        {
            if (input == null || input.UUID == null || input.UUID.Length == 0)
            {
                invalidInputReturnValue = new ListOutputType1()
                {
                    StandardRetur = new ErrorCode.NullInputErrorCode().ToStandardReturn()
                };
                return false;
            }

            var invalidUuidErrors = (from uuid in input.UUID where !Util.Strings.IsGuid(uuid) select new ErrorCode.InvalidUuidErrorCode(uuid).ToString()).ToArray();
            if (invalidUuidErrors.Length > 0)
            {
                invalidInputReturnValue = new ListOutputType1()
                {
                    StandardRetur = StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, String.Join(",",invalidUuidErrors))
                };
                return false;
            }

            var unknownUuidErrors = new List<String>();
            foreach (var inputPersonUuid in input.UUID)
            {
                var personIdentifier = DAL.Part.PersonMapping.GetPersonIdentifier(new Guid(inputPersonUuid));
                if (personIdentifier == null)
                {
                    unknownUuidErrors.Add("uuid "+ inputPersonUuid+ "valid but not found");
                }
                else
                {
                    inputUuidToPersonIdentifierMap.Add(inputPersonUuid, personIdentifier);
                }
            }
            if (unknownUuidErrors.Count > 0)
            {
                invalidInputReturnValue = new ListOutputType1()
                {
                    StandardRetur = StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST,String.Join(",",unknownUuidErrors.ToArray()))
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
                StandardRetur = StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED)
            };
        }

    }
}
