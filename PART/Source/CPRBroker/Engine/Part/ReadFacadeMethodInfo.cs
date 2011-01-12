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

        public ReadFacadeMethodInfo(LaesInputType input, LocalDataProviderUsageOption localAction, string appToken, string userToken, bool appTokenRequired)
            : base(appToken, userToken, appTokenRequired)
        {
            this.Input = input;
            this.LocalAction = localAction;
        }

        public override bool IsValidInput()
        {
            return base.IsValidInput();
        }

        public override void Initialize()
        {
            //TODO: Do not authenticate web method into this call because it will throw an exception
            pId = DAL.Part.PersonMapping.GetPersonIdentifier(new Guid(Input.UUID));
            if (pId == null)
            {
                throw new Exception(TextMessages.UuidNotFound);
            }

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
                    StatuskodeKode = ""
                }
            };
            QualityLevel = (SubMethodInfos[0] as ReadSubMethodInfo).QualityLevel;
            return o;
        }
    }
}
