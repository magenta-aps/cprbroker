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
        private static TilstandListeType ToTilstandListeType(Citizen citizen)
        {

            return new TilstandListeType()
            {
                CivilStatus = new CivilStatusType()
                {
                    CivilStatusKode = Converters.ToCivilStatusKodeType(citizen.MaritalStatus),
                    //TODO: Check if this is the mariage start or end date
                    TilstandVirkning = TilstandVirkningType.Create(Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty))
                },
                LivStatus = new LivStatusType()
                {
                    LivStatusKode = Converters.ToLivStatusKodeType(citizen.CitizenStatusCode, Converters.ToDateTime(citizen.Birthdate, citizen.BirthdateUncertainty)),
                    //TODO: Ensure the dates are correct
                    TilstandVirkning = TilstandVirkningType.Create(Converters.ToDateTime(citizen.CitizenStatusTimestamp, citizen.CitizenStatusTimestampUncertainty))
                },
                LokalUdvidelse = ToLokalUdvidelseType(citizen)
            };
        }
    }
}