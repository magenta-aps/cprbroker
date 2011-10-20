using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {
        private TilstandListeType ToTilstandListeType()
        {

            return new TilstandListeType()
            {
                CivilStatus = new CivilStatusType()
                {
                    CivilStatusKode = Converters.ToCivilStatusKodeType(MaritalStatus),
                    //TODO: Check if this is the mariage start or end date
                    TilstandVirkning = TilstandVirkningType.Create(Converters.ToDateTime(MaritalStatusTimestamp, MaritalStatusTimestampUncertainty))
                },
                LivStatus = new LivStatusType()
                {
                    LivStatusKode = Converters.ToLivStatusKodeType(CitizenStatusCode, Converters.ToDateTime(Birthdate, BirthdateUncertainty)),
                    //TODO: Ensure the dates are correct
                    TilstandVirkning = TilstandVirkningType.Create(Converters.ToDateTime(CitizenStatusTimestamp, CitizenStatusTimestampUncertainty))
                },
                LokalUdvidelse = ToLokalUdvidelseType()
            };
        }
    }
}