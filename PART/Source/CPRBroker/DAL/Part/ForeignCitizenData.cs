using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class ForeignCitizenData
    {
        public UdenlandskBorgerType ToXmlType()
        {
            return new UdenlandskBorgerType()
                {
                    FoedselslandKode = CountryCodeScheme.HasValue ? CountryIdentificationCodeType.Create((_CountryIdentificationSchemeType)CountryCodeScheme.Value, CountryCode) : null,
                    PersonCivilRegistrationReplacementIdentifier = CivilRegistrationReplacementIdentifier,
                    PersonIdentifikator = PersonIdentifier,
                    SprogKode = ForeignCitizenCountry.ToXmlType(ForeignCitizenCountries, false),
                    PersonNationalityCode = ForeignCitizenCountry.ToXmlType(ForeignCitizenCountries, true),
                };
        }

        public static ForeignCitizenData FromXmlType(UdenlandskBorgerType oio)
        {
            var ret = new ForeignCitizenData()
            {
                CountryCode = oio.FoedselslandKode.Value,
                CountryCodeScheme = (int)oio.FoedselslandKode.scheme,
                CivilRegistrationReplacementIdentifier = oio.PersonCivilRegistrationReplacementIdentifier,
                PersonIdentifier = oio.PersonIdentifikator,
            };

            ret.ForeignCitizenCountries.AddRange(ForeignCitizenCountry.FromXmlType(oio.SprogKode, false));
            ret.ForeignCitizenCountries.AddRange(ForeignCitizenCountry.FromXmlType(oio.PersonNationalityCode, true));
            
            return ret;
        }
    }
}
