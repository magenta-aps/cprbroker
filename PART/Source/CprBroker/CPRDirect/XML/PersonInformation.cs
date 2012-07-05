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
                LivStatusKode = Enums.ToLifeStatus(this.Status, this.ToBirthdate()),
                TilstandVirkning = TilstandVirkningType.Create(this.ToStatusDate())
            };
        }

        public DateTime? ToBirthdate()
        {
            return Converters.ToDateTime(this.Birthdate, this.BirthdateUncertainty);
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
