using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    public partial class CountryRef
    {
        public static CountryIdentificationCodeType ToXmlType(CountryRef db)
        {
            if (db != null)
            {
                return new CountryIdentificationCodeType()
                {
                    scheme = (_CountryIdentificationSchemeType)db.CountrySchemeTypeId,
                    Value = db.Value
                };
            }
            return null;
        }

        public static CountryRef FromXmlType(CountryIdentificationCodeType oio)
        {
            if (oio != null)
            {
                return new CountryRef()
                {
                    CountryRefId = Guid.NewGuid(),
                    Value = oio.Value,
                    CountrySchemeTypeId = (int)oio.scheme,
                };
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<CountryRef>(cr => cr.CountrySchemeType);
        }
    }
}
