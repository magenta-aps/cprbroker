using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class ReadFacadeMethodInfo : FacadeMethodInfo<LaesOutputType>
    {
        LaesInputType Input = null;
        PersonIdentifier pId = null;
        LocalDataProviderUsageOption LocalAction;
        public QualityLevel? QualityLevel;

        private ReadFacadeMethodInfo()
        { }

        public ReadFacadeMethodInfo(LaesInputType input, LocalDataProviderUsageOption localAction, string appToken, string userToken)
            : base(appToken, userToken)
        {
            this.Input = input;
            this.LocalAction = localAction;
        }

        public override bool IsValidInput(ref LaesOutputType invaliInputReturnValue)
        {
            if (Input == null)
            {
                invaliInputReturnValue = new LaesOutputType()
                {
                    StandardRetur = new ErrorCode.NullInputErrorCode().ToStandardReturn()
                };
                return false;
            }

            if (!Util.Strings.IsGuid(Input.UUID))
            {
                invaliInputReturnValue = new LaesOutputType()
                {
                    StandardRetur = new ErrorCode.InvalidUuidErrorCode(Input.UUID).ToStandardReturn()
                };
                return false;
            }

            pId = DAL.Part.PersonMapping.GetPersonIdentifier(new Guid(Input.UUID));
            if (pId == null)
            {
                invaliInputReturnValue = new LaesOutputType()
                {
                    StandardRetur = new ErrorCode.UnknownUuidErrorCode(Input.UUID).ToStandardReturn()
                };
                return false;
            }

            return true;
        }

        public override void Initialize()
        {
            //TODO: Do not authenticate web method into this call because it will throw an exception
            SubMethodInfos = new SubMethodInfo[] 
            {
                new ReadSubMethodInfo(pId,Input,(cpr)=>Manager.Part.GetPersonUuid(UserToken, ApplicationToken, cpr),LocalAction)
            };
        }

        public override LaesOutputType Aggregate(object[] results)
        {
            LaesOutputType o = new LaesOutputType()
            {
                LaesResultat = new LaesResultatType()
                {
                    Item = results[0]
                },
                //TODO: Fill this StandardRetur object
                StandardRetur = new StandardReturType()
                {
                    FejlbeskedTekst = "",
                    StatusKode = ""
                }
            };
            QualityLevel = (SubMethodInfos[0] as ReadSubMethodInfo).QualityLevel;
            return o;
        }
    }
}
