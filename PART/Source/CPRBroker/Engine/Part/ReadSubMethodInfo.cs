using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class ReadSubMethodInfo : SubMethodInfo<IPartReadDataProvider, RegistreringType1>
    {
        PersonIdentifier PersonIdentifier;
        LaesInputType Input;
        Func<string, Guid> CprToUuidConverter;
        internal QualityLevel? QualityLevel;

        private ReadSubMethodInfo()
        {
            FailIfNoDataProvider = true;
            FailOnDefaultOutput = true;
        }

        public ReadSubMethodInfo(PersonIdentifier pId, LaesInputType input, Func<string, Guid> cprFunc, LocalDataProviderUsageOption localAction)
            : this()
        {
            PersonIdentifier = pId;
            this.Input = input;
            this.CprToUuidConverter = cprFunc;
            LocalDataProviderOption = localAction;
            UpdateMethod = (personRegistration) => Local.UpdateDatabase.UpdatePersonRegistration(PersonIdentifier, personRegistration);
        }

        public override RegistreringType1 RunMainMethod(IPartReadDataProvider prov)
        {
            return prov.Read
           (
                this.PersonIdentifier,
               Input,
               CprToUuidConverter,
               out QualityLevel
           );
        }

        public override bool IsValidResult(CprBroker.Schemas.Part.RegistreringType1 result)
        {
            if (base.IsValidResult(result))
            {
                DateTime? d = result.TidspunktDatoTid.ToDateTime();
                if (d.HasValue)
                {
                    return Input.DateRangeIncludes(d.Value);
                }
            }
            return false;
        }

    }
}
