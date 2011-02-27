using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;

namespace CprBroker.Engine.Part
{
    public class ReadFacadeMethodInfo : FacadeMethodInfo<LaesOutputType, LaesResultatType>
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

        public override StandardReturType ValidateInput()
        {
            if (Input == null)
            {
                return StandardReturType.NullInput();
            }

            if (!Strings.IsGuid(Input.UUID))
            {
                return StandardReturType.InvalidUuid(Input.UUID);
            }

            pId = DAL.Part.PersonMapping.GetPersonIdentifier(new Guid(Input.UUID));
            if (pId == null)
            {
                return StandardReturType.UnknownUuid(Input.UUID);
            }

            return StandardReturType.OK();
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[] 
            {
                new ReadSubMethodInfo(pId, Input, LocalAction)
            };
        }

        public override LaesResultatType Aggregate(object[] results)
        {
            var LaesResultat = new LaesResultatType()
            {
                Item = results[0]
            };

            QualityLevel = (SubMethodInfos[0] as ReadSubMethodInfo).QualityLevel;
            return LaesResultat;
        }
    }
}
