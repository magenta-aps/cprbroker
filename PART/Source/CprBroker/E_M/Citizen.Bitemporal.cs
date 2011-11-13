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
        public virtual VirkningType ToEgenskabVirkningType()
        {
            // TODO: validate outcome of this method
            var dates = new List<DateTime?>();
            dates.AddRange(new DateTime?[]
            {           
                Converters.ToDateTime(this.Birthdate,this.BirthdateUncertainty),
                Converters.ToDateTime(this.CitizenStatusTimestamp,this.CitizenStatusTimestampUncertainty),                    
                this.CprChurchTimestamp,
                this.CprPersonTimestamp,
                Converters.ToDateTime(this.DepartureTimestamp,this.DepartureTimestampUncertainty),
                Converters.ToDateTime(this.DirectoryProtectionEndDate,this.DirectoryProtectionEndDateUncertainty),
                Converters.ToDateTime(this.DirectoryProtectionDate, this.DirectoryProtectionDateUncertainty),
                Converters.ToDateTime(this.MaritalStatusTimestamp,this.MaritalStatusTimestampUncertainty),
                Converters.ToDateTime(this.MaritalStatusTerminationTimestamp,this.MaritalStatusTerminationTimestampUncertainty),
                Converters.ToDateTime(this.MunicipalityArrivalDate,this.MunicipalityArrivalDateUncertainty),
                Converters.ToDateTime(this.ChurchMarkerDate, this.ChurchMarkerDateUncertainty),
                Converters.ToDateTime(this.NationalityChangeTimestamp,this.NationalityChangeTimestampUncertainty),
                Converters.ToDateTime(this.NationalityTerminationTimestamp,this.NationalityTerminationTimestampUncertainty),
                Converters.ToDateTime(this.OccupationDate,this.OccupationDateUncertainty),
                Converters.ToDateTime(this.PNRCreationDate, this.PnrCreationdateUncertainty),
                Converters.ToDateTime(this.PNRMarkingDate,this.PnrMarkingDateUncertainty),
                Converters.ToDateTime(this.RelocationTimestamp,this.RelocationTimestampUncertainty),
            });
            var maxDate = Converters.GetMaxDate(dates.ToArray());
            return VirkningType.Create(maxDate, null);
        }

        public virtual TidspunktType ToTidspunktType()
        {
            // TODO: Add a more meaningful date to registration date input
            return TidspunktType.Create(Converters.GetMaxDate(
                Converters.ToDateTime(this.BirthRegistrationDate, this.BirthRegistrationDateUncertainty)
                ));
        }

    }
}