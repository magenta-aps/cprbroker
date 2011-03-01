using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    /// <summary>
    /// Sub method info for Read and RefreshRead
    /// </summary>
    public class ReadSubMethodInfo : SubMethodInfo<IPartReadDataProvider, RegistreringType1>
    {
        PersonIdentifier PersonIdentifier;
        LaesInputType Input;
        internal QualityLevel? QualityLevel;

        private ReadSubMethodInfo()
        {
            FailIfNoDataProvider = true;
            FailOnDefaultOutput = true;
        }

        public ReadSubMethodInfo(PersonIdentifier pId, LaesInputType input, LocalDataProviderUsageOption localAction)
            : this()
        {
            PersonIdentifier = pId;
            this.Input = input;
            LocalDataProviderOption = localAction;
            UpdateMethod = (personRegistration) => Local.UpdateDatabase.UpdatePersonRegistration(PersonIdentifier, personRegistration);
        }

        public override RegistreringType1 RunMainMethod(IPartReadDataProvider prov)
        {
            return prov.Read
           (
                this.PersonIdentifier,
               Input,
               (cprNumber) => CprToUuid(cprNumber),
               out QualityLevel
           );
        }

        public override bool IsValidResult(CprBroker.Schemas.Part.RegistreringType1 result)
        {
            if (base.IsValidResult(result))
            {
                DateTime? d = result.Tidspunkt.ToDateTime();
                if (d.HasValue)
                {
                    return Input.DateRangeIncludes(d.Value);
                }
            }
            return false;
        }

        private Guid CprToUuid(string cprNumber)
        {
            var uuid = Manager.Part.GetUuid(BrokerContext.Current.UserToken, BrokerContext.Current.ApplicationToken, cprNumber);
            if (StandardReturType.IsSucceeded(uuid.StandardRetur))
            {
                return new Guid(uuid.UUID);
            }
            return Guid.Empty;
        }

    }
}
