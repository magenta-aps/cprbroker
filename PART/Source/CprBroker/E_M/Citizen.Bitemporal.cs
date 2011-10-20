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
        private VirkningType ToVirkningType()
        {
            // TODO: validate outcome of this method
            var dates = new List<DateTime?>();
            dates.AddRange(new DateTime?[]
            {           
                
                    Converters.ToDateTime(Birthdate,BirthdateUncertainty),
                    Converters.ToDateTime(CitizenStatusTimestamp,CitizenStatusTimestampUncertainty),                    
                    CpcNationalChurchTimestamp,
                    CprPersonTimestamp,
                    Converters.ToDateTime(DepartureTimestamp,DepartureTimestampUncertainty),
                    Converters.ToDateTime(DirectoryProtectionEndDate,DirectoryProtectionRemovalDateUncertainty),
                    Converters.ToDateTime(DiretoryProtectionDate, DirectoryProtectionDateUncertainty),
                    Converters.ToDateTime(MaritalStatusTimestamp,MaritalStatusTimestampUncertainty),
                    Converters.ToDateTime(MaritalStatusTerminationTimestamp,MaritalStatusTerminationTimestampUncertainty),
                    Converters.ToDateTime(MunicipalityArrivalDate,MunicipalityArrivalTimestampUncertainty),
                    Converters.ToDateTime(NationalChurchMarkerDate, NationalChurchMarkerUncertainty),
                    Converters.ToDateTime(NationalityChangeTimestamp,NationalityChangeTimestampUncertainty),
                    Converters.ToDateTime(NationalityTerminationTimestamp,NationalityTerminationTimestampUncertainty),
                    Converters.ToDateTime(OccupationDate,OccupationDateUncertainty),
                    Converters.ToDateTime(PNRCreationDate, PnrCreationdateUncertainty),
                    Converters.ToDateTime(PNRMarkingDate,PnrMarkingDateUncertainty)                    
            });
            foreach (var res in Resettlements)
            {
                dates.Add(Converters.ToDateTime(res.ArrivalTimestamp, res.ArrivalTimestampUncertainty));
                dates.Add(res.CprResettlementTimestamp);
                dates.Add(Converters.ToDateTime(res.DepartureTimestamp, res.DepartureTimestampUncertainty));
                dates.Add(Converters.ToDateTime(res.MunicipalityArrivalDate, res.MunicipalityArrivalDateUncertainty));
                dates.Add(Converters.ToDateTime(res.MunicipalityDepartureDate, res.MunicipalityDepartureDateUncertainty));
            }
            return VirkningType.Create(Converters.GetMaxDate(dates.ToArray()), null);

        }

        private TidspunktType ToTidspunktType()
        {
            // TODO: Add a more meaningful date to registration date input
            return TidspunktType.Create(Converters.GetMaxDate(
                Converters.ToDateTime(BirthRegistrationDate, BirthRegistrationDateUncertainty)
                ));
        }

    }
}