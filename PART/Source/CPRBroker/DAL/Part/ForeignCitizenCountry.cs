using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class ForeignCitizenCountry
    {
        public CountryIdentificationCodeType ToXmlType()
        {
            return new CountryIdentificationCodeType()
            {
                scheme = (_CountryIdentificationSchemeType)CountryCodeScheme,
                Value = CountryCode
            };
        }

        public static ForeignCitizenCountry FromXmlType(CountryIdentificationCodeType oio, bool isNationality, int ordinal)
        {
            return new ForeignCitizenCountry()
            {
                CountryCode = oio.Value,
                CountryCodeScheme = (int)oio.scheme,
                IsNationality = isNationality,
                Ordinal = ordinal,
            };
        }

        public static ForeignCitizenCountry[] FromXmlType(CountryIdentificationCodeType[] countries, bool isNationality)
        {
            int ordinal = 0;
            return countries
                .Select(c => ForeignCitizenCountry.FromXmlType(c, isNationality, ordinal++))
                .ToArray();
        }

        public static CountryIdentificationCodeType[] ToXmlType(this EntitySet<ForeignCitizenCountry> fcc, bool isNationality)
        {
            return fcc.Where(f => f.IsNationality = isNationality).OrderBy(f => f.Ordinal).Select(f => f.ToXmlType()).ToArray();
        }
    }


}
