using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;

namespace CprBroker.Providers.DPR
{
    public partial class Nationality
    {
        public CountryIdentificationCodeType ToCountryIdentificationCodeType()
        {
            return CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, CountryCode.ToDecimalString());
        }
    }
}
