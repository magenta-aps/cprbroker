using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class ForeignCitizenData
    {
        public static UdenlandskBorgerType ToXmlType(ForeignCitizenData db)
        {
            if (db != null)
            {
                return new UdenlandskBorgerType()
                    {
                        FoedselslandKode = CountryRef.ToXmlType(db.BirthCountryRef),
                        PersonCivilRegistrationReplacementIdentifier = db.CivilRegistrationReplacementIdentifier,
                        PersonIdentifikator = db.PersonIdentifier,
                        SprogKode = ForeignCitizenCountry.ToXmlType(db.ForeignCitizenCountries, false),
                        PersonNationalityCode = ForeignCitizenCountry.ToXmlType(db.ForeignCitizenCountries, true),
                    };
            }
            return null;
        }

        public static ForeignCitizenData FromXmlType(RegisterOplysningType[] oio)
        {
            if (oio != null && oio.Length > 0 && oio[0] != null)
            {
                return FromXmlType(oio[0].Item as UdenlandskBorgerType);
            }
            return null;
        }

        public static ForeignCitizenData FromXmlType(UdenlandskBorgerType oio)
        {
            if (oio != null)
            {
                var ret = new ForeignCitizenData()
                {
                    BirthCountryRef = CountryRef.FromXmlType(oio.FoedselslandKode),
                    CivilRegistrationReplacementIdentifier = oio.PersonCivilRegistrationReplacementIdentifier,
                    PersonIdentifier = oio.PersonIdentifikator,
                };

                ret.ForeignCitizenCountries.AddRange(ForeignCitizenCountry.FromXmlType(oio.SprogKode, false));
                ret.ForeignCitizenCountries.AddRange(ForeignCitizenCountry.FromXmlType(oio.PersonNationalityCode, true));

                return ret;
            }
            return null;
        }
    }
}
