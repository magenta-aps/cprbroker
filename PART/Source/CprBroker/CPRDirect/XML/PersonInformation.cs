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
            return Converters.ToPnrStringOrNull(this.PNR);
        }

        public PersonGenderCodeType ToPersonGenderCodeType()
        {
            return Converters.ToPersonGenderCodeType(this.Gender);
        }

    }
}
