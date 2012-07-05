using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CurrentCitizenshipType
    {
        public CountryIdentificationCodeType ToPersonNationalityCode()
        {
            return CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, this.StringCountryCode);
        }

        public string StringCountryCode
        {
            get { return Converters.DecimalToString(this.CountryCode); }
        }

        public DateTime? ToCitizenshipStartDate()
        {
            return Converters.ToDateTime(this.CitizenshipStartDate, this.CitizenshipStartDateUncertainty);
        }
    }
}
