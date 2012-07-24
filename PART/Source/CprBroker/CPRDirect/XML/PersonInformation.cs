using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Util;

namespace CprBroker.Providers.CPRDirect
{
    public partial class PersonInformationType
    {
        public LivStatusType ToLivStatusType()
        {
            return new LivStatusType()
            {
                // Passing true in order to prevent people with unknown/uncertain birthdates from being Prenatal
                // TODO: Check how to detect prenatal in CPR Direct
                LivStatusKode = Enums.ToLifeStatus(this.Status, true),
                TilstandVirkning = TilstandVirkningType.Create(this.ToStatusDate())
            };
        }

        public DateTime? ToBirthdate(bool tryPnr = false)
        {
            // First, look at birthdate field
            var val = Converters.ToDateTime(this.Birthdate, this.BirthdateUncertainty);
            if (val.HasValue || !tryPnr)
                return val;

            // Now try the current CPR number - it should be more recent than PNR
            val = Utilities.Strings.PersonNumberToDate(this.CurrentCprNumber);
            if (val.HasValue)
                return val;

            // Finally, use PNR
            val = Utilities.Strings.PersonNumberToDate(this.ToPnr());
            return val;
        }

        public DateTime? ToStatusDate()
        {
            return Converters.ToDateTime(this.StatusStartDate, this.StatusDateUncertainty);
        }

        public string ToPnr()
        {
            // TODO: Shall we use CurrentCprNumber instead - as that is the new CPR number? I guess answer is no !!
            return Converters.ToPnrStringOrNull(this.PNR);
        }

        public VirkningType ToVirkningType()
        {
            return VirkningType.Create(
                Converters.ToDateTime(this.PersonStartDate, this.PersonStartDateUncertainty),
                Converters.ToDateTime(this.PersonEndDate, this.PersonEndDateUncertainty)
                );
        }

        public PersonGenderCodeType ToPersonGenderCodeType()
        {
            return Converters.ToPersonGenderCodeType(this.Gender);
        }

        public bool ToPersonNummerGyldighedStatusIndikator()
        {
            return Enums.IsActiveCivilRegistrationStatus(this.Status);
        }

    }
}
